using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLayout : MonoBehaviour
{
    private static CardLayout _instance;
    public static CardLayout Instance { get { return _instance; } }

    [Header("Card Spot")]
    [SerializeField] Vector2 normalSize, selectedSize;

    private List<CardSpot> cardSpots;
    [SerializeField]
    private CardSpot selected;

    public delegate void CardSpotSelection(CardSpot cardSpot);
    public event CardSpotSelection onCardSpotSelected;

    [SerializeField] private CardViewUI cardViewUI;
    [SerializeField] private Hand hand;

    void Awake()
    {
        _instance = this;
        cardSpots = new List<CardSpot>();
        if (cardViewUI == null) cardViewUI = FindObjectOfType<CardViewUI>();
        if (hand == null) hand = FindObjectOfType<Hand>();
    }

    void Start()
    {
        
        cardViewUI.isCardPlaced += IsCardPlaced;
        hand.onCardPlayed += PlaceCard;
    }

    public void AddCardSpot(CardSpot spot)
    {
        cardSpots.Add(spot);
    }

    public void Select(CardSpot spot)
    {
        Deselect();
        selected = spot;
        StartCoroutine(imageScale());
        selected = spot;
        Interaction.Instance.onDeselection += DeselectionCoroutine;
        onCardSpotSelected?.Invoke(selected);
    }

    public void Deselect()
    {
        if (selected == null) return;
        selected.SetImageScale(normalSize);
        selected = null;
    }

    public bool IsCardPlaced()
    {
        if (selected != null)
        {
            return true;
        }
        else return false;
    }

    private void PlaceCard(Card card)
    {
        if (selected == null) return;
        selected.SetCard(card);
        CardPlacedAnimation(card, selected);
    }

    public void DeselectionCoroutine()
    {
        StartCoroutine(DeselectWait());
    }

    private IEnumerator DeselectWait()
    {
        yield return new WaitForFixedUpdate();
        Deselect();
    }

    /// <summary>
    /// When you place the card, it rapidly selects and deselects. this is meant to avoid that
    /// </summary>
    private void SetSelectedImageScale()
    {
        if(selected != null)
        {
            selected.SetImageScale(selectedSize);
        }
    }

    private IEnumerator imageScale()
    {
        yield return new WaitForFixedUpdate();
        SetSelectedImageScale();
    }

    private void CardPlacedAnimation(Card card, CardSpot spot)
    {

    }
}