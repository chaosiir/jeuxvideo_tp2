// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Launcher.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in "PUN Basic tutorial" to handle typical game management requirements
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using Com.MyCompany.MyGame;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using UnityEngine.UI;
using Random = UnityEngine.Random;


/// <summary>
/// Game manager.
/// Connects and watch Photon Status, Instantiate Player
/// Deals with quiting the room and the game
/// Deals with level loading (outside the in room synchronization)
/// </summary>
public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{



	static public GameManager Instance;


	private GameObject instance;

	[Tooltip("The prefab to use for representing the player")] [SerializeField]
	private GameObject playerPrefab;

	[SerializeField] private GameObject iaPrefab;

	public GameObject pausepanel;
	public GameObject gopanel;
	public Text vague;
	public bool paused = false;
	public int nbvague = 0;



	/// <summary>
	/// MonoBehaviour method called on GameObject by Unity during initialization phase.
	/// </summary>
	void Start()
	{
		Instance = this;

		if (playerPrefab == null)
		{
			// #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.

			Debug.LogError(
				"<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",
				this);
		}
		else
		{


			if (PlayerManager.LocalPlayerInstance == null)
			{
				Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

				PhotonNetwork.Instantiate(playerPrefab.name,
					new Vector3(Random.Range(-400, 400), 0, Random.Range(-300, 300)), Quaternion.identity);
				// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate

			}
			else
			{

				Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
			}


		}



	}


	/// <summary>
	/// MonoBehaviour method called on GameObject by Unity on every frame.
	/// </summary>
	void Update()
	{
		pausepanel.SetActive(paused);
		vague.text = "Wave " + nbvague;
		// "back" button of phone equals "Escape". quit app if that's pressed
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (paused)
			{
				photonView.RPC("Continue", RpcTarget.AllBuffered);
			}
			else
			{
				photonView.RPC("Pause", RpcTarget.AllBuffered);
			}
		}

		if (PhotonNetwork.IsMasterClient)
		{
			if (GameObject.FindGameObjectsWithTag("enemy").Length == 0
			) //quand il n'y a plus d'ennemis on passe à la vague suivante
			{
				spawnVague();
			}

		}
	}



	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		if (PhotonNetwork.IsMasterClient)
		{


			nbvague--;
			GameObject[] enemys = GameObject.FindGameObjectsWithTag("enemy");
			foreach (GameObject enemy in enemys)
			{
				PhotonNetwork.Destroy(enemy);
			}

			GameObject[] lasers = GameObject.FindGameObjectsWithTag("Laser");
			foreach (GameObject laser in enemys)
			{
				PhotonNetwork.Destroy(laser);
			}

			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			foreach (GameObject player in players)
			{
				if (!player.GetPhotonView().IsOwnerActive)
				{
					PhotonNetwork.Destroy(player);
				}

			}
		}
	}
	

	/// <summary>
	/// Called when the local player left the room. We need to load the launcher scene.
	/// </summary>
	public override void OnLeftRoom()
	{
		SceneManager.LoadScene("Launcher");
	}



	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
	}

	public void QuitApplication()
	{

		Application.Quit();
	}

	[PunRPC] //can be call by other client with message
	public void Pause()
	{

		paused = true;
	}

	[PunRPC]
	public void Continue()
	{
		paused = false;
	}

	private void gameOver()
	{
		gopanel.SetActive(true);
	}

	private void spawnVague()
	{
		nbvague++;

		Debug.Log("vague" + nbvague);
		int nbenemy = (int) ((Math.Pow(nbvague, 1.5) + nbvague) / 2);
		for (int i = 0; i < nbenemy; i++)
		{
			PhotonNetwork.Instantiate(iaPrefab.name, new Vector3(Random.Range(-400, 400), 0, Random.Range(-300, 300)),
				Quaternion.identity);
		}
	}
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.CloseConnection(newPlayer);//on empleche les nouveaux joueurs de rejoindre en cours de partie 
		}
		
	}
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting && PhotonNetwork.IsMasterClient)
		{
			stream.SendNext(nbvague);
		}
		else if (stream.IsReading && !PhotonNetwork.IsMasterClient)
		{
			nbvague = (int) stream.ReceiveNext();
		}
	}
}