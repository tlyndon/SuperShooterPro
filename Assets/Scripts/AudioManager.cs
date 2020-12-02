using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip backgroundMusic;
    //--------------------------------------------------------------
    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (V.musicOn == true)
        {
            audioSource.Stop();
            audioSource.loop = true;
            audioSource.clip = backgroundMusic;
            audioSource.volume = 0.7f;
            audioSource.Play();
        }
    }
}