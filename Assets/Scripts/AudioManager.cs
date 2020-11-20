using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private bool musicOn=true;
    private bool soundOn=true;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public bool MusicStatus()
    { return musicOn; }

    public bool SoundStatus()
    { return soundOn; }
}
