using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class SongLoader : MonoBehaviour
{
    // ─── External Windows File Dialog (no plugins needed) ───────────────────
    [DllImport("Comdlg32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName(ref OpenFileName ofn);

    // Windows OPENFILENAME struct
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct OpenFileName
    {
        public int lStructSize;
        public IntPtr hwndOwner;
        public IntPtr hInstance;
        public string lpstrFilter;      // File type filter
        public string lpstrCustomFilter;
        public int nMaxCustFilter;
        public int nFilterIndex;
        public string lpstrFile;        // Selected file path goes here
        public int nMaxFile;
        public string lpstrFileTitle;
        public int nMaxFileTitle;
        public string lpstrInitialDir;
        public string lpstrTitle;       // Dialog window title
        public int Flags;
        public short nFileOffset;
        public short nFileExtension;
        public string lpstrDefExt;
        public IntPtr lCustData;
        public IntPtr lpfnHook;
        public string lpTemplateName;
        public IntPtr pvReserved;
        public int dwReserved;
        public int FlagsEx;
    }

    private const int OFN_FILEMUSTEXIST = 0x00001000;
    private const int OFN_PATHMUSTEXIST = 0x00000800;
    private const int OFN_NOCHANGEDIR = 0x00000008; // Prevents Unity from losing its working directory

    // ─── Inspector Reference ─────────────────────────────────────────────────
    [Tooltip("Reference to BeatDetector which holds the AudioSource")]
    [SerializeField] private BeatDetector beatDetector;

    [Tooltip("Object to disable")]
    [SerializeField] private GameObject objectToDisable;


    // ─── Public button-callable method ───────────────────────────────────────
    /// <summary>
    /// Opens Windows file explorer filtered to audio files.
    /// Call this from a UI Button onClick.
    /// </summary>
    public void OpenFileBrowser()
    {
        string path = OpenWindowsFileBrowser();

        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("[SongLoader] No file selected.");
            return;
        }

        Debug.Log($"[SongLoader] File selected: {path}");
        StartCoroutine(LoadAudioClip(path));
    }

    // ─── Opens the native Windows dialog and returns selected path ───────────
    private string OpenWindowsFileBrowser()
    {
        OpenFileName ofn = new OpenFileName();

        ofn.lStructSize = Marshal.SizeOf(ofn);
        ofn.lpstrTitle = "Select an Audio File";

        // Filter: shows MP3, WAV, OGG files
        ofn.lpstrFilter = "Audio Files\0*.mp3;*.wav;*.ogg\0All Files\0*.*\0";
        ofn.lpstrFile = new string('\0', 256); // Buffer for the path
        ofn.nMaxFile = ofn.lpstrFile.Length;
        ofn.lpstrInitialDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyMusic);
        ofn.Flags = OFN_FILEMUSTEXIST | OFN_PATHMUSTEXIST | OFN_NOCHANGEDIR;

        // Returns true if user selected a file
        if (GetOpenFileName(ref ofn))
            return ofn.lpstrFile.TrimEnd('\0'); // Clean up null chars

        return null; // User cancelled
    }

    // ─── Loads the audio file from disk into an AudioClip ────────────────────
    private IEnumerator LoadAudioClip(string filePath)
    {
        // Convert file path to a URI Unity can load
        string uri = "file:///" + filePath.Replace("\\", "/");

        // Detect format from extension
        AudioType audioType = GetAudioType(filePath);

        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(uri, audioType))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[SongLoader] Failed to load audio: {request.error}");
                yield break;
            }

            // Get the loaded clip
            AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
            clip.name = System.IO.Path.GetFileNameWithoutExtension(filePath);

            // Pass it to BeatDetector (which owns the AudioSource)
            beatDetector.SetSong(clip);

            // Disable the UI panel after loading
            if (objectToDisable != null && objectToDisable.activeSelf)
                objectToDisable.SetActive(false);
        }
    }

    // ─── Maps file extension to Unity AudioType ───────────────────────────────
    private AudioType GetAudioType(string path)
    {
        string ext = System.IO.Path.GetExtension(path).ToLower();

        return ext switch
        {
            ".mp3" => AudioType.MPEG,
            ".ogg" => AudioType.OGGVORBIS,
            ".wav" => AudioType.WAV,
            _ => AudioType.UNKNOWN
        };
    }
}