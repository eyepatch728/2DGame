using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioClip[] smallSounds;
    public AudioSource audioSource;

    [Header("Audio Clips")]
    //public AudioClip backgroundMusic; // Assign your background music here
    public AudioClip mainMenuMusic; // Music for main menu
    public AudioClip gameplayMusic;
    public List<AudioClip> sfxClips;  // Assign your sound effects here
    private AudioClip currentContinentMusic;
    private AudioClip pendingMiniGameMusic;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void SetPendingMiniGameMusic(AudioClip miniGameMusic)
    {
        if (miniGameMusic != null)
        {
            // Set the pending mini-game music
            pendingMiniGameMusic = miniGameMusic;

            // Immediately play the mini-game music (without waiting for scene load)
            ForcePlayMiniGameMusic(miniGameMusic);

            Debug.Log($"Mini-game music set and forced play: {miniGameMusic.name}");
        }
    }


    private void ForcePlayMiniGameMusic(AudioClip miniGameMusic)
    {
        if (musicSource == null || miniGameMusic == null) return;

        // Stop any currently playing music
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }

        // Set clip and play
        musicSource.clip = miniGameMusic;
        musicSource.loop = true;
        musicSource.Play();
        Debug.Log($"Forced playing mini-game music: {miniGameMusic.name}");
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene Loaded: {scene.name}");

        // Check if there is any pending mini-game music
        if (pendingMiniGameMusic != null)
        {
            // Play the mini-game music if set
            Debug.Log($"Playing pending mini-game music on scene load: {pendingMiniGameMusic.name}");
            ForcePlayMiniGameMusic(pendingMiniGameMusic);
            return; // Don't load any other music, we already have the mini-game music
        }

        // If no mini-game music is set, play default music for the scene
        if (scene.name == "MainMenu")
        {
            ChangeMusic(mainMenuMusic);
            currentContinentMusic = null;
        }
        else if (scene.name == "RunnerAndPuzzle")
        {
            ChangeMusic(gameplayMusic);
            currentContinentMusic = null;
        }
    }

    public void ChangeToContinentMusic(AudioClip continentMusic)
    {
        if (continentMusic == null) return;

        pendingMiniGameMusic = null; // Clear any pending mini-game music

        // Don't restart the same music
        if (currentContinentMusic == continentMusic && musicSource.isPlaying) return;

        ChangeMusic(continentMusic);
        currentContinentMusic = continentMusic;
        Debug.Log($"Now Playing Continent Music: {continentMusic.name}");
    }
    public void ChangeMusic(AudioClip newMusic)
    {
        if (musicSource == null)
        {
            Debug.LogError("SoundManager: Music source is missing!");
            return;
        }

        // Only stop and change if it's different music
        if (musicSource.clip != newMusic)
        {
            StopMusic();
            musicSource.clip = newMusic;
            musicSource.loop = true;
            musicSource.Play();
            Debug.Log($"Now Playing: {newMusic.name}");
        }
        else if (!musicSource.isPlaying)
        {
            // Same clip but not playing, so just play it
            musicSource.Play();
            Debug.Log($"Resuming: {newMusic.name}");
        }
    }

    // Play background music
    //public void PlayMusic()
    //{
    //    if (musicSource != null && backgroundMusic != null)
    //    {
    //        musicSource.clip = backgroundMusic;
    //        musicSource.loop = true;
    //        musicSource.Play();
    //    }
    //}
    public void ResetMiniGameMusic()
    {
        pendingMiniGameMusic = null;
    }
    // Play a specific SFX by name
    public void PlaySFX(string clipName)
    {
        AudioClip clip = FindClipByName(clipName);
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SoundManager: SFX '{clipName}' not found!");
        }
    }

    // Find a clip in the list by name
    private AudioClip FindClipByName(string clipName)
    {
        foreach (AudioClip clip in sfxClips)
        {
            if (clip.name == clipName)
            {
                return clip;
            }
        }
        return null; // Return null if no match is found
    }

    // Stop background music
    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    // Adjust SFX volume
    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }
    }

    // Adjust music volume
    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }
    public void PlaySingleSound(int index)
    {
        if (index >= 0 && index < smallSounds.Length)
        {
            audioSource.clip = smallSounds[index];
            audioSource.Play();
        }
    }
}
