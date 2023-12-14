using TMPro;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private TMP_Text _deckIDText, _cardsRemaining;

    public void UpdateDeckID(string id) {
        _deckIDText.text = "Deck ID:\n" + id;
    }
    public void UpdateCardsRemaininhg(int numOfCrards) {
        _cardsRemaining.text = "Cards Remaining:\n" + numOfCrards;
    }
}
