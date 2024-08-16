using UnityEngine;

public class Animal : MonoBehaviour
{
    private new RectTransform transform;
    public Vector2 startPosition = Vector2.zero;

    private void Awake()
    {
        transform = GetComponent<RectTransform>();
    }

    void Start()
    {
        transform.anchoredPosition = startPosition;
    }

    void Update()
    {

    }

    public void HopTo(Vector2 position)
    {
        transform.anchoredPosition = position;
    }
}
