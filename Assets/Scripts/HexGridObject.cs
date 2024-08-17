using System;
using UnityEngine;

public class HexGridObject : MonoBehaviour
{
    [NonSerialized] public new RectTransform transform;

    [NonSerialized] public int x = 0;
    [NonSerialized] public int y = 0;
    public Vector2Int Position => new Vector2Int(x, y);

    public virtual void Awake()
    {
        transform = GetComponent<RectTransform>();
    }

    public void SetPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetPosition(Vector2Int position)
    {
        SetPosition(position.x, position.y);
    }

    public static Vector2 HexToAnchored(int x, int y)
    {
        return new Vector2(x * 100 + (y % 2 == 0 ? 25 : -25), y * 85);
    }
    public static Vector2 HexToAnchored(Vector2Int position)
    {
        return HexToAnchored(position.x, position.y);
    }

    public static Vector2Int AnchoredToHex(float x, float y)
    {
        int hexX = Mathf.CeilToInt(x / 100);
        int hexY = Mathf.CeilToInt(y / 85);
        return new Vector2Int(hexX, hexY);
    }
    public static Vector2Int AnchoredToHex(Vector2 position)
    {
        return AnchoredToHex(position.x, position.y);
    }
}
