using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    #region FIELDS

    public AudioClip mainMenuMusic;
    public AudioClip gameSceneMusic;
    private AudioSource audioSource;
    public AudioClip deathMusic;
    public AudioClip WinMusic;

    #endregion FIELDS

    #region UNITY METHODS

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    #endregion UNITY METHODS

    #region METHODS

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenu":
                PlayMusic(mainMenuMusic);
                break;

            case "Game":
                PlayMusic(gameSceneMusic);
                break;

            case "Death":
                PlayMusic(deathMusic);
                break;

            case "Victory":
                PlayMusic(WinMusic);
                break;

            default:
                break;
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion METHODS
}