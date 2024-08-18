using UnityEngine;

public class CrushTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Player entered trigger " + name);
            ThreeDSceneLogic.Instance.HandleGetCrushed();
        }
    }
}
