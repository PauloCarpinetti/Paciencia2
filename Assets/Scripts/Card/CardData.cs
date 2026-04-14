using UnityEngine;

public enum Suit
{
    hearts,
    diamonds,
    clubs,
    spades
}

public enum Rank
{
    Ace = 1,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    
}
public enum CardColor
{
    Red,
    Black
}
public class CardData
{
    public Suit Suit { get; }
    public Rank Rank { get; }

    public bool isFaceUp;

    public CardColor Color =>
        (Suit == Suit.hearts || Suit == Suit.diamonds)
        ? CardColor.Red
        : CardColor.Black;

    public CardData(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
        isFaceUp = true;
    }

    public override string ToString()
    {
        return $"{Rank}_{Suit}";
    }
}
