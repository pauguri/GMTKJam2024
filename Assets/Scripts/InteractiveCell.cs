using UnityEngine.EventSystems;

public class InteractiveCell : Cell, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (TwoDSceneLogic.Instance.enableInput && !blocked && !occupied)
        {
            SetBlocked(true);
            TwoDSceneLogic.Instance.HandleAnimalTurn();
        }
    }
}
