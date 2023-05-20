using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] private Transform playerRef;

    private void Update()
    {
        transform.position = new Vector3(playerRef.position.x, transform.position.y, playerRef.position.z);
    }
}
