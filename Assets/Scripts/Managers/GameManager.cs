using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private string deck_id;
    private int cardsRemaining;
    [SerializeField] private CardGridDisplay _cardDisplay;
    [SerializeField] private CardDrawer _cardDrawer;

    public void UpdateDeckID(string DeckId) {
        deck_id = DeckId;
        UIManager.Instance.UpdateDeckID(deck_id);
    }

    public string GetDeckID() {
        return deck_id;
    }

    public void ClearTable() {
        _cardDisplay.RemoveAllCards();
        _cardDrawer.ClearInstantiatedCards();
    }

    public void UpdateCardsRemaining(int cards) {
        cardsRemaining = cards;
        UIManager.Instance.UpdateCardsRemaininhg(cardsRemaining);
    }

}
