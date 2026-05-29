using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public enum SoundType
    {
        Tree,
        Stone,
        Dirt,
        Fiber,
        Pickup,
        Craft,
        Build,
        GlowCoreUpgrade,
        UIDrag,
        UIDrop,
        OpenChest,
        CloseChest,
        Consume,
        Walk,

        Music1,
        Music2,
        Music3
    }

    public enum AudioChannel
    {
        Player,
        Environment,
        Music
    }

    [System.Serializable]
    public class Sound
    {
        public SoundType Type;
        public AudioClip Clip;
        [Range(0f, 3f)] public float Volume = 1f;
        public bool Loop = false;
        public float LoopDelay = 0f;
    }

    [Header("Audio Sources")]
    [SerializeField]
    private AudioSource m_playerSource;

    [SerializeField] private AudioSource m_environmentSource;
    [SerializeField] private AudioSource m_musicSource;

    [Header("Sounds")][SerializeField] private Sound[] m_allSounds;

    private Dictionary<SoundType, Sound> m_soundDictionary = new();
    private Dictionary<AudioChannel, Coroutine> m_loopCoroutines = new();

    private void Start()
    {
        StartCoroutine(PlayMusicLoop());
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        foreach (var sound in m_allSounds)
        {
            if (!m_soundDictionary.ContainsKey(sound.Type))
            {
                m_soundDictionary.Add(sound.Type, sound);
            }
        }
    }

    private AudioSource GetSource(AudioChannel channel)
    {
        return channel switch
        {
            AudioChannel.Player => m_playerSource,
            AudioChannel.Environment => m_environmentSource,
            AudioChannel.Music => m_musicSource,
            _ => null
        };
    }

    public IEnumerator PlayMusicLoop()
    {
        SoundType[] songs = { SoundType.Music1, SoundType.Music2, SoundType.Music3 };

        yield return new WaitForSeconds(10f);
        while (true)
        {
            var nextSong = songs[Random.Range(0, songs.Length)];
            Play(nextSong, AudioChannel.Music);

            AudioSource source = GetSource(AudioChannel.Music);
            while (source != null && source.isPlaying)
                yield return null;

            float pauseSeconds = Random.Range(60f, 150f);
            yield return new WaitForSeconds(pauseSeconds);
        }
    }

    public void Play(SoundType type, AudioChannel channel)
    {
        if (!m_soundDictionary.TryGetValue(type, out Sound sound))
        {
            Debug.LogWarning($"Sound type {type} not found!");
            return;
        }

        AudioSource source = GetSource(channel);

        if (source == null)
        {
            Debug.LogWarning($"AudioSource for channel {channel} is missing!");
            return;
        }

        if (m_loopCoroutines.TryGetValue(channel, out var routine))
        {
            if (routine != null)
            {
                StopCoroutine(routine);
            }

            m_loopCoroutines.Remove(channel);
        }

        source.Stop();

        source.clip = sound.Clip;
        source.volume = sound.Volume;

        if (channel == AudioChannel.Music)
        {
            source.Play();
        }
        else
        {
            Coroutine loopRoutine =
                StartCoroutine(PlayLoopRoutine(source, sound));
            if (loopRoutine != null)
            {
                m_loopCoroutines[channel] = loopRoutine;
            }
        }

    }

    public void PlayOneShot(SoundType type, AudioChannel channel)
    {
        if (!m_soundDictionary.TryGetValue(type, out Sound sound))
        {
            Debug.LogWarning($"Sound type {type} not found!");
            return;
        }

        AudioSource source = GetSource(channel);

        if (source == null)
        {
            Debug.LogWarning($"AudioSource for channel {channel} is missing!");
            return;
        }

        source.PlayOneShot(sound.Clip, sound.Volume);
    }

    private System.Collections.IEnumerator PlayLoopRoutine(AudioSource source, Sound sound)
    {
        while (true)
        {
            source.Play();

            yield return new WaitWhile(() => source.isPlaying);

            if (sound.LoopDelay > 0f)
            {
                yield return new WaitForSeconds(sound.LoopDelay);
            }
        }
    }

    public void Stop(AudioChannel channel)
    {
        AudioSource source = GetSource(channel);
        if (source == null)
            return;
        source.Stop();

        if (m_loopCoroutines.TryGetValue(channel, out var routine))
        {
            if (routine != null)
            {
                StopCoroutine(routine);
            }

            m_loopCoroutines.Remove(channel);
        }
    }

    public bool IsPlaying(AudioChannel channel)
    {
        AudioSource source = GetSource(channel);

        if (source == null)
        {
            return false;
        }

        return source.isPlaying;
    }

    public void SetVolume(AudioChannel channel, float volume)
    {
        AudioSource source = GetSource(channel);

        if (source == null)
        {
            return;
        }

        source.volume = Mathf.Clamp01(volume);
    }
}