using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class PerPlatformText : MonoBehaviour
{
    [SerializeField][TextArea] private string desktopText;
    [SerializeField][TextArea] private string webText;

    void Start()
    {
#if UNITY_STANDALONE_WIN
        GetComponent<TextMeshProUGUI>().text = desktopText;
#else
        GetComponent<TextMeshProUGUI>().text = webText;
#endif
    }
}
