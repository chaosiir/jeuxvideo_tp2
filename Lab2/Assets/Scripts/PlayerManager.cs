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
        #region Private Fields
        

        //True, when the user is firing
        bool IsFiring;
        #endregion
        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;
        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        public GameObject PlayerUiPrefab;
        
        public float playerSpeed;
        private static float MAX_SPEED = 150.0f;
        private static float ACCEL = 50.0f;

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
            }

        }

        
        
        public void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag.Equals("laser"))//si on se fait toucher par un laser
            {
                PhotonNetwork.Destroy(other.gameObject);//on detruit le laser
                this.hit();
            }
        }

        public void hit()
        {
            Debug.Log("hit");
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
                if (playerSpeed > -(MAX_SPEED))
                {
                    playerSpeed -= ACCEL * Time.deltaTime;
                    
                }
            }
            LocalPlayerInstance.transform.Translate(0, 0, playerSpeed * Time.deltaTime);
            if (Input.GetKey(controlKeys["Right1"]))
            {
                LocalPlayerInstance.transform.Rotate(0,2,0);
            }
            if (Input.GetKey(controlKeys["Left1"]))
            {
                LocalPlayerInstance.transform.Rotate(0,-2,0);
            }
            
            
            if (Input.GetKeyDown(controlKeys["Fire1"]))
            {
                PhotonNetwork.Instantiate("Laser", transform.position + 20 * transform.forward, transform.rotation);
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