using System;
using UnityEngine;
using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

namespace Com.MyCompany.MyGame
{
    /// <summary>
    /// Player manager.
    /// Handles fire Input and Beams.
    /// </summary>
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {

        //True, when the user is firing
        bool IsFiring;
        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;
        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        public GameObject PlayerUiPrefab;

        public GameObject healthbar;
        private float playerSpeed;
        private int longeur = 15;
        private int largeur = 10;
        public float health=10;
        private static float MAX_SPEED = 150.0f;
        private static float ACCEL = 80.0f;
        private static float ROTATION_SPEED = 100.0f;

        private Dictionary<string, KeyCode> controlKeys = new Dictionary<string, KeyCode>();

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {    
            if (photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }
            //DontDestroyOnLoad(this.gameObject);

            if (tag.Equals("Player"))
            {
                GameObject _uiGo = Instantiate(this.PlayerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
                Instantiate(healthbar).SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }

        }
        void Start()
        {
            CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();

            playerSpeed = 0;
            controlKeys.Add("Up1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up1","W")));
            controlKeys.Add("Down1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down1","S")));
            controlKeys.Add("Left1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left1","A")));
            controlKeys.Add("Right1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right1","D")));
            controlKeys.Add("Slow1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Slow1","LeftShift")));
            controlKeys.Add("Fire1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Fire1","Space")));
            
            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
        }
        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity on every frame.
        /// </summary>
        void Update()
        {
            if (photonView.IsMine)
            {
                ProcessInputs ();
                checkcollision();
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
            health--;
            
            healthbar.SendMessage("Update_health", this, SendMessageOptions.RequireReceiver);
        }

        /// <summary>
        /// Processes the inputs. Maintain a flag representing when the user is pressing Fire.
        /// </summary>
        void ProcessInputs()
        {
            
            if (Input.GetKey(controlKeys["Up1"]))
            {
                if (playerSpeed < MAX_SPEED)
                {
                    playerSpeed += ACCEL * Time.deltaTime;
                    
                }
            }
            else if (Input.GetKey(controlKeys["Down1"]))
            {
                if (playerSpeed > -(MAX_SPEED * 0.6))
                {
                    playerSpeed -= ACCEL * Time.deltaTime;
                    
                }
            }
            else if (Input.GetKey(controlKeys["Slow1"]))
            {
                if (playerSpeed > 0)
                {
                    playerSpeed = Math.Max(playerSpeed - ACCEL * Time.deltaTime, 0);
                }
                else if (playerSpeed < 0)
                {
                    playerSpeed = Math.Min(playerSpeed + ACCEL * Time.deltaTime, 0);
                }
            }
            LocalPlayerInstance.transform.Translate(0, 0, playerSpeed * Time.deltaTime);
            if (Input.GetKey(controlKeys["Right1"]))
            {
                LocalPlayerInstance.transform.Rotate(0,ROTATION_SPEED * Time.deltaTime,0);
            }
            if (Input.GetKey(controlKeys["Left1"]))
            {
                LocalPlayerInstance.transform.Rotate(0,-ROTATION_SPEED * Time.deltaTime,0);
            }

            
            if (Input.GetKeyDown(controlKeys["Fire1"]))
            {
                PhotonNetwork.Instantiate("Laser", transform.position + 20 * transform.forward, transform.rotation);
            }
            
        }

        public void checkcollision()//les colliders ne trouvent pas les collisions donc on les tests manuellement
        {
            GameObject[] lasers= GameObject.FindGameObjectsWithTag("Laser");
            foreach (GameObject obj in lasers)
            {
                Vector3 poslocal = transform.InverseTransformPoint(obj.transform.position); //on prend la position du laser dans le repere du joueur
                if (poslocal.x < largeur && poslocal.x > -largeur && poslocal.z < longeur && poslocal.z > -longeur)
                {
                    hit	();
                    PhotonNetwork.Destroy(obj);
                }
            } 
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(health);
                
            }
            else
            {
                this.health = (float) stream.ReceiveNext();
            }
        }


        
    }
    
    
}