using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    //--------------------------------------------------------------
    private void Awake()
    {
        GameObject.Find("manageAudio").GetComponent<ManageEazySoundManager>().playExplosionSound();
    }

    void Start()
    {
        Destroy(this.gameObject, 3.0f);
    }
}