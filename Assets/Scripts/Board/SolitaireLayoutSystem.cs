using System;
using UnityEngine;

public class SolitaireLayoutSystem : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    [Header("Slots (Anchors)")]
    [SerializeField] private Transform stockSlot;
    [SerializeField] private Transform wasteSlot;

    [SerializeField] private Transform[] foundationSlots = new Transform[4];
    [SerializeField] private Transform[] tableauSlots = new Transform[7];

    [Header("Layout Settings")]
    [SerializeField] private float margin = 0.6f;
    [SerializeField] private float topRowHeightRatio = 0.22f;     // altura reservada para a linha de cima
    [SerializeField] private float tableauStartRatio = 0.28f;     // onde começa o tableau no eixo Y (percentual)
    [SerializeField] private float depthZ = 0f;

    [Header("Card Settings (World Units)")]
    [SerializeField] private Vector2 cardSize = new(1.4f, 2.0f);
    [SerializeField] private float minHorizontalGap = 0.25f;

    // expose card size so other systems can size instantiated cards
    public Vector2 CardSize => cardSize;

    private Bounds screenBounds;
    private Vector2 lastScreenSize;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }
    void Start()
    {
        
    }

    public void CalculateScreenBounds()
    {
        if (targetCamera == null)
            return;

        if (targetCamera.orthographic)
        {
            float height = targetCamera.orthographicSize * 2f;
            float width = height * targetCamera.aspect;

            Vector3 center = new Vector3(targetCamera.transform.position.x, targetCamera.transform.position.y, depthZ);
            screenBounds = new Bounds(center, new Vector3(width, height, 1f));
        }
        else
        {
            float distance = Mathf.Abs(depthZ - targetCamera.transform.position.z);

            Vector3 bottomLeft = targetCamera.ViewportToWorldPoint(new Vector3(0, 0, distance));
            Vector3 topRight = targetCamera.ViewportToWorldPoint(new Vector3(1, 1, distance));

            Vector3 center = (bottomLeft + topRight) * 0.5f;
            Vector3 size = topRight - bottomLeft;

            screenBounds = new Bounds(center, new Vector3(size.x, size.y, 1f));
        }
    }

    public void ApplyLayout()
    {
        float left = screenBounds.min.x + margin;
        float right = screenBounds.max.x - margin;
        float top = screenBounds.max.y - margin;
        float bottom = screenBounds.min.y + margin;

        float width = right - left;
        float height = top - bottom;

        // ---------------------------------------
        // Tableau (7 colunas) - compute positions first
        // ---------------------------------------
        int tableauCount = tableauSlots.Length;
        float tableauTotalCardsWidth = tableauCount * cardSize.x;

        float tableauFreeSpace = width - tableauTotalCardsWidth;
        float tableauGap = Mathf.Max(minHorizontalGap, tableauFreeSpace / Mathf.Max(1, (tableauCount - 1)));

        if (tableauTotalCardsWidth + tableauGap * (tableauCount - 1) > width)
        {
            tableauGap = tableauFreeSpace / Mathf.Max(1, (tableauCount - 1));
        }

        float tableauStartX = left + (cardSize.x * 0.5f);
        float tableauTopY = top - (height * topRowHeightRatio) - margin; // place top of tableau below top row
        float tableauStartY = tableauTopY - (cardSize.y * 0.5f);

        for (int i = 0; i < tableauSlots.Length; i++)
        {
            if (tableauSlots[i] == null)
                continue;

            float tx = tableauStartX + i * (cardSize.x + tableauGap);
            tableauSlots[i].position = new Vector3(tx, tableauStartY, depthZ);
        }

        // compute tableau horizontal span for alignment
        float tableauFirstX = tableauStartX;
        float tableauLastX = tableauStartX + (tableauCount - 1) * (cardSize.x + tableauGap);

        // ---------------------------------------
        // Top row (Stock, Waste, Foundations) aligned to tableau span
        // ---------------------------------------
        // Place top row aligned so stock is at the first tableau column
        float topRowY = top - (cardSize.y * 0.5f);
        float topSlotsCount = 6; // Stock + Waste + 4 foundations

        // Use the same gap as tableau to keep consistent spacing; if that would overflow, fallback to computed gap
        float topGap = tableauGap;

        // stock at tableauFirstX
        Vector3 stockPos = new Vector3(tableauFirstX, topRowY, depthZ);

        // waste next to stock
        Vector3 wastePos = new Vector3(tableauFirstX + (cardSize.x + topGap), topRowY, depthZ);

        // foundations start after stock+waste
        float foundationStartX = tableauFirstX + 2f * (cardSize.x + topGap);

        // If foundations would overflow the right margin, fall back to centering top row in available width
        float foundationsSpan = 4 * cardSize.x + 3 * topGap;
        float foundationsEnd = foundationStartX + foundationsSpan - (cardSize.x * 0.5f);
        if (foundationsEnd > right)
        {
            // center top row across the available width
            float totalTopWidth = topSlotsCount * cardSize.x + (topSlotsCount - 1) * topGap;
            float centeredX0 = left + (width - totalTopWidth) * 0.5f + (cardSize.x * 0.5f);

            stockPos = new Vector3(centeredX0, topRowY, depthZ);
            wastePos = new Vector3(centeredX0 + (cardSize.x + topGap), topRowY, depthZ);
            foundationStartX = centeredX0 + 2f * (cardSize.x + topGap);
        }

        // Apply Stock and Waste
        if (stockSlot != null)
            stockSlot.position = stockPos;

        if (wasteSlot != null)
            wasteSlot.position = wastePos;

        // Apply Foundations (4 piles)
        for (int i = 0; i < foundationSlots.Length; i++)
        {
            if (foundationSlots[i] == null)
                continue;

            float fx = foundationStartX + 2.5f + i * (cardSize.x + topGap);
            foundationSlots[i].position = new Vector3(fx, topRowY, depthZ);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (ScreenSizeChanged())
        {
            CalculateScreenBounds();
            ApplyLayout();
        }
    }

    private bool ScreenSizeChanged()
    {
        if (lastScreenSize.x != Screen.width || lastScreenSize.y != Screen.height)
        {
            lastScreenSize = new Vector2(Screen.width, Screen.height);
            return true;
        }
        return false;
    }

    // ============================================================
    // DEBUG DRAW
    // ============================================================
    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireCube(screenBounds.center, screenBounds.size);

    //    Gizmos.color = Color.yellow;

    //    if (stockSlot != null)
    //        Gizmos.DrawWireCube(stockSlot.position, new Vector3(cardSize.x, cardSize.y, 0.1f));

    //    if (wasteSlot != null)
    //        Gizmos.DrawWireCube(wasteSlot.position, new Vector3(cardSize.x, cardSize.y, 0.1f));

    //    foreach (var f in foundationSlots)
    //    {
    //        if (f == null) continue;
    //        Gizmos.DrawWireCube(f.position, new Vector3(cardSize.x, cardSize.y, 0.1f));
    //    }

    //    foreach (var t in tableauSlots)
    //    {
    //        if (t == null) continue;
    //        Gizmos.DrawWireCube(t.position, new Vector3(cardSize.x, cardSize.y, 0.1f));
    //    }
    //}
}
