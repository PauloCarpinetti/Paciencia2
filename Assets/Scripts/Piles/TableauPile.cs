using UnityEngine;

public class TableauPile : BasePile
{
    public override bool CanAddCard(CardView card)
    {
        if (card == null || card.Card == null) return false;

        // Empty tableau accepts only King
        if (cards.Count == 0)
            return card.Card.Rank == Rank.King;

        // Otherwise alternating colors and rank = topRank - 1
        var top = cards[^1].Card;
        if (top == null) return false;

        bool differentColor = card.Card.Color != top.Color;
        bool rankIsOneLower = (int)card.Card.Rank == (int)top.Rank - 1;
        return differentColor && rankIsOneLower;
    }

    protected override void UpdateLayout()
    {
        // Cascade cards vertically. Use smaller overlap for face-down cards.
        float yFaceDownStep = -0.15f;
        float yFaceUpStep = -0.4f;

        float y = 0f;
        for (int i = 0; i < cards.Count; i++)
        {
            var cardView = cards[i];
            var card = cardView.Card;

            // determine step based on whether the card is face up
            float step = (card != null && card.isFaceUp) ? yFaceUpStep : yFaceDownStep;

            cardView.transform.localPosition = new Vector3(0f, y, i * 0.01f);
            cardView.transform.localRotation = Quaternion.identity;

            // set sorting order so top cards render above lower ones
            cardView.SetSortingOrder(GetSortingForIndex(i), "Cards");

            // Ensure face-up state is reflected visually
            cardView.UpdateVisual();

            y += step;
        }
    }
}
