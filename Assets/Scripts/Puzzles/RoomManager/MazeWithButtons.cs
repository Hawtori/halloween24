using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeWithButtons : PuzzleRoom
{
    private List<int> buttonOrder = new List<int>();
    private List<int> buttonsPressed = new List<int>();

    protected override void SetupPuzzle()
    {

    }

    public override bool CheckForCompletion()
    {
        if (buttonOrder.Count != buttonsPressed.Count) return false;

        for(int i = 0; i < buttonOrder.Count; i++)
        {
            if (buttonsPressed[i] != buttonOrder[i]) return false;
        }

        return true;
    }
}
