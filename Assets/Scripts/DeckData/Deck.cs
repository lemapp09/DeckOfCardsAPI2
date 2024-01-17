using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Deck : MonoBehaviour
{
    [SerializeField]
    public List<Cards> allCards = new List<Cards>(); // Holds all 52 card scriptable objects

    private Dictionary<Cards, CardState> cardStates = new Dictionary<Cards, CardState>();

    private void Start()
    {
        InitializeDeck();
    }

    private void InitializeDeck()
    {
        foreach (Cards card in allCards)
        {
            cardStates[card] = new CardState();
        }
    }

    public void PlayCard(Cards card)
    {
        if (cardStates.ContainsKey(card))
        {
            cardStates[card].HasBeenPlayed = true;
        }
    }
    
    // Method to find a card by suit and value
    public Cards GetCard(string code)
    {
        return allCards.FirstOrDefault(card => card.cardData.code.ToString() == code);
    }

    // Other methods like DiscardCard, AddToHand, etc. would be similar to PlayCard

    public class CardState
    {
        public bool HasBeenPlayed { get; set; }
        public bool HasBeenDiscarded { get; set; }
        public bool InHand1 { get; set; }
        public bool InHand2 { get; set; }
        public bool InHand3 { get; set; }
        public bool InHand4 { get; set; }
        public bool InMainDeck { get; set; }
    }
}