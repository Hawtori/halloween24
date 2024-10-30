using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class PuzzleRoom : MonoBehaviour
{
    public List<GameObject> puzzleObjects = new List<GameObject>();
    public List<GameObject> hintObjects = new List<GameObject>();
    public List<GameObject> puzzleObjectPositions = new List<GameObject>();
    public List<GameObject> puzzleEnemies = new List<GameObject>();

    public List<GameObject> roomsWeCanWorkWith = new List<GameObject>();


    protected bool isSolved = false;

    protected virtual void Start()
    {
        InitRoom();
        if (roomsWeCanWorkWith.Count < 2) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SetupPuzzle();
    }

    protected abstract void InitRoom();
    protected abstract void SetupPuzzle();
    public abstract bool CheckForCompletion();
}
