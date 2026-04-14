using UnityEngine;

public class WastePile : BasePile
{
    public override bool CanAddCard(CardView card)
    {
        // Waste accepts cards from stock draws; allow any card
        return card != null;
    }

    protected override void UpdateLayout()
    {
        // Layout waste as a small horizontal fan of face-up cards
        float offsetX = 0.4f;
        for (int i = 0; i < cards.Count; i++)
        {
            var cardView = cards[i];
            cardView.transform.localPosition = new Vector3(i * offsetX, 0f, i * 0.01f);
            cardView.transform.localRotation = Quaternion.identity;

            if (cardView.Card != null)
                cardView.Card.isFaceUp = true;

            // set sorting order so top cards render above lower ones
            cardView.SetSortingOrder(GetSortingForIndex(i), "Cards");

            cardView.UpdateVisual();
        }
    }
}
