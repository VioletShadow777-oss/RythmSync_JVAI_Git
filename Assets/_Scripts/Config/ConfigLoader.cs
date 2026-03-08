using System.Linq;
using UnityEngine;

public static class ConfigLoader
{
    private static ServerConfig cachedServerConfig;
    private static SongConfig cachedSongConfig;

     [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void DebugListConfigAssets()
    {
        TextAsset[] assets = Resources.LoadAll<TextAsset>("Config");

        Debug.Log($"[Config] Found {assets.Length} TextAsset(s) in Resources/Config");

        foreach (var asset in assets)
        {
            Debug.Log($"[Config] Resource asset: {asset.name}");
        }
    }

    public static ServerConfig LoadServerConfig()
    {
        if (cachedServerConfig != null)
            return cachedServerConfig;

        TextAsset jsonFile = Resources.Load<TextAsset>("Config/server_config");

        if (jsonFile == null)
        {
            Debug.LogWarning("[Config] server_config.json not found. Using defaults.");
            cachedServerConfig = new ServerConfig();
            return cachedServerConfig;
        }

         Debug.Log("[Config] server_config raw: " + jsonFile.text);

        cachedServerConfig = JsonUtility.FromJson<ServerConfig>(jsonFile.text);

        if (cachedServerConfig == null)
        {
            Debug.LogWarning("[Config] Failed to parse server_config.json. Using defaults.");
            cachedServerConfig = new ServerConfig();
        }

        return cachedServerConfig;
    }

    public static SongConfig LoadSongConfig()
    {
        if (cachedSongConfig != null)
            return cachedSongConfig;

        TextAsset jsonFile = Resources.Load<TextAsset>("Config/song_config");

        if (jsonFile == null)
        {
            Debug.LogWarning("[Config] song_config.json not found.");
            cachedSongConfig = new SongConfig { songs = new SongItemConfig[0] };
            return cachedSongConfig;
        }

        Debug.Log("[Config] song_config raw: " + jsonFile.text);
        cachedSongConfig = JsonUtility.FromJson<SongConfig>(jsonFile.text);

        if (cachedSongConfig == null || cachedSongConfig.songs == null)
        {
            Debug.LogWarning("[Config] Failed to parse song_config.json.");
            cachedSongConfig = new SongConfig { songs = new SongItemConfig[0] };
        }

        return cachedSongConfig;
    }

    public static SongItemConfig GetSongById(int songId)
    {
        SongConfig config = LoadSongConfig();
        return config.songs.FirstOrDefault(song => song.id == songId);
    }
}