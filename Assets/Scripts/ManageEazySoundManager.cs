using UnityEngine;
using System.Collections;
using Hellmade.Sound;

public class ManageEazySoundManager : MonoBehaviour
{

    [SerializeField]
    private AudioClip musicAudioClip;
    [SerializeField]
    private AudioClip explosionClip;
    [SerializeField]
    private AudioClip laserClip;
    [SerializeField]
    private AudioClip powerupClip;
    [SerializeField]
    private AudioClip buzzClip;

    //--------------------------------------------------------------
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
    //--------------------------------------------------------------
    private void Start()
    {
        int backgroundMusicID = EazySoundManager.PlayMusic(musicAudioClip, 0.7f, true, false, 1, 1);
    }
    //--------------------------------------------------------------
    public void playExplosionSound()
    {
        // use transform instead of null if 3D sound
        int soundID = EazySoundManager.PlaySound(explosionClip, 1f, false, null);
    }
    //--------------------------------------------------------------
    public void playLaserSound()
    {
        // use transform instead of null if 3D sound
        int soundID = EazySoundManager.PlaySound(laserClip, 1f, false, null);
    }
    //--------------------------------------------------------------
    public void playPowerupSound()
    {
        // use transform instead of null if 3D sound
        int soundID = EazySoundManager.PlaySound(powerupClip, 1f, false, null);
    }
    //--------------------------------------------------------------
    public void playBuzzSound()
    {
        // use transform instead of null if 3D sound
        int soundID = EazySoundManager.PlaySound(buzzClip, 1f, false, null);
    }
}