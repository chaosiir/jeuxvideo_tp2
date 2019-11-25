using Com.MyCompany.MyGame.AI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Com.MyCompany.MyGame
{
    public class BotManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        private static float MAX_SPEED = 120.0f;
        private static float TRANSLATION_ACCELERATION = 30.0f;
        private static float ROTATION_SPEED = 70.0f;
        
        private GameObject LocalPlayerInstance;
        public GameManager game;
        public bool isSharpshooter = true;
        //[SerializeField]
        //public GameObject PlayerUiPrefab;
        
        private bool IsFiring;
        private float _speed;
        private int longeur=11;
        private int largeur=7;
        private int longeurplayer = 15;
        private int largeurplayer = 10;
        public bool destroy=false;
        private AIBehaviour _aiBehaviour;
        
        void Awake()
        {    
            if (PhotonNetwork.IsMasterClient)
            {    
                game = GameObject.Find("Game Manager").GetComponent<GameManager>();
                LocalPlayerInstance = this.gameObject;
                _aiBehaviour = new AIBehaviour(LocalPlayerInstance.transform, isSharpshooter);
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
            if (PhotonNetwork.IsMasterClient && !game.paused)
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
                Laser laser = obj.GetComponent<Laser>();// sert à savoir si le laser n'a pas deja effectuer une collision
                Vector3 poslocal = transform.InverseTransformPoint(obj.transform.position); //on prend la position du laser dans le repere du bot
                if (!laser.destroy&&!destroy&&poslocal.x < largeur && poslocal.x > -largeur && poslocal.z < longeur && poslocal.z > -longeur)
                {
                    destroy = true;
                    hit	();
                    obj.SendMessage	("Destroy");//on detruit le laser

                }
            } 
            GameObject[] players= GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject obj in players)
            {
                Vector3 poslocal = transform.InverseTransformPoint(obj.transform.position+obj.transform.forward*longeurplayer+obj.transform.right*largeurplayer); 
                //on prend la position du coin avant droite joueur dans le repere du bot 
                if (!!destroy&&poslocal.x < largeur && poslocal.x > -largeur && poslocal.z < longeur && poslocal.z > -longeur)
                {
                    destroy = true;
                    hit	();//on detruit le bot
                    obj.SendMessage	("hit");//on envoi au joueur comme quoi il s'est fait toucher
                }
                 poslocal = transform.InverseTransformPoint(obj.transform.position+obj.transform.forward*longeurplayer-obj.transform.right*largeurplayer); 
                //on prend la position du coin avant gauche joueur dans le repere du bot 
                if (!destroy&&poslocal.x < largeur && poslocal.x > -largeur && poslocal.z < longeur && poslocal.z > -longeur)
                {
                    destroy = true;
                    hit	();//on detruit le bot
                    obj.SendMessage	("hit");//on envoi au joueur comme quoi il s'est fait toucher
                }
                 poslocal = transform.InverseTransformPoint(obj.transform.position-obj.transform.forward*longeurplayer+obj.transform.right*largeurplayer); 
                //on prend la position du coin arriere droite joueur dans le repere du bot 
                if (!destroy&&poslocal.x < largeur && poslocal.x > -largeur && poslocal.z < longeur && poslocal.z > -longeur)
                {
                    destroy = true;
                    hit	();//on detruit le bot
                    obj.SendMessage	("hit");//on envoi au joueur comme quoi il s'est fait toucher
                }
                 poslocal = transform.InverseTransformPoint(obj.transform.position-obj.transform.forward*longeurplayer-obj.transform.right*largeurplayer); 
                //on prend la position du coin arriere gauche joueur dans le repere du bot 
                if (!destroy&&poslocal.x < largeur && poslocal.x > -largeur && poslocal.z < longeur && poslocal.z > -longeur)
                {
                    destroy = true;
                    hit	();//on detruit le bot
                    obj.SendMessage	("hit");//on envoi au joueur comme quoi il s'est fait toucher
                }
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
            Debug.Log("Player unsynchronisation");
            
            _aiBehaviour.unsyncPlayer();
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

            if (_aiBehaviour._isFiring)
            {
                _aiBehaviour._isFiring = false;
                PhotonNetwork.Instantiate("Laser", transform.position + 20 * transform.forward, transform.rotation);
            }
            
        }

        public void syncPlayer()
        {
            Debug.Log("Player synchronisation:");
            _aiBehaviour.syncPlayer();
            Debug.Log("Synchronisation completed");
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