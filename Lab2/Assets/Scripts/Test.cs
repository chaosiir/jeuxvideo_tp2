using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public Text players;

    void Start()
    {
        PhotonNetwork.Instantiate("Lobby", Vector3.zero, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
