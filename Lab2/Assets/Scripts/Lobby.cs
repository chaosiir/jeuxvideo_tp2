using System;
using System.Collections; 
using System.Collections.Generic; 
using Photon.Pun; 
using Photon.Realtime; 
using UnityEngine; 
using UnityEngine.SceneManagement; 
using UnityEngine.UI; 
 
public class Lobby : MonoBehaviourPunCallbacks ,IPunObservable
{ 
    public InputField player1; //pour afficher les noms des joueurs dans la partie  
    public InputField player2;  
    public InputField player3; 
    public InputField player4; 
    public GameObject launch;
    public Text start;
    private Player[] players;
    private bool isStarting=false;
    private float tstart=0;
    // Start is called before the first frame update 
    void Start()
    {
        
        launch.SetActive( PhotonNetwork.IsMasterClient); 
        start.enabled = false;
        players = PhotonNetwork.PlayerList; 
        affichejoueurs(); 
    } 
 
    // Update is called once per frame 
    void Update() 
    {

        if (isStarting)
        {
            start.text = (5 - (int)Math.Floor(Time.time - tstart)).ToString();
            if (start.text.Equals("0"))
            {
                start.text = "Go!";
                isStarting = false;
                PhotonNetwork.LoadLevel("Game");
            }
        }
    } 
 
    public override void OnPlayerEnteredRoom(Player newPlayer) 
    {

        base.OnPlayerEnteredRoom(newPlayer); 
        players = PhotonNetwork.PlayerList; 
        affichejoueurs(); 
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        launch.SetActive( PhotonNetwork.IsMasterClient); 
    }

    public override void OnLeftRoom() 
    { 
        PhotonNetwork.LeaveRoom(); 
        SceneManager.LoadScene("Launcher"); 
    } 
    public override void OnPlayerLeftRoom(Player otherPlayer) 
    {
        base.OnPlayerLeftRoom(otherPlayer); 
        players = PhotonNetwork.PlayerList; 
        affichejoueurs(); 
    } 
 
    private void affichejoueurs() 
    { 
        switch (players.Length) 
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

    public void clickLaunch()
    {
        isStarting = true;
        tstart = Time.time;
        start.enabled = true;
        
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting && PhotonNetwork.IsMasterClient)//comme il n'y a que l'hote qui peut clicker sur launch il est les seul a ecrire
        {
            // We own this player: send the others our data
            stream.SendNext(isStarting);
            stream.SendNext(start.enabled);
  
                
        }
        else
        {
            // Network player, receive data
            if(!stream.IsWriting )
            {
                isStarting = (bool)stream.ReceiveNext();
                start.enabled = (bool)stream.ReceiveNext();
                if (isStarting && tstart==0)//des qu'on recois le signal isStarting a vrai on initialise tstart car tout les jeux n'ont
                {//pas des horloges synchros , on ne veut l'initialiser qu'une fois donc pourcela on verifie que tstart=0
                   
                    tstart = Time.time;//on sait comme cela depuis combien de temps on a reçu isStarting=true
                }
            }
            
        }
    }
} 
