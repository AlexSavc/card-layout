using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardViewUI : MonoBehaviour
{
    public Image image;
    [SerializeField] private Card card;
    public Card Card { get { return card; } }
    [SerializeField] private Canvas canvas;

    [Tooltip("The size of a card in world units, in the scene")]
    [SerializeField] private Vector2 cardSize;

    [SerializeField] private Button cardButton;

    public delegate void CardPressDelegate(Card card);
    public event CardPressDelegate onPressCard;

    public delegate void CardPlayDelegate(Card card);
    public event CardPlayDelegate onPlayCard;

    public delegate void CloseDelegate();
    public event CloseDelegate onCloseCard;

    private bool isDown;
    public Vector3 downposition;

    public void Open(Card card)
    {
        isDown = false;
        gameObject.SetActive(true);
        this.card = card;
        SetImage(this.card.CardSO.cardSprite);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        if (isDown) TryPlaceAndReopen();
        else { onCloseCard?.Invoke(); }
    }

    /// <summary>
    /// To deal with the event order nightmare
    /// </summary>
    /// <param name="sprite"></param>

    private void SetImage(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void OnPressCard()
    {
        isDown = true;
        CreateCardClone();
        onPressCard?.Invoke(card);
    }

    public void SetListeners(List<CloseDelegate> OnCloseCard)
    {
        onCloseCard = null;
        foreach(CloseDelegate action in OnCloseCard)
        {
            onCloseCard += action;
        }
    }
    public void SetListeners(List<CardPressDelegate> OnPlayCard)
    {
        onPressCard = null;
        foreach (CardPressDelegate action in OnPlayCard)
        {
            onPressCard += action;
        }
    }

    /// <summary>
    /// Creates a smaller version of the UI card on the bottom of the screen while you choose where to place it
    /// </summary>
    private void CreateCardClone()
    {
        DuplicateCard();
        SetActiveBigCard(false);
        PlaceBelow(cardDuplicate);
        SetCardDuplicateListeners();
    }

    public GameObject cardDuplicate;
    private void DuplicateCard()
    {
        cardDuplicate = Instantiate(cardButton.gameObject);
        cardDuplicate.transform.SetParent(cardButton.transform.parent, false);
    }

    /// <summary>
    /// Scales an object to 2x the size of a card, specified in cardSize, places it on the botom os the screen
    /// </summary>
    /// <param name="toPlace"></param>
    private void PlaceBelow(GameObject toPlace)
    {
        RectTransform rect = canvas.GetComponent<RectTransform>();
        Vector2 rectSize = Utility.GetRectDimensions(rect);
        LeanTween.moveLocalY(toPlace, - (rectSize.y / 2), 0.1f);
        Vector2 canvasScale = Utility.WorldToCanvasSize(cardSize, Camera.main, rect);
        
        Vector2 CardUISize = Utility.GetRectDimensions(image.GetComponent<RectTransform>());
        float coeff = canvasScale.x / CardUISize.x;

        LeanTween.scale(toPlace, new Vector2(coeff * 2, coeff * 2), 0.1f);
    }

    public delegate bool IsCardPlaced();
    public event IsCardPlaced isCardPlaced;

    private void TryPlaceCard()
    {
        if (isCardPlaced?.Invoke() == true)
        {
            onPlayCard(card);
        }
        else
        {
            isDown = false;
            Close();
        }
    }

    /// <summary>
    /// Only use when coming back from the minimized card view in CreateCardClone
    /// </summary>
    public void ReopenCard()
    {
        isDown = false;
        SetActiveBigCard(true);
        if (cardDuplicate != null)
        {
            Destroy(cardDuplicate.gameObject);
        }
    }

    
    /// <summary>
    /// When you try place a card, it deselects so you go to reopen, 
    /// but then you check if the card was placed. 
    /// This one is for when you want to reopen and check, othrwise just use ReopenCard()
    /// </summary>
    public void TryPlaceAndReopen()
    {
        TryPlaceCard();
        ReopenCard();
    }

    private void SetPosToZero(GameObject obj)
    {
        obj.transform.localPosition = Vector3.zero;
    }

    private void SetCardDuplicateListeners()
    {
        if (cardDuplicate == null) { return; }
        Button button = cardDuplicate.GetComponent<Button>();
        button.onClick = null;
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(ReopenCard);
    }

    private void SetActiveBigCard(bool active)
    {
        image.gameObject.SetActive(active);
    }
}