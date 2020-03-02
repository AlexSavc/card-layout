using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private List<Card> hand;

    [Header("Hand View")]
    [SerializeField] private GameObject cardParent;
    [SerializeField] private GameObject handUI;
    [SerializeField] private GameObject cardPrefab;

    [Header("Hand on table")]
    [SerializeField] private GameObject handPreviewParent;
    [SerializeField] private PreviewCard handPreviewCard;
    [SerializeField] private Vector2 offsetAndRotation;
    [SerializeField] private GameObject handHiderObject; // keep disabled or off screen

    [Header("Card View")]
    [SerializeField] private CardViewUI cardViewUI;

    public CardAnimations anim;

    public delegate void CardPlayeDelegate(Card card);
    public event CardPlayeDelegate onCardPlayed;

    public delegate void CardNumChangeDelegate();
    public event CardNumChangeDelegate onCardNumChange;

    public delegate void HandOpenDelegate();
    public event HandOpenDelegate onOpenHand;

    private Interaction interaction;

    private bool handCloseOnce;

    void Start()
    {
        interaction = Interaction.Instance;
        cardViewUI.onPlayCard += CardViewPlayed;
    }

    public void OpenHand()
    {
        handUI.SetActive(true);
        ShowPreviewParent(false);
        SetUIHandCards();
        interaction.SetDeselectCondition(cardParent);
        interaction.onDeselection += CloseHand;
        onOpenHand?.Invoke();
        //interaction.AddEventToDeselectionQueue(CloseHand);
        //interaction.AddDeselectionQueue();
    }
    
    public void CloseHand()
    {
        handUI.SetActive(false);
        ShowPreviewParent(true);
        interaction.ClearDeselectCondition();
        interaction.RemoveEventFromDeselection(CloseHand);
    }

    public GameObject[] CreateHandCards()
    {
        GameObject[] cardArray = new GameObject[hand.Count];

        for(int i = 0; i < hand.Count; i++)
        {
            cardArray[i] = Instantiate(cardPrefab);
            HandCardUI ui = cardArray[i].GetComponent<HandCardUI>();
            ui.SetCard(hand[i]);
            ui.onPlayCard += CardPressed;
        }

        return cardArray;
    }

    private void SetUIHandCards()
    {
        Utility.ClearChildren(cardParent);
        foreach (GameObject card in CreateHandCards())
        {
            card.transform.SetParent(cardParent.transform);
        }
    }

    public void AddCards(Card[] cards)
    {
        hand.AddRange(cards);
        Utility.SetParentAll(cards as MonoBehaviour[], handHiderObject);
        cardAdded = true;
        SetHandPreviewCards();
    }

    private bool cardAdded;

    public void RemoveCards(Card[] cards)
    {
        foreach(Card cardd in cards) hand.Remove(cardd);
        cardAdded = false;
        SetHandPreviewCards();
    }

    public void SetHandPreviewCards()
    {
        Utility.ClearChildren(handPreviewParent);
        LayoutPreviewCards(CreatePreviewCards(), GetPreviewCardLayout(hand.Count));
    }

    /// <summary>
    /// Lays out the card objects according to the layout pos offset and rotation
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="layout"></param>
    private void LayoutPreviewCards(GameObject[] cards, Vector2[] layout)
    {
        for(int i = 0; i < cards.Length; i++)
        {
            GameObject card = cards[i]; Vector2 posRot = layout[i];
            
            card.transform.SetParent(handPreviewParent.transform);
            card.transform.localPosition = new Vector3(posRot[0], 0, i);
            card.transform.localEulerAngles = new Vector3(0, 0, posRot[1]);

            LatestCardPos = card.transform.position;
            LatestCardRot = card.transform.eulerAngles;
        }

        if(cardAdded)
        {
            int childCount = handPreviewParent.transform.childCount;
            StartCoroutine(CardAppearTimer(handPreviewParent.transform.GetChild(childCount - 1).gameObject));
        }

        cardAdded = false;
    }

    private IEnumerator CardAppearTimer(GameObject obj)
    {
        obj.SetActive(false);
        yield return new WaitForSeconds(anim.travelTime);
        if(obj != null)
        obj.SetActive(true);
        yield return null;
    }

    /// <summary>
    /// Instantiates HandPreviewCard objects, one for each card in hand
    /// </summary>
    /// <returns></returns>
    private GameObject[] CreatePreviewCards()
    {
        List<GameObject> objs = new List<GameObject>();
        foreach(Card card in hand)
        {
            PreviewCard pr = Instantiate(handPreviewCard.gameObject).GetComponent<PreviewCard>();
            pr.onHandOpen += OpenHand;
            objs.Add(pr.gameObject);
        }
        return objs.ToArray();
    }

    /// <summary>
    /// First dimension is the x offset, second is the rotation. The Parent Object must be rotated 180 degrees from the desired rotation
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    private Vector2[] GetPreviewCardLayout(int amount)
    {
        Vector2[] vectors = new Vector2[amount];
        float offsetAngle = offsetAndRotation[1];
        float xOffset = offsetAndRotation[0];

        float startAngle = ((offsetAngle * amount) / 2) * -1;
        float startX = ((xOffset * amount) / 2) * -1;
        for(int i = 0; i< amount; i++)
        {
            vectors[i] = new Vector2(startX + (i * xOffset), startAngle + (i * offsetAngle) - 180);
        }
        
        return vectors;
    }

    public Vector3 LatestCardPos;
    public Vector3 LatestCardRot;

    public void CardPressed(Card card)
    {
        CloseHand();
        ShowPreviewParent(false);
        OpenCard(card);
    }

    private void OpenCard(Card card)
    {
        cardViewUI.Open(card);
        cardViewUI.SetListeners(new List<CardViewUI.CloseDelegate> { OpenWait });
        cardViewUI.SetListeners(new List<CardViewUI.CardPressDelegate> { CardViewPressed });
        interaction.SetDeselectCondition(cardViewUI.image.gameObject);
        interaction.onDeselection += cardViewUI.Close;
    }

    private void HidePreviewParent(bool hide)
    {
        if (hide) handPreviewParent.SetActive(false);
        else handPreviewParent.SetActive(true);
    }

    private void CardViewPressed(Card card)
    {
        ShowPreviewParent(false);
        interaction.SetDeselectCondition(cardViewUI.cardDuplicate);
    }

    private void CardViewPlayed(Card card)
    {
        onCardPlayed?.Invoke(card);
        RemoveCards(new Card[] { card });
        CloseHand();
    }

    private void ShowPreviewParent(bool show)
    {
        handPreviewParent.SetActive(show);
    }

    private void OpenWait()
    {
        StartCoroutine(CardOpenTimer());
    }

    private IEnumerator CardOpenTimer()
    {
        yield return new WaitForFixedUpdate();
        OpenHand();
        yield return null;
    }
}