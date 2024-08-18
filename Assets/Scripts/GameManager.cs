using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [NonSerialized] public List<Vector2Int> blockedCells = new List<Vector2Int>();
    [NonSerialized] public List<Vector2Int> clickedCells = new List<Vector2Int>();
    [NonSerialized] public List<Vector2Int> surroundedCells = new List<Vector2Int>();
    [NonSerialized] public int round = 0;

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

    private void Update()
    {
        // TODO: make it two clicks to quit
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    Application.Quit();
        //}
    }

    public void Start2DPhase()
    {
        SceneManager.LoadScene("2DScene");
    }

    public void Start3DIntro()
    {

        SceneManager.LoadScene("3DIntroScene");
    }

    public void Start3DPhase()
    {
        SceneManager.LoadScene("3DScene");
    }

    public void StartEnding()
    {
        round = 0;
        SceneManager.LoadScene("EndingScene");
    }
}
