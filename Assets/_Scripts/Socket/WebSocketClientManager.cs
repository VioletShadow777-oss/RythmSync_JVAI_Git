using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using NativeWebSocket;

public class WebSocketClientManager : MonoBehaviour
{
    public static WebSocketClientManager Instance { get; private set; }

    [Header("Connection Fallback")]
    [SerializeField] private string serverHost = "127.0.0.1";
    [SerializeField] private int serverPort = 8080;
    [SerializeField] private bool connectOnStart = true;
    [SerializeField] private int currentSongId = 1;

    private WebSocket websocket;
    private readonly Queue<Action> mainThreadActions = new Queue<Action>();
    private bool isConnecting = false;
    private bool resultSent = false;

    public bool IsConnected => websocket != null && websocket.State == WebSocketState.Open;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void LoadConnectionConfig()
    {
        ServerConfig config = ConfigLoader.LoadServerConfig();

        if (config != null)
        {
            if (!string.IsNullOrWhiteSpace(config.host))
                serverHost = config.host;

            if (config.port > 0)
                serverPort = config.port;

            connectOnStart = config.autoConnect;
        }

        Debug.Log($"[WS Config] Host: {serverHost}, Port: {serverPort}, AutoConnect: {connectOnStart}");
    }

    private async void Start()
    {
        LoadConnectionConfig();

        if (connectOnStart)
        {
            await Connect();
        }
    }

    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif

