using UnityEngine;
using System.Collections;
using Hellmade.Sound;

public class ManageEasySoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip musicAudioClip;
    //--------------------------------------------------------------
    private void Start()
    {
        int backgroundMusicID = EazySoundManager.PlayMusic(musicAudioClip, 0.7f, true, false, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
