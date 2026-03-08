using System;
using System.Linq;
using UnityEngine;

public class SongRegistry : MonoBehaviour
{
    public static SongRegistry Instance { get; private set; }

    [Serializable]
    public class SongClipEntry
    {
        public string key;
        public AudioClip clip;
    }

    [SerializeField] private SongClipEntry[] songs;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public AudioClip GetClipByKey(string key)
    {
        if (songs == null || songs.Length == 0 || string.IsNullOrEmpty(key))
            return null;

        SongClipEntry entry = songs.FirstOrDefault(x => x.key == key);
        return entry != null ? entry.clip : null;
    }
}