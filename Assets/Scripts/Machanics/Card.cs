using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private CardSO cardSO;
    public CardSO CardSO { get { return cardSO; } }

    [SerializeField] private SpriteRenderer rend;
    [SerializeField] private string cardName;

    public void SetCard(CardSO scriptable)
    {
        cardSO = Instantiate(scriptable);
        rend.sprite = cardSO.cardSprite;
        cardName = rend.sprite.name;
    } 
}