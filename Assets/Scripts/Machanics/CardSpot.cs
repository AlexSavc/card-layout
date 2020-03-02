using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpot : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject emptyImage;

    [SerializeField] private Card card;
    public Card Card { get{ return card; } }

    public delegate void CardSpotSelectionDelegate(CardSpot card);
    public event CardSpotSelectionDelegate onCardSpotSelection;

    void Start()
    {
        CardLayout layout = CardLayout.Instance;
        layout.AddCardSpot(this);
        onCardSpotSelection += layout.Select;
    }

    /// <summary>
    /// Change the scale of the card outline to show that the card is selected
    /// </summary>
    /// <param name="scale"></param>
    public void SetImageScale(Vector2 scale)
    {
        emptyImage.transform.localScale = scale;
    }
    
    public virtual void OnMouseDown()
    {
        if (Utility.IsPointerOverUIObject()) { return; }
        onCardSpotSelection?.Invoke(this);
    }

    public virtual void SetCard(Card toSet)
    {
        if(card != null) { Debug.Log("Card not Null"); return; }
        card = toSet;
        card.transform.SetParent(transform);
        card.transform.localPosition = Vector3.zero;
    }

    public virtual void OnInteraction()
    {
        onCardSpotSelection?.Invoke(this);
    }
}