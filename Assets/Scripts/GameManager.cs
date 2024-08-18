using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [NonSerialized] public List<Vector2Int> blockedCells = new List<Vector2Int>();
    [NonSerialized] public List<Vector2Int> clickedCells = new List<Vector2Int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start2DPhase()
    {
        SceneManager.LoadScene("2DScene");
        blockedCells.Clear();
        clickedCells.Clear();
    }

    public void Start3DIntro()
    {

        SceneManager.LoadScene("3DIntroScene");
    }

    public void Start3DPhase()
    {
        SceneManager.LoadScene("3DScene");
    }
}
