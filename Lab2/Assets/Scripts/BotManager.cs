using Com.MyCompany.MyGame.AI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Com.MyCompany.MyGame
{
    public class BotManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        private GameObject LocalPlayerInstance;
        
        //[SerializeField]
        //public GameObject PlayerUiPrefab;
        
        private bool IsFiring;
        private AIBehaviour _aiBehaviour;
        
        void Awake()
        {    
            if (PhotonNetwork.IsMasterClient)
            {
                LocalPlayerInstance = this.gameObject;
            }
            //DontDestroyOnLoad(this.gameObject);

            /*if (tag.Equals("AI"))
            {
                GameObject _uiGo = Instantiate(this.PlayerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }*/
        }

        void Start()
        {
            
        }
        
        void Update()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                
            }
        }

        /// <summary>
        /// Called when a Photon Player got disconnected. We need to load a smaller scene.
        /// </summary>
        /// <param name="other">Other.</param>
        public override void OnPlayerLeftRoom( Player other  )
        {
            //_aiBehaviour.removePlayer(other);
            //TODO destroy + remplacer par ia si vivant
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                
            }
            else
            {
                
            }
        }
    }
}