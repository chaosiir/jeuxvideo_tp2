using System;
using System.Collections;
using System.Collections.Generic;
using Com.MyCompany.MyGame;
using Photon.Pun;
using UnityEngine;

public class Laser : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    private int speed=300;
    private int largeur=500;
    private int hauteur=350;
    private BoxCollider hitbox;
    public GameManager game;
    public bool destroy=false;//va servir à savoir quand on doit detruire le laser car comme tout le monde n'as pas le droit d'appeler destroy sur cet objet (il faut etre mastreclient ou
                               //proprietaire de cet objet, on leur fait alors changer ce booleen
    void Start()
    {
        hitbox = GetComponent<BoxCollider>();
        if (photonView.IsMine)
        {
            game = GameObject.Find("Game Manager").GetComponent<GameManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine && !game.paused)
        {
            if (destroy)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
            transform.Translate	(speed*Time.deltaTime*Vector3.forward);
            if (transform.position.x < -largeur || transform.position.x > largeur || transform.position.z < -hauteur ||
                transform.position.z > hauteur)//lorsque le laser sort de l'arene on lui laisse un peut de marge puis on le supprime 
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
        
    }

    public void Destroy()//va servir à detruire le laser 
    {
        destroy = true;
    }

}
