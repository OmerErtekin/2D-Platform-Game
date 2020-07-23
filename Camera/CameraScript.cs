using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;

    }


    void Update()
    {
        transform.position = new Vector3(playerTransform.position.x, transform.position.y, transform.position.z);

    }
}

