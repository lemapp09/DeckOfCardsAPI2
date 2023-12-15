using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

public class ListCards : MonoBehaviour
{

    public void StartListingCards() {
        GameManager.Instance.ClearTable();
        StartCoroutine(GetListOfCards());
    }

    IEnumerator GetListOfCards()
    {
        string deckId = GameManager.Instance.GetDeckID();
        // string pileName = ""; // ""main";
        string url = $"https://deckofcardsapi.com/api/deck/{deckId}/list/";

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
                Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(jsonResponse);

                if (myDeserializedClass.success)
                {
                    // Process the card list here
                    foreach (Card card in myDeserializedClass.piles.player2.cards)
                    {
                        Debug.Log($"Card: {card.value} of {card.suit}. Image URL: {card.image}");
                    }
                }
                else
                {
                    Debug.LogError("Failed to list cards. Response: " + jsonResponse);
                }
            }
        }
    }

    // Define Card, Piles, Player1, Player2, and Root classes here...
    
    //Generated by https://json2csharp.com/
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Card
    {
        public string image { get; set; }
        public string value { get; set; }
        public string suit { get; set; }
        public string code { get; set; }
    }

    public class Piles
    {
        public Player1 player1 { get; set; }
        public Player2 player2 { get; set; }
    }

    public class Player1
    {
        public string remaining { get; set; }
    }

    public class Player2
    {
        public List<Card> cards { get; set; }
        public string remaining { get; set; }
    }

    public class Root
    {
        public bool success { get; set; }
        public string deck_id { get; set; }
        public string remaining { get; set; }
        public Piles piles { get; set; }
    }
}