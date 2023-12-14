using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;

public class DeckShuffler : MonoBehaviour
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Root
    {
        public bool success { get; set; }
        public string deck_id { get; set; }
        public bool shuffled { get; set; }
        public int remaining { get; set; }
    }
    
    private readonly string shuffleUrl = "https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1";

    void Start()
    {
        StartCoroutine(ShuffleDeck());
    }

    IEnumerator ShuffleDeck()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(shuffleUrl))
        {
            // Send the request and wait for the response
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || 
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                // Parse the response
                // DeckResponse response = JsonUtility.FromJson<DeckResponse>(webRequest.downloadHandler.text);
                Root response = JsonConvert.DeserializeObject<Root>(webRequest.downloadHandler.text);
                
                Debug.Log("Deck ID: " + response.deck_id);
                GameManager.Instance.UpdateDeckID( response.deck_id);
                GameManager.Instance.UpdateCardsRemaining(response.remaining);
            }
        }
    }

    private class DeckResponse
    {
        public string deck_id;
    }
}

public enum CardSuit
{
    Clubs,
    Spades,
    Hearts,
    Diamonds
}

public enum CardValue
{
    Ace,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King
}
