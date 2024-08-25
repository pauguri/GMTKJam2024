using System.Collections;
using UnityEngine;

public class Animal : HexGridObject
{
    private Coroutine moveCoroutine;

    public void MoveTo(Vector2Int to)
    {
        SetPosition(to);
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        moveCoroutine = StartCoroutine(MoveAnimation(to));
    }

    private IEnumerator MoveAnimation(Vector2Int to)
    {
        Vector2 fromPos = transform.anchoredPosition;
        Vector2 toPos = HexToAnchored(to);
        Vector2 movement = (toPos - fromPos) / 5;

        for (int i = 0; i < 5; i++)
        {
            transform.anchoredPosition += movement;
            yield return new WaitForSeconds(0.08f);
        }
    }
}
