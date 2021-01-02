using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource audioSrc;

    [SerializeField]
    AudioClip[] musicArray;

    [SerializeField]
    AudioClip[] soundsArray;
    //--------------------------------------------------------------
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    //--------------------------------------------------------------
    void Start()
    {
        AudioSource audioSrc = GetComponent<AudioSource>();
    }
    //--------------------------------------------------------------
    public void PlayMusic(string music, float volume)
    {
        if (V.musicOn == true)
        {
            switch (music)
            {
                case "backgroundMusic":
                    audioSrc.PlayOneShot(musicArray[0], volume);
                    break;
                default:
                    V.zprint("error", "There is no music with the given name:" + music);
                    break;
            }
        }
    }
    //--------------------------------------------------------------
    public void PlaySound(string sound, float volume)
    {
        if (V.soundOn == true)
        {
            switch (sound)
            {
                case "explosion":
                    audioSrc.PlayOneShot(soundsArray[0], volume);
                    break;
                case "buzz":
                    audioSrc.PlayOneShot(soundsArray[1], volume);
                    break;
                case "laser":
                    audioSrc.PlayOneShot(soundsArray[2], volume);
                    break;
                case "powerup":
                    audioSrc.PlayOneShot(soundsArray[3], volume);
                    break;
                default:
                    V.zprint("error", "There is no sound with the given name:" + sound);
                    break;
            }
        }
    }
}