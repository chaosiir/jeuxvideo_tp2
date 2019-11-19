using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    public Text players;

    void Start()
    {
        foreach (Player pl in PhotonNetwork.PlayerList)
        {
            Debug.Log(pl.NickName);

            players.text = players.text + " " + pl.NickName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
