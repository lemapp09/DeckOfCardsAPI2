using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class CardDrawer : MonoBehaviour
{
    [SerializeField] private int numCardsToDraw = 2;
    [SerializeField] private Deck deck; 
    private List<GameObject> instantiatedCards = new List<GameObject>();

    
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Card {
        public string code { get; set; }
        public string image { get; set; }
        public Images images { get; set; }
        public string value { get; set; }
        public string suit { get; set; }
    }

    public class Images {
        public string svg { get; set; }
        public string png { get; set; }
    }

    public class Root {
        public bool success { get; set; }
        public string deck_id { get; set; }
        public List<Card> cards { get; set; }
        public int remaining { get; set; }
    }
    
    public void StartDrawingCards()
    {
        GameManager.Instance.ClearTable();
        StartCoroutine(DrawCards());
    }

    IEnumerator DrawCards()
    {
        string deckId = GameManager.Instance.GetDeckID();
        if (string.IsNullOrEmpty(deckId))
        {
            Debug.LogError("Deck ID is null or empty.");
            yield break;
        }

        string url = $"https://deckofcardsapi.com/api/deck/{deckId}/draw/?count={numCardsToDraw}";
        Debug.Log("Requesting URL: " + url);

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || 
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Response: " + jsonResponse);
                Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(jsonResponse);
                GameManager.Instance.UpdateCardsRemaining(myDeserializedClass.remaining);

                if (myDeserializedClass.success)
                {
                    Debug.Log("Cards drawn successfully. Remaining cards: " + myDeserializedClass.remaining);

                    int loopID = 0;
                    float cardWidth = deck.allCards[0].cardData.cardPrefab.GetComponent<Renderer>().bounds.size.x;
                    float offsetX = (numCardsToDraw * (cardWidth + 0.05f)) - 0.05f;
                    foreach (var card in myDeserializedClass.cards)
                    {
                        Debug.Log($"Card: {card.value} of {card.suit}. Image URL: {card.image}");
                        Cards foundCard = deck.GetCard(card.code);
                        if (foundCard != null)
                        {
                            Debug.Log("Found Card: " + foundCard.cardData.value + " of " + foundCard.cardData.suit);
                        }
                        else
                        {
                            Debug.Log("Card not found in the deck.");
                        }

                        deck.PlayCard(foundCard);
                        Vector3 tempPos = new Vector3( (loopID * (cardWidth + 0.05f)) - offsetX, 0.02f, 0 );
                        GameObject cardObject = Instantiate(foundCard.cardData.cardPrefab, deck.transform );
                        instantiatedCards.Add(cardObject);
                        cardObject.transform.position = tempPos;
                        cardObject.transform.rotation = Quaternion.Euler(-90,0,0);
                        // Additional processing, such as displaying the card images in your game, goes here.
                        loopID++;
                    }
                }
                else
                {
                    Debug.LogError("Failed to draw cards. Response: " + jsonResponse);
                }
            }
        }
    }
    
    public void ClearInstantiatedCards()
    {
        foreach (var cardObject in instantiatedCards)
        {
            if (cardObject != null)
            {
                Destroy(cardObject);
            }
        }
        instantiatedCards.Clear();
    }
}