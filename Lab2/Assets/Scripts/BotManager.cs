using Com.MyCompany.MyGame.AI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Com.MyCompany.MyGame
{
    public class BotManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        public bool isSharpshooter = true;
        
        private static float MAX_SPEED = 120.0f;
        private static float TRANSLATION_ACCELERATION = 30.0f;
        private static float ROTATION_SPEED = 70.0f;
        
        private GameObject LocalPlayerInstance;
        
        //[SerializeField]
        //public GameObject PlayerUiPrefab;
        
        private bool IsFiring;
        private float _speed;
        private int longeur=11;
        private int largeur=7;
        
        private AIBehaviour _aiBehaviour;
        
        void Awake()
        {    
            if (PhotonNetwork.IsMasterClient)
            {
                LocalPlayerInstance = this.gameObject;
<<<<<<< Updated upstream
                _aiBehaviour = new AIBehaviour(LocalPlayerInstance.transform, isSharpshooter);
=======
                _aiBehaviour = new AIBehaviour(LocalPlayerInstance.transform,true);
>>>>>>> Stashed changes
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
                checkcollision();
            }
        }

        public void checkcollision()//les colliders ne trouvent pas les collisions donc on les tests manuellement
        {
            GameObject[] lasers= GameObject.FindGameObjectsWithTag("Laser");
            foreach (GameObject obj in lasers)
            {
                Vector3 poslocal = transform.InverseTransformPoint(obj.transform.position); //on prend la position du laser dans le repere du bot
                if (poslocal.x < largeur && poslocal.x > -largeur && poslocal.z < longeur && poslocal.z > -longeur)
                {
                    hit	();
                    obj.SendMessage	("Destroy");//on demande au laser de se detruire car on peut ne pas avoir les droit de le detruire
                }
            } 
        }
        public void OnCollisionEnter(Collision other)
        {
            Debug.Log("hit");
            
            hit();
            if (photonView.IsMine && other.gameObject.tag.Equals("laser"))//si on se fait toucher par un laser
            {
                PhotonNetwork.Destroy(other.gameObject);//on detruit le laser
                
            }
        }

        public void hit()
        {
            PhotonNetwork.Destroy(this.gameObject);
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