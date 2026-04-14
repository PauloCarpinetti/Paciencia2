using System.Collections.Generic;
using UnityEngine;

public class DeckManager 
{
   
    public List<CardData> deckData = new();

    private List<CardView> allCards = new();

    public GameState CurrentState { get; set; } = GameState.Setup;

    public List<CardData> BuildDeck()
    {
        // var deck = new List<CardData>();
        deckData.Clear();

        foreach (Suit suit in System.Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank rank in System.Enum.GetValues(typeof(Rank)))
            {
                try
                {
                    CardData cardData = new(suit, rank);
                    deckData.Add(cardData);
                    Debug.Log($"Created card: {suit} {rank}");
                    Debug.Log($"Deck now has {deckData.Count} cards.");
                    Debug.Log($"CardData: {cardData}");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error creating card: {suit} {rank} - {ex.Message}");

                }
            }

        }
        Debug.Log($"Deck built with {deckData.Count} cards.");
        return deckData;
    }

    public CardData DrawCard()
    {
        if (deckData == null || deckData.Count == 0)
            return null;
        // Segurança: só permite comprar se o jogo estiver rodando
        if (CurrentState != GameState.Setup && CurrentState != GameState.Playing)
            return null;
        CardData card = deckData[0];
        deckData.RemoveAt(0);
        return card;
                   
        // placeholder para lógica de compra de carta

        // opcional: atualizar pontuação, registrar move, etc
        // scoreManager.OnDrawCard();
        // undoManager.RegisterDrawMove();
    }

    private void ClearOldGame()
    {
        foreach (var card in allCards)
        {
           // if (card != null)
               // Destroy(card.gameObject);
        }
        allCards.Clear();
    }

    public void Shuffle(List<CardData> cards)
    {
        if (cards == null || cards.Count <= 1) return;

        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]);
            Debug.Log($"Embaralahndo as cartas {i} vez");
        }
        Debug.Log("Deck shuffled.");
    }

    // Shuffle the internal deckData
    public void Shuffle()
    {
        Shuffle(deckData);
    }
}
