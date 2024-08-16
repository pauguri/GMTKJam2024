using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    private new RectTransform transform;

    [NonSerialized] public int x;
    [NonSerialized] public int y;

    [NonSerialized] public Board board;
    [NonSerialized] public bool blocked = false;

    [NonSerialized] public int distanceToEdge = 0;
    [NonSerialized] public int possibleRoutes;
    [NonSerialized] public float score;

    void Awake()
    {
        transform = GetComponent<RectTransform>();

        x = Mathf.CeilToInt(transform.anchoredPosition.x / transform.rect.width);
        y = Mathf.CeilToInt(transform.parent.GetComponent<RectTransform>().anchoredPosition.y / (transform.rect.height * 0.8f));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //float distanceFromCenter = Vector2.Distance(eventData.position, transform.position);
        //if (distanceFromCenter < transform.rect.width / 2)
        //{
        //    print("Clicked on cell " + gameObject.name);
        //}

        if (!blocked)
        {
            print("Clicked on cell " + gameObject.name);
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
