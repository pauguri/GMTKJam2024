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
        transform.anchoredPosition = new Vector2(x * 100 + (y % 2 == 0 ? 25 : -25), y * 80);
    }

    public void SetPosition(Vector2Int position)
    {
        SetPosition(position.x, position.y);
    }
}
