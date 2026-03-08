using System;
using UnityEngine;

[Serializable]
public class ServerConfig
{
    public string host = "127.0.0.1";
    public int port = 8080;
    public bool autoConnect = true;
}

[Serializable]
public class SongItemConfig
{
    public int id;
    public string key;
    public string displayName;
}

[Serializable]
public class SongConfig
{
    public SongItemConfig[] songs;
}