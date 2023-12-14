using System.Collections;
using System.Linq;
using UnityEngine;

public class CardGridDisplay : MonoBehaviour
{
    public Deck deck;
    public float cardSpacing = 0.05f;  // Space between cards
    private bool displayAllCards = false;
    private float cardHeight;

    public void StartCardDisplay()
    {
        if (!displayAllCards) {
            if (deck != null && deck.allCards.Count > 0) {
                SortCards();
                GameManager.Instance.ClearTable();
                DisplayCards();
                StartCoroutine( RotateAllCards());
            }  else {
                Debug.LogError("Deck is empty or not assigned!");
            }
        } else {
            RemoveAllCards();
        }
    }

    private void DisplayCards()
    {
        Debug.Log("Displaying Cards ...");
        displayAllCards = true;
        int row = 0;
        int column = 0;
        float cardWidth = deck.allCards[0].cardData.cardPrefab.GetComponent<Renderer>().bounds.size.x;
        cardHeight = deck.allCards[0].cardData.cardPrefab.GetComponent<Renderer>().bounds.size.y;
        float displayOffsetX = cardWidth * 6.5f + (cardSpacing * 6);
        float displayOffsetY = cardHeight * 2f + (cardSpacing * 1.5f);

        foreach (var card in deck.allCards)
        {
            if (card.cardData.cardPrefab != null)
            {
                // Instantiate the card prefab
                GameObject cardObject = Instantiate(card.cardData.cardPrefab, this.transform);

                // Calculate position
                float posX = column * (cardWidth + cardSpacing);
                float posY = row * -(cardHeight + cardSpacing); // Negative for moving down
                cardObject.transform.localPosition = new Vector3(posX - displayOffsetX, 0.01f, posY + displayOffsetY);
                cardObject.transform.rotation = Quaternion.Euler(-90,0,0);
                
                // Update row and column for next card
                column++;
                if (column > 12)
                {
                    column = 0;
                    row++;
                }
            }
        }
    }
    
    private void SortCards()
    {
        deck.allCards = deck.allCards.OrderBy(card => card.cardData.suit).ThenBy(card => card.cardData.value).ToList();
    }
    
    public void RemoveAllCards()
    {
        displayAllCards = false;
        Debug.Log("Removing all cards ...");
        StopAllCoroutines();
        foreach (Transform child in transform)
        {            
            Destroy(child.gameObject);
        }
    }
    
    public IEnumerator RotateAllCards()
    {
        
        foreach (Transform cardTransform in transform)
        {
            yield return StartCoroutine(RotateCard(cardTransform));
        }
    }

    IEnumerator RotateCard(Transform cardTransform) {
            float duration = 0.5f; // Duration for the rotation
            float time = 0;
            Vector3 startAngles = cardTransform.eulerAngles;
            Vector3 endAngles = new Vector3(startAngles.x + 180, startAngles.y, startAngles.z); // Always rotate 180 degrees on the x-axis


            // Lift card slightly more than half of its height so it won't hit the table
            float liftAmount = cardHeight / 2 + 0.01f;

            Vector3 startPosition = cardTransform.position;
            Vector3 liftedPosition = new Vector3(startPosition.x, startPosition.y + liftAmount, startPosition.z);

            // Lift the card up
            cardTransform.position = liftedPosition;

            while (time < duration) {
                if (cardTransform == null) {
                    // If the card GameObject has been destroyed, exit the coroutine
                    yield break;
                }
                cardTransform.eulerAngles = Vector3.Lerp(startAngles, endAngles, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            // Set the final rotation and lower the card back down
            if (cardTransform != null) {
                cardTransform.eulerAngles = endAngles;
                cardTransform.position = startPosition;
            }
    }
}