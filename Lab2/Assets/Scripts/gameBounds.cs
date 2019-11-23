using System.Collections;
using System.Collections.Generic;
using Com.MyCompany.MyGame;
using UnityEngine;

public class gameBounds : MonoBehaviour
{
    // Start is called before the first frame update
    private bool haswarp=false;
    private int largeur=450;
    private int hauteur=300;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < -largeur)//lorsqu'on touche un bord on est teleporté de l'autre cote de l'arene
        {
            transform.Translate(2*largeur-10,0,0, Space.World);
            haswarp = true;
        }
        if (transform.position.x >largeur)
        {
            transform.Translate(-2*largeur + 10, 0,0, Space.World);
            haswarp = true;
        }
        if (transform.position.z < -hauteur)
        {
            transform.Translate(0,0,2*hauteur-10 , Space.World);
            haswarp = true;
        }
        if (transform.position.z > hauteur)
        {
            transform.Translate(0,0,-2*hauteur+10, Space.World);
            haswarp = true;
        }
        
    }
}
