
using Photon.Pun;
using UnityEngine;
/**
 * Script servant à gerer les tirs des vaisseaux 
 */
public class Laser : MonoBehaviourPunCallbacks
{
    private int speed=300;
    private int largeur=500;//dimensions de l'arene avec de la marge pour les supprimers une fois sortis de celle-c
    private int hauteur=350;
    public GameManager game;//recuperation du GameManager poour savoir lorsqu'on est en pause
    public bool destroy=false;//va servir à savoir quand on doit detruire le laser car comme tout le monde n'as pas le droit d'appeler destroy sur cet objet (il faut etre mastreclient ou
                               //proprietaire de cet objet, on leur fait alors changer ce booleen
    void Start()
    {
        if (photonView.IsMine)
        {
            game = GameObject.Find("Game Manager").GetComponent<GameManager>();//recuperation du GameManager du proprietaire (avec photonView.isMine)
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine && !game.paused)//seul le proprietaire du laser peut le deplacer et on test si le jeu n'est pas en pause
        {
            if (destroy)
            {
                PhotonNetwork.Destroy(this.gameObject);//on le detruit si on doit
            }
            transform.Translate	(speed*Time.deltaTime*Vector3.forward);
            if (transform.position.x < -largeur || transform.position.x > largeur || transform.position.z < -hauteur ||
                transform.position.z > hauteur)//lorsque le laser sort de l'arene on lui laisse un peut de marge puis on le supprime 
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
        
    }
    /**
     * va servir à savoir quand on doit detruire le laser (appelée par les autres objets )
     */
    public void Destroy()
    {
        destroy = true;
    }

}
