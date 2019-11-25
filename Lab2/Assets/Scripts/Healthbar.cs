using System.Collections;
using System.Collections.Generic;
using Com.MyCompany.MyGame;
using UnityEngine;
using UnityEngine.UI;
/**
 * va servir à afficher la barre de vie du joueur en bas a gauche dans le jeu
 */
public class Healthbar : MonoBehaviour
{
    private PlayerManager target;
    [Tooltip("UI Slider to display Player's Health")]
    [SerializeField]
    private Slider playerHealthSlider;
    void Awake()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);//on le place dans le canvas
    }
    /**
     * appelé par l'objet joueur pour les reliés 
     */
    public void SetTarget(PlayerManager _target)
    {
        if (_target == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }
        target = _target;//recuperation du Player manager pour obtenir la variable health
    }
    public void Update()
    {
        playerHealthSlider.value = target.health;//la valeur du slider est egale à celle du joueur
    }
}
