using UnityEngine;

public class FoundationPile : BasePile
{
    public override bool CanAddCard(CardView card)
    {
        if (card == null || card.Card == null) return false;

        // If empty, only Ace allowed
        if (cards.Count == 0)
            return card.Card.Rank == Rank.Ace;

        // Otherwise must be same suit and rank = topRank + 1
        var top = cards[^1].Card;
        return card.Card.Suit == top.Suit && (int)card.Card.Rank == (int)top.Rank + 1;
    }

    protected override void UpdateLayout()
    {
        // All foundation cards share the same anchor position; show only small z-offset so top is visible
        for (int i = 0; i < cards.Count; i++)
        {
            var cardView = cards[i];
            cardView.transform.localPosition = new Vector3(0f, 0f, i * 0.01f);
            cardView.transform.localRotation = Quaternion.identity;

            // Ensure foundation cards are face-up
            if (cardView.Card != null)
                cardView.Card.isFaceUp = true;

            // set sorting order so top cards render above lower ones
            cardView.SetSortingOrder(GetSortingForIndex(i), "Cards");

            // Update visuals
            cardView.UpdateVisual();
        }
    }
}
