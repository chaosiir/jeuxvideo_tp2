using System;
using System.Collections; 
using System.Collections.Generic; 
using Photon.Pun; 
using Photon.Realtime; 
using UnityEngine; 
using UnityEngine.SceneManagement; 
using UnityEngine.UI; 
 
/**
 * script pour la gestion de la scene lobby ou l'on attend les joueurs avant de commencer la parte
 */
public class Lobby : MonoBehaviourPunCallbacks ,IPunObservable
{ 
    public InputField player1; //pour afficher les noms des joueurs dans la partie  
    public InputField player2;  
    public InputField player3; 
    public InputField player4; 
    public GameObject launch;//bouton pour lencer la partie
    public Text start;//pour l'affichage du decors
    private Player[] players;//liste des joueurs present dans le lobby
    private bool isStarting=false;
    private float tstart=0;//temps du debut du decompte
    // Start is called before the first frame update 
    void Start()
    {
        
        launch.SetActive( PhotonNetwork.IsMasterClient); //seul le MasterClient peut lancer la partie
        start.enabled = false;//on affiche pas le decompte
        players = PhotonNetwork.PlayerList; 
        affichejoueurs(); 
    } 
    
    void Update() 
    {

        if (isStarting)//si la partie commence
        {
            start.text = (3 - (int)Math.Floor(Time.time - tstart)).ToString();//la valeur du decompte change avec le temps 
            if (start.text.Equals("0"))//si on arrive à 0
            {
                start.text = "Go!";
                isStarting = false;
                PhotonNetwork.LoadLevel("Game");//on charge la scene du jeu
            }
        }
    } 
    /**
     * fonction appelée lorsqu'un joueur rejoin le lobby
     */
    public override void OnPlayerEnteredRoom(Player newPlayer) 
    {

        base.OnPlayerEnteredRoom(newPlayer); 
        players = PhotonNetwork.PlayerList; //on met a jour la liste des joueurs
        affichejoueurs(); //et on l'affiche
    }
    /**
     * fonction appelée lorsu'on change le MasterClient (lorsque celui-ci quitte le lobby)
     */
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        launch.SetActive( PhotonNetwork.IsMasterClient); //on affiche le bouton pour lancer la partie au nouveau MasterClient
    }
    /**
     * fonction appelée lorsqu'on quitte le lobby
     */
    public override void OnLeftRoom() 
    { 
        PhotonNetwork.LeaveRoom(); 
        SceneManager.LoadScene("Launcher"); //on retourne dans la scene d'acceuil
    } 
    
    /**
   * fonction appelée lorsqu'un joueur quitte le lobby pour les autres joueurs
   */
    public override void OnPlayerLeftRoom(Player otherPlayer) 
    {
        base.OnPlayerLeftRoom(otherPlayer); 
        players = PhotonNetwork.PlayerList; //on met a jour la liste des joueurs
        affichejoueurs(); //et on l'affiche
    } 
 
    /**
     * fonction pour afficher les joeurs 
     */
    private void affichejoueurs() 
    { 
        switch (players.Length) //selon le nombre de joueur on affiche leur nom ou empty dans les cases.
        { 
            case 4: 
                player1.text = players[0].NickName; 
                player2.text = players[1].NickName; 
                player3.text = players[2].NickName; 
                player4.text = players[3].NickName; 
                break; 
            case 3: 
                player1.text = players[0].NickName; 
                player2.text = players[1].NickName; 
                player3.text = players[2].NickName;
                player4.text = "Empty";
                break; 
            case 2: 
                player1.text = players[0].NickName; 
                player2.text = players[1].NickName; 
                player3.text = "Empty";
                player4.text = "Empty";
                break; 
            default: 
                
                player1.text = players[0].NickName; 
                player2.text = "Empty";
                player3.text = "Empty";
                player4.text = "Empty";
                break; 
        } 
    }
    /**
     * fonction appelée lorsque le masterClient clique sur le bouton pour lancer la partie
     */
    public void clickLaunch()
    {
        isStarting = true;//on indique que l'on va commencer la partie
        tstart = Time.time;//on prend le temps d'origine du decompte
        start.enabled = true;//on affiche le decompte
        
    }

    /**
     * fonction qui va etre appelée par les composants de photon pour synchroniser des informations transmises via un stream
     */
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //envois d'information
        if (stream.IsWriting && PhotonNetwork.IsMasterClient)//comme il n'y a que l'hote qui peut clicker sur launch il est les seul a ecrire
        {
            stream.SendNext(isStarting);//on synchronise le fait que la partie commence et qu'on affiche le decompte
            stream.SendNext(start.enabled);
  
                
        }
        else
        {
            // reception d'information
            if(!stream.IsWriting )
            {
                isStarting = (bool)stream.ReceiveNext();//on va ecraser nos variable par celle presente dans le flux donc celle du MasterClient
                start.enabled = (bool)stream.ReceiveNext();
                if (isStarting && tstart==0)//des qu'on recois le signal isStarting a vrai on initialise tstart car tout les jeux n'ont
                {//pas des horloges synchros , on ne veut l'initialiser qu'une fois donc pourcela on verifie que tstart=0
                   
                    tstart = Time.time;//on sait comme cela depuis combien de temps on a reçu isStarting=true
                }
            }
            
        }
    }
} 
