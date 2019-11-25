using System;
using UnityEngine;

using System.Collections.Generic;
using Photon.Pun;
using Random = System.Random;

namespace Com.MyCompany.MyGame
{
    /**
     * va servir à definir les comportement de l'avatar du joueur
     */
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {

        //True, when the user is firing
        bool IsFiring;
        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;//instance de l'objet qui appartient au joueur local 
        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        public GameObject PlayerUiPrefab;
        private GameManager game;//GameManager pour connaitre lorsqu'il y a une pause
        public GameObject healthbar;
        public float playerSpeed;
        private int longeur = 15;//taille du vaisseau 
        private int largeur = 10;
        public float health=10;//vie maximale
        private static float MAX_SPEED = 150.0f;
        private static float ACCEL = 100f;
        private static float ROTATION_SPEED = 100.0f;

        private Dictionary<string, KeyCode> controlKeys = new Dictionary<string, KeyCode>();//recuperation des touches predefinies


        void Awake()
        {    
            if (photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;//recuperation du vaisseau representant le joueur local (celui dont la vu nous appartient)
                game = GameObject.Find("Game Manager").GetComponent<GameManager>();
                healthbar = Instantiate(this.healthbar);
                healthbar.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);//creation et lien avec la barre de vie
            }
            //DontDestroyOnLoad(this.gameObject);

            if (tag.Equals("Player"))
            {
                GameObject _uiGo = Instantiate(this.PlayerUiPrefab);//creation et lien avec le UI pour afficher le nom du joueur
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
                
                
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
                foreach (GameObject enemy in enemies) {
                        enemy.SendMessage("syncPlayer"); // s'assure que les bots sont synchronisé avec le joueur
                }
            }

        }
        void Start()
        {
            CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();

            playerSpeed = 0;
            controlKeys.Add("Up1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up1","W")));//recuperation des touche dans le dictionnaire
            controlKeys.Add("Down1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down1","S")));
            controlKeys.Add("Left1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left1","A")));
            controlKeys.Add("Right1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right1","D")));
            controlKeys.Add("Slow1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Slow1","LeftShift")));
            controlKeys.Add("Fire1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Fire1","Space")));
            
            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();//on fait suivre la camera 
                }
            }
        }
    
        void Update()
        {
            if (photonView.IsMine && !game.paused)
            {

                ProcessInputs ();//si le jeu n'est pas en pause et que l'on est sur le joueur que l'on controle on prend en compte les inputs
                checkcollision();//et on test les collision
            }

        }

        
        /**
         * appelé lorsqu'on est touché par un laser , deux joueurs ne peuvent pas se toucher
         */
        public void hit()
        {
            health--;//on perd de la vie
            if (health <= 0)
            {
                game.SendMessage("gameOver");//on dit au jeu qu'on a perdu quand on meut 
                PhotonNetwork.Destroy(this.gameObject);//et on detruit le joueur 
            }
        }

   
        void ProcessInputs()
        {
            
            if (Input.GetKey(controlKeys["Up1"]))//on accelere 
            {
                if (playerSpeed < MAX_SPEED)
                {
                    playerSpeed += ACCEL * Time.deltaTime;
                    
                }
            }
            else if (Input.GetKey(controlKeys["Down1"]))//on ralentit
            {
                if (playerSpeed > -(MAX_SPEED))
                {
                    playerSpeed -= ACCEL * Time.deltaTime;
                    
                }
            }
            else if (Input.GetKey(controlKeys["Slow1"]))//permet de s'arreter 
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
            LocalPlayerInstance.transform.Translate(0, 0, playerSpeed * Time.deltaTime);//mise a jour de la position en coordonée local plus pratique
            if (Input.GetKey(controlKeys["Right1"]))//tourne à droite 
            {
                LocalPlayerInstance.transform.Rotate(0,ROTATION_SPEED * Time.deltaTime,0);
            }
            if (Input.GetKey(controlKeys["Left1"]))//tourne a gauche
            {
                LocalPlayerInstance.transform.Rotate(0,-ROTATION_SPEED * Time.deltaTime,0);
            }

            
            if (Input.GetKeyDown(controlKeys["Fire1"]))//creer un laser devant notre vaiseau et dans notre dirrection
            {
                PhotonNetwork.Instantiate("Laser", transform.position + 20 * transform.forward, transform.rotation);
            }
            
        }

        public void checkcollision()//les colliders ne trouvent pas les collisions donc on les tests manuellement
        {
            GameObject[] lasers= GameObject.FindGameObjectsWithTag("Laser");
            foreach (GameObject obj in lasers)//les lasers etant suffisament petit par rapport aux vaisseaux on peut estimer cela à une collision entre un point et une box
            {
                Laser laser = obj.GetComponent<Laser>();// sert à savoir si le laser n'a pas deja effectuer une collision
                Vector3 poslocal = transform.InverseTransformPoint(obj.transform.position); //on prend la position du laser dans le repere du joueur
                if (!laser.destroy &&poslocal.x < largeur && poslocal.x > -largeur && poslocal.z < longeur && poslocal.z > -longeur)
                {//si le laser n'a pas deja fait une collision et qu'il est dans la box du joueur
                    hit	();//on prend un coup
                    obj.SendMessage	("Destroy");//on demande au laser de se detruire car on peut ne pas avoir les droit de le detruire
                }
            } 

        }

        /**
         * permet de synchroniser la vie des joueurs
         */
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(health);//on envois au autres joueur la vie de notre vaisseau
                
            }
            else
            {
                this.health = (float) stream.ReceiveNext();//et on reçoit celle des leur
            }
        }


        
    }
    
    
}