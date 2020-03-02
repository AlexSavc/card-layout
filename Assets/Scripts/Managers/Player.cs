using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public Hand hand;
    public Cemetary cemetary;
    public Deck deck;

    public List<CardSO> cards;
    public Card cardPrefab;

    void Start()
    {
        PopulateDeck();
        SetListeners();
    }

    private void PopulateDeck()
    {
        deck.SetDeck(CreateCards().ToArray());
    }

    private List<Card> CreateCards()
    {
        List<Card> cardList = new List<Card>();

        foreach(CardSO so in cards)
        {
            Card c = Instantiate(cardPrefab);
            c.SetCard(so);
            cardList.Add(c);
        }

        return cardList;
    }

    private void OnPullCard(Card[] pulled)
    {
        hand.AddCards(pulled);
    }

    private void SetListeners()
    {
        deck.onCardPulled += OnPullCard;
    }

    public void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}