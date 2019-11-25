using UnityEngine;
using UnityEngine.UI;

using Com.MyCompany.MyGame;

/**
 * va servir à afficher le nom des joueurs au dessus de leur vaisseu
 * aussi repris partiellement du tutoriel de photon
 */

public class PlayerUI : MonoBehaviour
{
    private PlayerManager target;//joueur sur lequel va s'appliquer cet UI

    [Tooltip("Pixel offset from the player target")] [SerializeField]
    private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

    [Tooltip("UI Text to display Player's Name")] [SerializeField]
    private Text playerNameText;

    Transform targetTransform;
    Renderer targetRenderer;
    CanvasGroup _canvasGroup;
    Vector3 targetPosition;

    /**
     * va etre appeleé par l'objet joueur_humain a la creation de l'UI pour les reliés
     */

    public void SetTarget(PlayerManager _target)
    {
        if (_target == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }

        // Cache references for efficiency
        target = _target;
        targetTransform = this.target.GetComponent<Transform>();
        targetRenderer = this.target.GetComponent<Renderer>();

        if (playerNameText != null)
        {
            playerNameText.text = target.photonView.Owner.NickName;//recuperation du nom du joueur
        }
    }

    void Awake()
    {
        _canvasGroup = this.GetComponent<CanvasGroup>();
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }
    
    void Update()
    {
        if (target == null)//destruction automatique de l'UI des que celle-ci n'a plus de target (donc que le joueur a ete detruit)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    void LateUpdate()
    {
// Do not show the UI if we are not visible to the camera, thus avoid potential bugs with seeing the UI, but not the player itself.
        if (targetRenderer != null)
        {
            this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
        }


// #Critical
// Follow the Target GameObject on screen.
        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
            this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
        }
    }


}
