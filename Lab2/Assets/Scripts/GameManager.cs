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
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Com.MyCompany.MyGame
{
	#pragma warning disable 649

	/// <summary>
	/// Game manager.
	/// Connects and watch Photon Status, Instantiate Player
	/// Deals with quiting the room and the game
	/// Deals with level loading (outside the in room synchronization)
	/// </summary>
	public class GameManager : MonoBehaviourPunCallbacks,IPunObservable
    {


		
		static public GameManager Instance;


		private GameObject instance;

        [Tooltip("The prefab to use for representing the player")]
        [SerializeField]
        private GameObject playerPrefab;
        [SerializeField]
        private GameObject iaPrefab;

        public GameObject pausepanel;
        public GameObject gopanel;
        public Text vague;
        public bool paused=false;
        public int nbvague=0;
        

        
        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
	        Instance = this;

			if (playerPrefab == null) { // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.

				Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
			} else {


				if (PlayerManager.LocalPlayerInstance==null)
				{
					Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
				
					PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(Random.Range(-400,400),0,Random.Range(-300,300)), Quaternion.identity);
					// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
					
				}else{
					
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
				Debug.Log(photonView.Owner);
				if (paused )
				{
					photonView.RPC("Continue", RpcTarget.All);
				}
				else
				{
					photonView.RPC("Pause", RpcTarget.All, photonView.Owner);
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

        /// <summary>
        /// Called when a Photon Player got connected. We need to then load a bigger scene.
        /// </summary>
        /// <param name="other">Other.</param>
        public override void OnPlayerEnteredRoom( Player other  )
		{
			//TODO kick or transfert the player
		}

		/// <summary>
		/// Called when a Photon Player got disconnected. We need to load a smaller scene.
		/// </summary>
		/// <param name="other">Other.</param>
		public override void OnPlayerLeftRoom( Player other  )
		{
			//TODO destroy player +transfer si master
			// todo doc + rapport agent , flot et ia 
		}

		/// <summary>
		/// Called when the local player left the room. We need to load the launcher scene.
		/// </summary>
		public override void OnLeftRoom()
		{
			SceneManager.LoadScene("Launcher");
		}

		public IEnumerator  TransfertOwnership()
		{
			if (PhotonNetwork.PlayerList.Length > 0 )//pas besoin de transfert si on est le dernier joeur
			{
				Player master = PhotonNetwork.PlayerList[1];//le master est forcement le 1er de la liste 
				if (PhotonNetwork.SetMasterClient(master)) //si le transfert marche 
				{
					GameObject[] lasers = GameObject.FindGameObjectsWithTag("Laser");
					GameObject[] enemys = GameObject.FindGameObjectsWithTag("enemy");
					foreach (GameObject laser in lasers)
					{
						if (laser.GetPhotonView().Owner.UserId==photonView.Owner.UserId)//si le laser appartient a l'ancien masterClient on le transfert 
						{
							laser.GetPhotonView().TransferOwnership(master);
						}
					}
					foreach (GameObject enemy in enemys)
					{
						enemy.GetPhotonView().TransferOwnership(master);//les enemies appartiennent obligatoirement au master	
					}
					Debug.Log("wait");
					yield return new WaitForSeconds(1);
				}
				
			}
			
		}

		public void LeaveRoom()
		{
			
			if (PhotonNetwork.IsMasterClient)
			{
				TransfertOwnership();
			}
			PhotonNetwork.LeaveRoom();
		}

		public void QuitApplication()
		{
			if (PhotonNetwork.IsMasterClient)
			{
				TransfertOwnership();
			}
			
			Application.Quit();
		}
		[PunRPC]//can be call by other client with message
		public void Pause(Player player)
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
			
			Debug.Log("vague"+nbvague);
			int nbenemy = (int) ((Math.Pow(nbvague, 1.5) + nbvague) / 2);
			for (int i = 0; i < nbenemy; i++)
			{
				PhotonNetwork.Instantiate(iaPrefab.name, new Vector3(Random.Range(-400, 400), 0, Random.Range(-300, 300)),
					Quaternion.identity);
			}
		}
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if(stream.IsWriting && PhotonNetwork.IsMasterClient)
			{
				stream.SendNext(nbvague);
			}
			else if (stream.IsReading && !PhotonNetwork.IsMasterClient)
			{
				nbvague = (int) stream.ReceiveNext();
			}
		}
    }

}