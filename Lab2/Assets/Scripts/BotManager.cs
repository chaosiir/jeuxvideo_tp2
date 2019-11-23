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
                _aiBehaviour = new AIBehaviour(LocalPlayerInstance.transform);
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
                _aiBehaviour.update();
                ProcessInputs();
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

        void ProcessInputs()
        {
            if (_aiBehaviour._movementTranslationState == MovementTranslationState.FORWARD)
            {
                LocalPlayerInstance.transform.Translate(Vector3.forward);
            } else if (_aiBehaviour._movementTranslationState == MovementTranslationState.HALF_FORWARD) {
                LocalPlayerInstance.transform.Translate(Vector3.forward * 0.5f);
            }

            if (_aiBehaviour._movementRotationState == MovementRotationState.RGHT) {
                LocalPlayerInstance.transform.Rotate(0,2,0);
            } else if (_aiBehaviour._movementRotationState == MovementRotationState.LEFT) {
                LocalPlayerInstance.transform.Rotate(0,-2,0);
            }
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