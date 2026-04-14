using UnityEngine;

public class StockPile : BasePile
{
    public override bool CanAddCard(CardView card)
    {
        // Stock is just a stack — allow adding any card
        return card != null;
    }

    protected override void UpdateLayout()
    {
        // Stack cards at the pile anchor; keep them face-down
        for (int i = 0; i < cards.Count; i++)
        {
            var cardView = cards[i];
            // small z-offset so top card renders above others
            cardView.transform.localPosition = new Vector3(0f, 0f, i * 0.001f);
            cardView.transform.localRotation = Quaternion.identity;

            if (cardView.Card != null)
                cardView.Card.isFaceUp = false;

            // set sorting order so top cards render above lower ones
            cardView.SetSortingOrder(GetSortingForIndex(i), "Cards");

            cardView.UpdateVisual();
        }
    }
}
