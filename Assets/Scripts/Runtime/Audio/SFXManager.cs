using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    #region FIELDS

    public static SFXManager Instance;

    public AudioClip buttonClick;
    public AudioClip pause;
    public AudioClip[] OminousSounds;
    public AudioClip[] EnemySounds;
    public AudioClip[] PlayerSounds;

    private AudioSource audioSource;

    #endregion FIELDS

    #region UNITY METHODS

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    #endregion UNITY METHODS

    #region METHODS

    public void PlaySFX(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void playEnemySound()
    {
        int rand = Random.Range(0, EnemySounds.Length);
        audioSource.PlayOneShot(EnemySounds[rand]);
    }

    public void playPlayerSound()
    {
        int rand = Random.Range(0, PlayerSounds.Length);
        audioSource.PlayOneShot(PlayerSounds[rand]);
    }


    #endregion METHODS
}