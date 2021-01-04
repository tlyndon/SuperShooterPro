using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIn3Seconds : MonoBehaviour
{
    void Start()
    {
        Destroy(this.gameObject, 03f);
    }
}