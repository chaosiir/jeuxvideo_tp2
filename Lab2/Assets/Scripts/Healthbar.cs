using System.Collections;
using System.Collections.Generic;
using Com.MyCompany.MyGame;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerManager target;
    [Tooltip("UI Slider to display Player's Health")]
    [SerializeField]
    private Slider playerHealthSlider;
    void Awake()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }
    
    public void SetTarget(PlayerManager _target)
    {
        //Debug.Log("Healthbar called");
        if (_target == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }
        // Cache references for efficiency
        target = _target;
    }
    // Update is called once per frame
    public void Update()
    {
        //Debug.Log("Update healthbar called");

        playerHealthSlider.value = target.health;
    }
}
