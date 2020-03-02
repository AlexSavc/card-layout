using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : CardSpot
{
    [SerializeField] private List<Card> cards;
    [SerializeField] private SpriteRenderer BackOfCard;

    [SerializeField] private CardAnimations anim;
    [SerializeField] private GameObject handPosition;

    public delegate void CardPulledDelegate(Card[] pulled);
    public event CardPulledDelegate onCardPulled;

    public Hand hand;

    void Awake()
    {
        cards = new List<Card>();
        if (anim == null) anim = FindObjectOfType<CardAnimations>();
        if (hand == null) hand = FindObjectOfType<Hand>();
    }

    public Card[] PullCards(int number)
    {
        if(number > cards.Count) { number = cards.Count; }
        Card[] cardArray = new Card[number];
        for(int i = 0; i < number; i++)
        {
            cardArray[i] = cards[0];
            cards.RemoveAt(0);
        }
        CheckDisplay();
        return cardArray;
    }

    public void SetDeck(Card[] cardArray)
    {
        cards = new List<Card>(cardArray);
        foreach(Card card in cards)
        {
            SetCard(card);
        }
    }

    public void AddCards(Card[] cardArray)
    {
        foreach(Card card in cardArray)
        {
            cards.Add(card);
            SetCard(card);
        }
    }

    /// <summary>
    /// Sets the Card's transform parent. Overrides base function, that assigns the CardSpot's card.
    /// </summary>
    /// <param name="toSet"></param>
    public override void SetCard(Card toSet)
    {
        toSet.transform.parent = transform;
        toSet.transform.localPosition = Vector3.zero;
    }

    public override void OnInteraction()
    {
        Card[] pulled = PullCards(1);
        onCardPulled?.Invoke(pulled);
        if(pulled.Length > 0)
        {
            anim.PreviewCardTravel(pulled[0].CardSO, transform.position, hand.LatestCardPos, hand.LatestCardRot);
        }
        
    }

    private void CheckDisplay()
    {
        if(cards.Count < 1)
        {
            BackOfCard.gameObject.SetActive(false);
        }
        else BackOfCard.gameObject.SetActive(true);
    }
}