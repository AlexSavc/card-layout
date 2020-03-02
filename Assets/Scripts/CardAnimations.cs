using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAnimations : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    public float travelTime = 0.2f;

    public void PreviewCardTravel(CardSO cardSO, Vector3 from, Vector3 to, Vector3 targetRotation)
    {
        GameObject card = SpawnCard(from, cardSO);
        to.y += 1.23f;
        LeanTween.move(card, to, travelTime).setEaseInExpo();
        LeanTween.rotate(card, targetRotation, 0.5f);
        StartCoroutine(DestroyObjectAfter(card, travelTime));
    }

    public void UIScale(GameObject obj, GameObject targetScale)
    {

    }

    private GameObject SpawnCard(Vector3 initialPos, CardSO setCard)
    {
        Card card = Instantiate(cardPrefab);
        card.transform.position = initialPos;
        card.SetCard(setCard);
        return card.gameObject;
    }

    private IEnumerator DestroyObjectAfter(GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(obj);
    }
}
