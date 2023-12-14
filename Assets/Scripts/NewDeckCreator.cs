using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections;

public class NewDeckCreator : MonoBehaviour
{
    public void StartCreatingNewDeck() {
        StartCoroutine(CreateNewDeck());
    }

    IEnumerator CreateNewDeck()
    {
        string url = "https://deckofcardsapi.com/api/deck/new/";

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
                    Debug.Log("New Deck Created. Deck ID: " + myDeserializedClass.deck_id);
                    GameManager.Instance.UpdateDeckID(myDeserializedClass.deck_id);
                }
                else
                {
                    Debug.LogError("Failed to create a new deck. Response: " + jsonResponse);
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