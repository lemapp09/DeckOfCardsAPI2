using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections;

public class DeckReshuffler : MonoBehaviour
{
    private string deckId;

    public void Reshuffle(){
        StartCoroutine(ReshuffleDeck());
    }

    IEnumerator ReshuffleDeck()
    {
        deckId = GameManager.Instance.GetDeckID();
        string url = $"https://www.deckofcardsapi.com/api/deck/{deckId}/shuffle/";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
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
                string jsonResponse = webRequest.downloadHandler.text;
                Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(jsonResponse);

                if (myDeserializedClass.success)
                {
                    Debug.Log("Deck reshuffled successfully. Remaining cards: " + myDeserializedClass.remaining);
                    GameManager.Instance.UpdateCardsRemaining(myDeserializedClass.remaining);
                }
                else
                {
                    Debug.LogError("Failed to reshuffle the deck. Response: " + jsonResponse);
                }
            }
        }
    }

    // Root class as you defined
    public class Root
    {
        public bool success { get; set; }
        public string deck_id { get; set; }
        public bool shuffled { get; set; }
        public int remaining { get; set; }
    }
}