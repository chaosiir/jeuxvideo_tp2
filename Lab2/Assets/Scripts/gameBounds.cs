using System.Collections;
using System.Collections.Generic;
using Com.MyCompany.MyGame;
using UnityEngine;
/**
 * va servir à faire boucler les objets sur l'arene
 */
public class gameBounds : MonoBehaviour
{
    private int largeur=450;//taille de l'arene
    private int hauteur=300;
    void Start()
    {
    }

    void Update()
    {
        if (transform.position.x < -largeur)//lorsqu'on touche un bord on est teleporté de l'autre cote de l'arene
        {
            transform.position=new Vector3(largeur - 10, 0,transform.position.z);
        }
        if (transform.position.x >largeur)
        {
            transform.position=new Vector3(-largeur + 10, 0,transform.position.z);
        }
        if (transform.position.z < -hauteur)
        {
            transform.position=new Vector3(transform.position.x, 0,hauteur-10);
        }
        if (transform.position.z > hauteur)
        {
            transform.position=new Vector3(transform.position.x, 0,-hauteur+10);
        }
        
    }
}
