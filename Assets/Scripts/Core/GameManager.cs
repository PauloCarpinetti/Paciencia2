using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions;

public enum GameState
{
    Setup,
    Playing,
    Victory,
    Defeat
}

public class GameManager : MonoBehaviour
{

    
    [Header("Deck (Debug)")]
    [SerializeField] private List<CardData> deck;

    private DeckManager deckManager = new();

    [Header("Layout")]  
    [SerializeField] private SolitaireLayoutSystem layoutSystem;

    [Header("Board")]
    [SerializeField] private BoardManager boardManager;

    [Header("Prefab")]
    [SerializeField] private CardView cardPrefab;

    //private List<CardView> deckViews = new();
    //private List<CardData> deckData = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartNewGame();
    }

    private void StartNewGame()
    {
        if (layoutSystem == null)
            layoutSystem = FindAnyObjectByType<SolitaireLayoutSystem>();

        if (boardManager == null)
            boardManager = FindAnyObjectByType<BoardManager>();

        if (layoutSystem == null)
        {
            Debug.LogError("GameManager: SolitaireLayoutSystem reference not set or found in scene.");
            return;
        }

        if (boardManager == null)
        {
            Debug.LogError("GameManager: BoardManager reference not set or found in scene.");
            return;
        }

        // layout
        layoutSystem.CalculateScreenBounds();
        layoutSystem.ApplyLayout();

        // deck lifecycle
        deckManager = new DeckManager();

        deck = deckManager.BuildDeck(); // returns internal deck list
        deckManager.Shuffle(deck);

        // mark setup state while dealing
        deckManager.CurrentState = GameState.Setup;

        // --- Assertions & defensive checks before populating the board ---
        int deckCount = deckManager.deckData?.Count ?? 0;

        // Basic expected size check (standard 52-card deck)
        bool sizeOk = deckCount == 52;
        Assert.IsTrue(sizeOk, $"DeckManager deck size is {deckCount}, expected 52.");
        if (!sizeOk)
        {
            Debug.LogError($"GameManager: Deck size invalid ({deckCount}). Aborting board setup.");
            return;
        }

        // Confirm state is Setup so DrawCard is allowed during dealing
        bool stateOk = deckManager.CurrentState == GameState.Setup;
        Assert.IsTrue(stateOk, $"DeckManager CurrentState is {deckManager.CurrentState}, expected Setup.");
        if (!stateOk)
        {
            Debug.LogError($"GameManager: DeckManager in wrong state ({deckManager.CurrentState}). Aborting board setup.");
            return;
        }

        // give board the deck manager so it can instantiate & place cards
        boardManager.SetupBoard(deckManager);

        // game now ready to play
        deckManager.CurrentState = GameState.Playing;
    }
        
    // Update is called once per frame
    void Update()
    {
        
    }
}
