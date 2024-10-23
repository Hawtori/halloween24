using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PuzzleRoom : MonoBehaviour
{
    public List<GameObject> puzzleObjects = new List<GameObject>();
    public List<GameObject> puzzleObjectPositions = new List<GameObject>();
    protected bool isSolved = false;

    protected virtual void Start()
    {
        InitRoom();
        SetupPuzzle();
    }

    protected void InitRoom()
    {
        foreach(GameObject puzzleItem in puzzleObjects)
        {
            // raycast from center of room to a random point on the wall
            // or have a list of set positions where the hints can be and pick from there
        }
    }

    protected abstract void SetupPuzzle();
    public abstract bool CheckForCompletion();
}
