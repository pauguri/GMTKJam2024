using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : HexGridObject, IPointerClickHandler
{
    [NonSerialized] public Board board;
    [NonSerialized] public bool blocked = false;
    [NonSerialized] public bool occupied = false;

    [NonSerialized] public int distanceToEdge = 0;
    //[NonSerialized] public int possibleRoutes;
    //public float Score => distanceToEdge > 0 ? possibleRoutes / distanceToEdge : -1;
    // public float Score => distanceToEdge > 0 ? 1f / distanceToEdge : -1;

    private TextMeshProUGUI debugText;

    public override void Awake()
    {
        base.Awake();

        // debugText = GetComponentInChildren<TextMeshProUGUI>();
        x = Mathf.CeilToInt(transform.anchoredPosition.x / 100);
        y = Mathf.CeilToInt(transform.parent.GetComponent<RectTransform>().anchoredPosition.y / 85);
    }

    private void Update()
    {
        // debugText.text = "b " + blocked + "\no " + occupied;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //float distanceFromCenter = Vector2.Distance(eventData.position, transform.position);
        //if (distanceFromCenter < transform.rect.width / 2)
        //{
        //    print("Clicked on cell " + gameObject.name);
        //}

        if (!blocked && !occupied)
        {
            SetBlocked(true);

            GameManager.Instance.clickedCells.Add(Position);
            board.CalculatePathfinding();
        }
    }

    public void SetBlocked(bool state)
    {
        blocked = state;

        Image image = GetComponent<Image>();
        image.color = state ? Color.red : Color.white;
        image.raycastTarget = !state;
    }
}
