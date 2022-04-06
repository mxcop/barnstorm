using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    [SerializeField] private float volume;
    [SerializeField] private SoundLibraryEntry[] audioLibrary;

    private static SFXManager instance;
    private static AudioSource source;

    private Dictionary<string, AudioClip> lib;

    private void Awake()
    {
        if (instance is null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            source = GetComponent<AudioSource>();

            PopulateLibrary();
        }
        else Destroy(gameObject);
    }

    /// <summary>
    /// Populate the audio library dictionary.
    /// </summary>
    private void PopulateLibrary()
    {
        lib = new Dictionary<string, AudioClip>();
        for (int i = 0; i < audioLibrary.Length; i++)
            lib.Add(audioLibrary[i].name, audioLibrary[i].clip);
    }

    /// <summary>
    /// Play an audio clip.
    /// </summary>
    /// <param name="clip">The name of the clip in the audio library.</param>
    public static void PlayClip(string clip)
    {
        if (instance.lib.TryGetValue(clip, out AudioClip audio))
        {
            source.PlayOneShot(audio);
        }
        else Debug.LogError($"Attempted to play non existing audio clip: \"{ clip }\"");
    }

    /// <summary>
    /// Play a music clip on loop.
    /// </summary>
    /// <param name="clip">The name of the clip in the audio library.</param>
    public static void PlayMusic(string clip)
    {
        if (instance.lib.TryGetValue(clip, out AudioClip audio))
        {
            source.Stop();
            LeanTween.value(instance.gameObject, s => source.volume = s, source.volume, 0.0f, 0.5f).setOnComplete(() =>
            {
                source.clip = audio;
                source.loop = true;
                source.Play();
                LeanTween.value(instance.gameObject, s => source.volume = s, source.volume, instance.volume, 0.5f);
            });
        }
        else Debug.LogError($"Attempted to play non existing music clip: \"{ clip }\"");
    }

    [System.Serializable]
    private struct SoundLibraryEntry
    {
        public string name;
        public AudioClip clip;
    }
}
