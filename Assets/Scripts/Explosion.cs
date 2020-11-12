using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private AudioSource _explosionSound;

    void Start()
    {
        
        Destroy(this.gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
