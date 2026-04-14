using UnityEngine;

public class CardView : MonoBehaviour
{
    [SerializeField] private CardSpriteDatabase spriteDatabase;

    private SpriteRenderer spriteRenderer;
    private CardData data;

    [Header("Card Sides")]
    [SerializeField] GameObject cardFront;
    [SerializeField] GameObject cardBack;
    private Suit suit;
    private Rank rank;

    // Expose the bound CardData for pile logic (read-only)
    public CardData Card => data;


    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"CardView ({name}): no SpriteRenderer found on prefab. Add one to root or a child.");
            return;
        }

        // Ensure renderer tint isn't hiding the sprite
        spriteRenderer.color = Color.white;

        // Quick sanity check on material/shader
        if (spriteRenderer.sharedMaterial == null || spriteRenderer.sharedMaterial.shader == null
            || spriteRenderer.sharedMaterial.shader.name != "Sprites/Default")
        {
            Debug.LogWarning($"CardView ({name}): SpriteRenderer material/shader may be incorrect ({spriteRenderer.sharedMaterial?.shader?.name}). Expected 'Sprites/Default'.");
        }

    }

    public void Bind(CardData cardData)
    {
        data = cardData;
        UpdateVisual();
        // Diagnostic: log which sprite got assigned (helps detect nulls)
        Debug.Log($"CardView ({name}): Bound {data} - sprite: {spriteRenderer?.sprite?.name ?? "null"} - renderer color: {spriteRenderer?.color}");
    }

    public void UpdateVisual()
    {
        if (data == null) return;

        if (spriteRenderer == null)
        {
            Debug.LogError($"CardView ({name}): SpriteRenderer not found. Ensure a SpriteRenderer exists on this GameObject or a child.");
            return;
        }

        if (spriteDatabase == null)
        {
            Debug.LogError($"CardView ({name}): spriteDatabase not assigned on the CardView prefab. Assign a CardSpriteDatabase ScriptableObject in the inspector.");
            return;
        }

        if (!data.isFaceUp)
        {
            var back = spriteDatabase.GetBackSprite();
            if (back == null)
            {
                Debug.LogError($"CardView ({name}): CardSpriteDatabase.backSprite is null. Open the CardSpriteDatabase asset and assign the back sprite.");
            }
            spriteRenderer.sprite = back;
            // keep tint white
            spriteRenderer.color = Color.white;
            return;
        }

        var cardSprite = spriteDatabase.GetCardSprite(data.Suit, (int)data.Rank);
        if (cardSprite == null)
        {
            Debug.LogError($"CardView ({name}): GetCardSprite returned null for {data}. Check the CardSpriteDatabase sprites array and indexing.");
        }
        else
        {
            spriteRenderer.sprite = cardSprite;
            spriteRenderer.color = Color.white; // ensure no tint
        }

        Debug.Log($"CardView ({name}): UpdateVisual assigned sprite '{spriteRenderer.sprite?.name ?? "null"}'");
    }

    public void SetWorldSize(Vector2 worldSize)
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning($"CardView ({name}): no SpriteRenderer to size.");
            return;
        }

        // Try to get current sprite
        var sprite = spriteRenderer.sprite;

        // If no sprite yet, attempt to assign one from the database (fallback)
        if (sprite == null)
        {
            // Attempt to populate spriteRenderer from known sources so sizing can proceed
            if (spriteDatabase != null)
            {
                Sprite candidate = (data == null || !data.isFaceUp)
                    ? spriteDatabase.GetBackSprite()
                    : spriteDatabase.GetCardSprite(data.Suit, (int)data.Rank);


                //// Prefer back sprite when we don't have data or card is face-down
                //if (data == null || !data.isFaceUp)
                //{
                //    candidate = spriteDatabase.GetBackSprite();
                //}
                //else
                //{
                //    candidate = spriteDatabase.GetCardSprite(data.Suit, (int)data.Rank);
                //}

                if (candidate != null)
                {
                    spriteRenderer.sprite = candidate;
                    sprite = candidate;
                }
            }

            // If still null, call UpdateVisual once more (it logs why)
            if (sprite == null)
            {
                UpdateVisual();
                sprite = spriteRenderer.sprite;
            }

            if (sprite == null)
            {
                Debug.LogWarning($"CardView ({name}): sprite is null; cannot size to world size yet. Ensure spriteDatabase and sprites are assigned and Bind() has been called before sizing.");
                return;
            }

            //// sprite may be assigned by Bind(); call UpdateVisual first if needed
            //UpdateVisual();
            //sprite = spriteRenderer.sprite;
            //if (sprite == null)
            //{
            //    Debug.LogWarning($"CardView ({name}): sprite is null; cannot size to world size yet.");
            //    return;
            //}
        }

        // sprite.bounds.size is in world units for scale=1
        Vector2 spriteWorldSize = sprite.bounds.size;
        if (spriteWorldSize.x <= 0 || spriteWorldSize.y <= 0)
        {
            Debug.LogWarning($"CardView ({name}): sprite bounds are zero; cannot size. Sprite: {sprite.name}");
            return;
        }

        float scaleX = worldSize.x / spriteWorldSize.x;
        float scaleY = worldSize.y / spriteWorldSize.y;

        transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }

    // allow control of sorting order and layer
    public void SetSortingOrder(int order, string sortingLayerName = null)
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null)
            return;

        spriteRenderer.sortingOrder = order;
        if (!string.IsNullOrEmpty(sortingLayerName))
            spriteRenderer.sortingLayerName = sortingLayerName;
    }
    public int GetRankValue(Rank rank)
    {
        return (int)rank;
    }

    public void Flip(bool faceUp)
    {
        if (data == null) return;
        data.isFaceUp = faceUp;
        UpdateVisual();
    }
}
