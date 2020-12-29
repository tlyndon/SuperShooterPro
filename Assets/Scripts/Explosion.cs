using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private AudioClip explosionSound;
    //--------------------------------------------------------------
    void Start()
    {
        if (V.soundOn == true)
        {
            GetComponent<AudioSource>().PlayOneShot(explosionSound, 0.7f);
        }
        Destroy(this.gameObject, 3.0f);
    }
}