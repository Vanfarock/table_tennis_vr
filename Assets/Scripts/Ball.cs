using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Ball : MonoBehaviour
{
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            var rb = GetComponent<Rigidbody>();
            Destroy(rb);
        }
    }
}
