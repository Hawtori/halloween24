using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PuzzleRoom : MonoBehaviour
{
    public List<GameObject> puzzleObjects = new List<GameObject>();
    public List<GameObject> hintObjects = new List<GameObject>();
    public List<GameObject> puzzleObjectPositions = new List<GameObject>();

    public List<GameObject> roomsWeCanWorkWith = new List<GameObject>();


    protected bool isSolved = false;

    protected virtual void Start()
    {
        InitRoom();
        SetupPuzzle();
    }

    protected abstract void InitRoom();
    protected abstract void SetupPuzzle();
    public abstract bool CheckForCompletion();
}
