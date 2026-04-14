using System.Collections.Generic;
using UnityEngine;

public abstract class BasePile : MonoBehaviour
{
    protected readonly List<CardView> cards = new();

    [Header("Rendering (sorting)")]
    [Tooltip("Base sorting order for this pile. Each card will add sortingStep * index.")]
    [SerializeField] protected int baseSortingOrder = 0;

    [Tooltip("Increment applied per-card in this pile.")]
    [SerializeField] protected int sortingStep = 1;

    public int Count => cards.Count;
    public void AddCard(CardView card)
    {
        if (card == null)
            return;

        cards.Add(card);
        card.transform.SetParent(transform, worldPositionStays: false);
        UpdateLayout();
    }

    // expose base order for subclasses if needed
    protected int GetSortingForIndex(int index) => baseSortingOrder + index * sortingStep;

    // Remove and destroy all cards (used when clearing board)
    public virtual void Clear()
    {
        for (int i = cards.Count - 1; i >= 0; i--)
        {
            var c = cards[i];
            if (c != null)
                Object.Destroy(c.gameObject);
        }
        cards.Clear();
        UpdateLayout();
    }
    // Metodo abstrato: cada filha decide sua propria regra
    public abstract bool CanAddCard(CardView card);

    protected abstract void UpdateLayout();
}