        while (true)
        {
            Action action = null;

            lock (mainThreadActions)
            {
                if (mainThreadActions.Count == 0)
                    break;

                action = mainThreadActions.Dequeue();
            }

            action?.Invoke();
        }
    }

    public async System.Threading.Tasks.Task Connect()
    {
        if (isConnecting || IsConnected)
            return;

        isConnecting = true;

        string url = $"ws://{serverHost}:{serverPort}";
        websocket = new WebSocket(url);

        websocket.OnOpen += () =>
        {
            EnqueueMainThread(() =>
            {
                Debug.Log("[WS] Connected to backend.");
                SendReady();
            });
        };

        websocket.OnError += (errorMsg) =>
        {
            EnqueueMainThread(() =>
            {
                Debug.LogError("[WS] Error: " + errorMsg);
            });
        };

        websocket.OnClose += (closeCode) =>
        {
            EnqueueMainThread(() =>
            {
                Debug.LogWarning("[WS] Connection closed. Code: " + closeCode);
            });
        };

        websocket.OnMessage += (bytes) =>
        {
            string message = Encoding.UTF8.GetString(bytes);
            EnqueueMainThread(() => HandleIncomingMessage(message));
        };

        try
        {
            await websocket.Connect();
        }
        catch (Exception ex)
        {
            Debug.LogError("[WS] Connect failed: " + ex.Message);
        }
        finally
        {
            isConnecting = false;
        }
    }

    private void EnqueueMainThread(Action action)
    {
        lock (mainThreadActions)
        {
            mainThreadActions.Enqueue(action);
        }
    }

    private void HandleIncomingMessage(string rawJson)
    {
        Debug.Log("[WS] Received: " + rawJson);

        SocketEnvelope envelope = JsonUtility.FromJson<SocketEnvelope>(rawJson);
        if (envelope == null || string.IsNullOrEmpty(envelope.command))
        {
            Debug.LogWarning("[WS] Invalid message: " + rawJson);
            return;
        }

        switch (envelope.command)
        {
            case "server_connected":
                break;

            case "start":
                HandleStart(rawJson);
                break;

            case "stop":
                HandleStop();
                break;

            case "ping":
                SendPong();
                break;

            case "status":
                SendState();
                break;

            default:
                Debug.LogWarning("[WS] Unknown command: " + envelope.command);
                break;
        }
    }

    private void HandleStart(string rawJson)
    {
        StartCommandMessage msg = JsonUtility.FromJson<StartCommandMessage>(rawJson);
        if (msg != null && msg.song > 0)
        {
            currentSongId = msg.song;
        }

        SongItemConfig selectedSong = ConfigLoader.GetSongById(currentSongId);
        AudioClip selectedClip = null;

        if (selectedSong != null)
        {
            Debug.Log($"[WS] Starting song id={selectedSong.id}, key={selectedSong.key}, name={selectedSong.displayName}");

            if (SongRegistry.Instance != null)
            {
                selectedClip = SongRegistry.Instance.GetClipByKey(selectedSong.key);
            }

            if (selectedClip != null)
            {
                Debug.Log($"[WS] Found audio clip for key: {selectedSong.key}");
            }
            else
            {
                Debug.LogWarning($"[WS] No AudioClip mapped for key: {selectedSong.key}");
            }
        }
        else
        {
            Debug.LogWarning($"[WS] No song mapping found for song id {currentSongId}");
        }

        resultSent = false;

        BeatDetector beatDetector = FindFirstObjectByType<BeatDetector>();

        if (beatDetector != null && selectedClip != null)
        {
            beatDetector.SetSong(selectedClip);
        }
        else if (beatDetector == null)
        {
            Debug.LogWarning("[WS] BeatDetector not found in scene.");
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
            SendState();
        }
        else
        {
            Debug.LogError("[WS] GameManager.Instance not found.");
        }
    }

    private void HandleStop()
    {
        resultSent = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndGame();
        }

        SendStopped();
        SendState();
    }

    public async void SendReady()
    {
        await SendJson(new SocketEnvelope { command = "ready" });
    }

    public async void SendPong()
    {
        await SendJson(new SocketEnvelope { command = "pong" });
    }

    public async void SendStopped()
    {
        await SendJson(new SocketEnvelope { command = "stopped" });
    }

    public async void SendState()
    {
        string state = GameManager.Instance != null ? GameManager.Instance.CurrentState.ToString() : "Unknown";

        StateMessage msg = new StateMessage
        {
            command = "state",
            state = state,
            song = currentSongId
        };

        await SendJson(msg);
    }

    public async void SendScore(string rating)
    {
        if (ScoreManager.Instance == null)
            return;

        ScoreMessage msg = new ScoreMessage
        {
            command = "score",
            song = currentSongId,
            rating = rating,
            score = ScoreManager.Instance.Score,
            combo = ScoreManager.Instance.Combo
        };

        await SendJson(msg);
    }

    public async void SendFinalResult()
    {
        if (resultSent)
            return;

        if (ScoreManager.Instance == null)
            return;

        ResultMessage msg = new ResultMessage
        {
            command = "result",
            song = currentSongId,
            finalScore = ScoreManager.Instance.Score,
            perfect = ScoreManager.Instance.PerfectCount,
            great = ScoreManager.Instance.GreatCount,
            good = ScoreManager.Instance.GoodCount,
            miss = ScoreManager.Instance.MissCount,
            maxCombo = ScoreManager.Instance.MaxCombo,
            grade = CalculateGrade()
        };

        resultSent = true;
        await SendJson(msg);
    }

    private string CalculateGrade()
    {
        if (ScoreManager.Instance == null)
            return "F";

        int perfect = ScoreManager.Instance.PerfectCount;
        int great = ScoreManager.Instance.GreatCount;
        int good = ScoreManager.Instance.GoodCount;
        int miss = ScoreManager.Instance.MissCount;
        int total = perfect + great + good + miss;

        if (total == 0)
            return "F";

        float performance = (perfect * 1f + great * 0.7f + good * 0.5f) / total;

        if (performance >= 0.95f) return "S";
        if (performance >= 0.85f) return "A";
        if (performance >= 0.70f) return "B";
        if (performance >= 0.55f) return "C";
        return "F";
    }

    private async System.Threading.Tasks.Task SendJson(object payload)
    {
        if (!IsConnected)
        {
            Debug.LogWarning("[WS] Cannot send. Socket not connected.");
            return;
        }

        try
        {
            string json = JsonUtility.ToJson(payload);
            await websocket.SendText(json);
            Debug.Log("[WS] Sent: " + json);
        }
        catch (Exception ex)
        {
            Debug.LogError("[WS] Send failed: " + ex.Message);
        }
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            await websocket.Close();
        }
    }
}