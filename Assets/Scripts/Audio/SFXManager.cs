using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    [Range(0.0f, 1.0f), SerializeField] float volume;
    [SerializeField] private SoundLibraryEntry[] audioLibrary;

    private static SFXManager instance;
    private static AudioSource source;

    private Dictionary<string, SoundLibraryEntry> lib = new Dictionary<string, SoundLibraryEntry>();

    private void Awake()
    {
        instance = this;
        source = GetComponent<AudioSource>();

        PopulateLibrary();
    }

    /// <summary>
    /// Populate the audio library dictionary.
    /// </summary>
    private void PopulateLibrary()
    {
        for (int i = 0; i < audioLibrary.Length; i++)
            lib.Add(audioLibrary[i].name, audioLibrary[i]);
    }

    /// <summary>
    /// Play an audio clip.
    /// </summary>
    /// <param name="clip">The name of the clip in the audio library.</param>
    public static void PlayClip(string clip)
    {
        if (instance.lib.TryGetValue(clip, out SoundLibraryEntry entry))
        {
            source.PlayOneShot(entry.clip, entry.volume);
        }
        else Debug.LogError($"Attempted to play non existing audio clip: \"{ clip }\"");
    }

    /// <summary>
    /// Play a music clip on loop.
    /// </summary>
    /// <param name="clip">The name of the clip in the audio library.</param>
    public static void PlayMusic(string clip)
    {
        if (instance.lib.TryGetValue(clip, out SoundLibraryEntry entry))
        {
            source.Stop();
            LeanTween.value(instance.gameObject, s => source.volume = s, source.volume, 0.0f, 0.5f).setOnComplete(() =>
            {
                source.clip = entry.clip;
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
        [Range(0, 1)] public float volume;
        public AudioClip clip;
    }
}
