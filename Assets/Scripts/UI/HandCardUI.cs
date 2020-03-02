using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandCardUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Button button;
    [SerializeField] private Card card;
    public Card Card { get { return card; } }

    public delegate void CardPlayDelegate(Card card);
    public event CardPlayDelegate onPlayCard;

    void Start()
    {
        button.onClick.AddListener(OnPressCard);
    }

    public void SetCard(Card card)
    {
        this.card = card;
        SetImage(this.card.CardSO.cardSprite);
    }

    private void SetImage(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void OnPressCard()
    {
        onPlayCard?.Invoke(card);
    }
}
