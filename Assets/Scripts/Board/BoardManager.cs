using System;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Card Prefab")]
    [SerializeField] private CardView cardPrefab;

    [Header("Pile References")]
    [SerializeField] private TableauPile[] tableaus;        // 7
    [SerializeField] private FoundationPile[] foundations;  // 4
    [SerializeField] private StockPile stock;
    [SerializeField] private WastePile waste;

    [Header("Slot Debug / Visuals")]
    [Tooltip("Prefab used to draw slot anchors. Must contain a SpriteRenderer to be visible.")]
    [SerializeField] private GameObject slotPrefab;

    private DeckManager deckManager;
    private SolitaireLayoutSystem layoutSystem;

    // Root name used by the slot builder / visualizer
    private const string SLOTS_ROOT_NAME = "GameLayoutSlots";
    private const string SLOTS_SORTING_LAYER = "Slots";
    private Transform slotsRoot;

    // ==========================================================
    // SETUP
    // ==========================================================
    public void SetupBoard(DeckManager deck)
    {
        deckManager = deck;
        // find layout system once so we can size cards
        if (layoutSystem == null)
            layoutSystem = FindAnyObjectByType<SolitaireLayoutSystem>();

        ClearBoard();
        DealTableau();
        FillStock();
        DealSlots();
    }

    private void FillStock()
    {
        if (deckManager == null)
        {
            Debug.LogError("BoardManager.FillStock: deckManager is null");
            return;
        }

        if (stock == null)
        {
            Debug.LogWarning("BoardManager.FillStock: stock pile reference is null. Skipping fill.");
            return;
        }

        Vector2 desiredSize = layoutSystem != null ? layoutSystem.CardSize : new Vector2(1.4f, 2.0f);

        CardData cardData;
        while ((cardData = deckManager.DrawCard()) != null)
        {
            cardData.isFaceUp = false;
            var cardView = Instantiate(cardPrefab, stock.transform);
            cardView.Bind(cardData);
            cardView.SetWorldSize(desiredSize);
            stock.AddCard(cardView);
        }
    }
    private void ClearBoard()
    {
        // remove children from each pile transform (safe if transform fields are assigned)
        if (tableaus != null)
        {
            foreach (var t in tableaus)
            {
                if (t == null || t.transform == null) continue;

                for (int i = t.transform.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(t.transform.GetChild(i).gameObject);
                }
            }
        }

        if (foundations != null)
        {
            foreach (var f in foundations)
            {
                if (f == null || f.transform == null) continue;

                for (int i = f.transform.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(f.transform.GetChild(i).gameObject);
                }
            }
        }

        if (stock != null && stock.transform != null)
        {
            for (int i = stock.transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(stock.transform.GetChild(i).gameObject);
        }

        if (waste != null && waste.transform != null)
        {
            for (int i = waste.transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(waste.transform.GetChild(i).gameObject);
        }
    }

    // ==========================================================
    // DEALING
    // ==========================================================

    private void DealTableau()
    {
        // Distribuição clássica:
        // Coluna 1 -> 1 carta
        // Coluna 2 -> 2 cartas
        // ...
        // Coluna 7 -> 7 cartas

        if (deckManager == null)
        {
            Debug.LogError("BoardManager.DealTableau: deckManager is null");
            return;
        }

        if (tableaus == null || tableaus.Length == 0)
        {
            Debug.LogError("BoardManager.DealTableau: tableaus not configured in inspector.");
            return;
        }

        int tableauCount = tableaus.Length;
        int requiredCards = tableauCount * (tableauCount + 1) / 2;
        int available = deckManager.deckData?.Count ?? 0;

        Vector2 desiredSize = layoutSystem != null ? layoutSystem.CardSize : new Vector2(1.4f, 2.0f);

        Debug.Log($"BoardManager.DealTableau: need {requiredCards} cards for {tableauCount} tableaus. Deck has {available}.");

        if (available < requiredCards)
        {
            Debug.LogError($"Not enough cards to deal tableau. Required: {requiredCards}, available: {available}. Aborting deal.");
            return;
        }

        for (int col = 0; col < tableaus.Length; col++)
        {
            for (int row = 0; row <= col; row++)
            {
                CardData cardData = deckManager.DrawCard();
                Debug.Log(cardData);
                if (cardData == null)
                {
                    Debug.LogError($"Deck ran out of cards during DealTableau at column {col}, row {row}! Stopping.");
                    return;
                }

                bool faceUp = (row == col);

                cardData.isFaceUp = faceUp;

                CardView cardView = Instantiate(cardPrefab);

                if (tableaus[col] != null && tableaus[col].transform != null)
                    cardView.transform.SetParent(tableaus[col].transform, worldPositionStays: false);

                cardView.Bind(cardData);
                cardView.SetWorldSize(desiredSize);

                tableaus[col].AddCard(cardView);
            }
        }
    }

    /// <summary>
    /// Create visual slot placeholders at each pile anchor and parent them under "GameLayoutSlots".
    /// Requires a slotPrefab assigned in the inspector (prefab should include a SpriteRenderer).
    /// Call this after layout.ApplyLayout() so anchors are in final positions.
    /// </summary>
    private void DealSlots()
    {
        if ((tableaus == null || tableaus.Length == 0) &&
            (foundations == null || foundations.Length == 0) &&
            stock == null && waste == null)
        {
            Debug.LogWarning("BoardManager.DealSlots: no pile anchors configured in inspector.");
            return;
        }

        var existing = GameObject.Find(SLOTS_ROOT_NAME);
        if (existing != null)
            slotsRoot = existing.transform;
        else
            slotsRoot = new GameObject(SLOTS_ROOT_NAME).transform;

        // clear previous
        for (int i = slotsRoot.childCount - 1; i >= 0; i--)
        {
            var child = slotsRoot.GetChild(i);
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }

        if (slotPrefab == null)
        {
            Debug.LogWarning("BoardManager.DealSlots: slotPrefab not assigned. Assign a prefab to visualize slots.");
            return;
        }

        // Desired world size for a slot: use layout card size (you can tweak if you want a smaller slot)
        Vector2 desiredWorldSize = layoutSystem != null ? layoutSystem.CardSize : new Vector2(1.4f, 2.0f);

        GameObject InstantiateAndSizeSlot(Transform anchor, string name)
        {
            if (anchor == null) return null;

            var inst = Instantiate(slotPrefab, anchor.position, Quaternion.identity, slotsRoot);
            inst.name = name;

            // reset local rotation so it matches anchor orientation
            inst.transform.localRotation = Quaternion.identity;

            // ensure sorting layer
            var sr = inst.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerName = SLOTS_SORTING_LAYER;
                sr.sortingOrder = 0;
            }

            // Resize slot so sprite fills desiredWorldSize (preserve aspect by scaling uniformly)
            if (sr != null && sr.sprite != null)
            {
                Vector2 spriteWorldSize = sr.sprite.bounds.size; // size at scale = 1
                if (spriteWorldSize.x > 0 && spriteWorldSize.y > 0)
                {
                    // compute uniform scale that fits within desiredWorldSize (preserve aspect)
                    float scaleX = desiredWorldSize.x / spriteWorldSize.x;
                    float scaleY = desiredWorldSize.y / spriteWorldSize.y;

                    // pick the smaller scale so sprite fully fits within target size
                    float uniformScale = Mathf.Min(scaleX, scaleY);

                    inst.transform.localScale = new Vector3(uniformScale, uniformScale, 1f);
                }
                else
                {
                    Debug.LogWarning($"BoardManager.DealSlots: slot prefab sprite bounds are invalid for '{inst.name}'.");
                }
            }
            else
            {
                Debug.LogWarning($"BoardManager.DealSlots: slot prefab missing SpriteRenderer or sprite for '{inst.name}'.");
            }

            return inst;
        }

        // instantiate for each anchor
        if (stock != null && stock.transform != null) InstantiateAndSizeSlot(stock.transform, "Slot_Stock");
        if (waste != null && waste.transform != null) InstantiateAndSizeSlot(waste.transform, "Slot_Waste");

        if (foundations != null)
        {
            for (int i = 0; i < foundations.Length; i++)
            {
                var f = foundations[i];
                if (f != null && f.transform != null)
                    InstantiateAndSizeSlot(f.transform, $"Slot_Foundation_{i}");
            }
        }

        if (tableaus != null)
        {
            for (int i = 0; i < tableaus.Length; i++)
            {
                var t = tableaus[i];
                if (t != null && t.transform != null)
                    InstantiateAndSizeSlot(t.transform, $"Slot_Tableau_{i}");
            }
        }

        Debug.Log("BoardManager.DealSlots: created slot visuals under " + SLOTS_ROOT_NAME);
    }
}



