using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Laser : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    private int speed=300;
    private int largeur=500;
    private int hauteur=350;
    private BoxCollider hitbox;
    void Start()
    {
        hitbox = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            transform.Translate	(speed*Time.deltaTime*Vector3.forward);
            if (transform.position.x < -largeur || transform.position.x > largeur || transform.position.z < -hauteur ||
                transform.position.z > hauteur)//lorsque le laser sort de l'arene on lui laisse un peut de marge puis on le supprime 
            {
               PhotonNetwork.Destroy(this.gameObject);
            }
        }
        
    }


}
