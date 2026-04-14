using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Card Sprite Database")]
public class CardSpriteDatabase : ScriptableObject
{
    [Header("52 cartas em ordem: suit * 13 + rank")]
    [SerializeField] private Sprite[] cardSprites;

    [SerializeField] private Sprite backSprite;

    public Sprite GetCardSprite(Suit suit, int rank)
    {
        int index = ((int)suit * 13) + (rank - 1);

        if (index < 0 || index >= cardSprites.Length)
        {
            Debug.LogError($"indice invalido: {index}");
            return null;
        }

        return cardSprites[index];
    }

    public Sprite GetBackSprite()
    {

        return backSprite;
    }


}
