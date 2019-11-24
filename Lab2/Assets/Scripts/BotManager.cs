using Com.MyCompany.MyGame.AI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Com.MyCompany.MyGame
{
    public class BotManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        private static float MAX_SPEED = 120.0f;
        private static float TRANSLATION_ACCELERATION = 60.0f;
        private static float ROTATION_SPEED = 70.0f;
        
        private GameObject LocalPlayerInstance;
        
        //[SerializeField]
        //public GameObject PlayerUiPrefab;
        
        private float _speed;
        
        private AIBehaviour _aiBehaviour;
        
        void Awake()
        {    
            if (PhotonNetwork.IsMasterClient)
            {
                LocalPlayerInstance = this.gameObject;
                _aiBehaviour = new AIBehaviour(LocalPlayerInstance.transform, true);
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
            var timelapse = Time.deltaTime;
            if (_aiBehaviour._movementTranslationState == MovementTranslationState.FORWARD)
            {
                if (_speed < MAX_SPEED)
                {
                    _speed += TRANSLATION_ACCELERATION * timelapse;
                }
                
            } else if (_aiBehaviour._movementTranslationState == MovementTranslationState.HALF_FORWARD) {
                if (_speed < MAX_SPEED / 2)
                {
                    _speed += TRANSLATION_ACCELERATION * timelapse;
                } else {
                    _speed -= TRANSLATION_ACCELERATION * timelapse;
                }
            } else if (_aiBehaviour._movementTranslationState == MovementTranslationState.SLOW) {
                if (_speed > 0) {
                    _speed -= TRANSLATION_ACCELERATION * timelapse;
                } else {
                    _speed = 0;
                }
            }
            LocalPlayerInstance.transform.Translate(0, 0, _speed * Time.deltaTime);
            if (_aiBehaviour._movementRotationState == MovementRotationState.LEFT) {
                LocalPlayerInstance.transform.Rotate(0,ROTATION_SPEED * Time.deltaTime,0);
            } else if (_aiBehaviour._movementRotationState == MovementRotationState.RIGHT) {
                LocalPlayerInstance.transform.Rotate(0,-ROTATION_SPEED * Time.deltaTime,0);
            }

            if (_aiBehaviour._isFiring) {
                PhotonNetwork.Instantiate("Laser", transform.position + 20 * transform.forward, transform.rotation);
                _aiBehaviour._isFiring = false;
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