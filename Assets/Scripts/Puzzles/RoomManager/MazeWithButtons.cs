using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeWithButtons : PuzzleRoom
{
    List<int> buttonsPressed = new List<int>();
    List<int> unAvailableButtons = new List<int>();

    List<GameObject> roomsToSpawnButtonsIn = new List<GameObject>();
    List<GameObject> roomsToSpawnEnemiesIn = new List<GameObject>();

    List<GameObject> listOfButtonsInGame = new List<GameObject>();

    char[] buttons;
      
    private int howManyButtons = 0;

    protected override void InitRoom()
    {
        Random.InitState(System.DateTime.Now.Millisecond);

        for (int i = 0; i < puzzleObjects.Count; i++)
        {
            int index = Random.Range(1, roomsWeCanWorkWith.Count-1);
            if (roomsToSpawnButtonsIn.Contains(roomsWeCanWorkWith[index]))
            {
                i--;
                continue;
            }
            roomsToSpawnButtonsIn.Add(roomsWeCanWorkWith[index]);
            howManyButtons++;
        }

        for (int i = 0; i < puzzleEnemies.Count; i++)
        {
            int maxClamp = Mathf.Min(5, roomsWeCanWorkWith.Count);
            int index = Random.Range(1, roomsWeCanWorkWith.Count - maxClamp);
            if (roomsToSpawnEnemiesIn.Contains(roomsWeCanWorkWith[index]))
            {
                i--;
                continue;
            }
            roomsToSpawnEnemiesIn.Add(roomsWeCanWorkWith[index]);
        }
    }

    protected override void SetupPuzzle()
    {
        Debug.Log("This level has a maze puzzle");

        buttons = new char[puzzleObjects.Count];

        // put buttons around map
        for(int i = 0; i < puzzleObjects.Count; i++)
        {
            GameObject btn = Instantiate(puzzleObjects[i], roomsToSpawnButtonsIn[i].transform);
            
            btn.transform.position = btn.transform.position + Vector3.up * 0.001f;
            listOfButtonsInGame.Add(btn);
        }

        for(int i = 0; i < listOfButtonsInGame.Count; i++)
        {
            Debug.Log($"{listOfButtonsInGame[i].GetComponentInChildren<MeshRenderer>().material.name} is in {listOfButtonsInGame[i].GetComponentInChildren<PressMazeButton>().placeItShouldBeIn}");
            buttons[listOfButtonsInGame[i].GetComponentInChildren<PressMazeButton>().placeItShouldBeIn] = listOfButtonsInGame[i].GetComponentInChildren<MeshRenderer>().material.name[0];
        }

        // set up hint

        string res = "";
        for(int i = 0; i < buttons.Length; i++)
        {
            Debug.Log($"{i} is {buttons[i]}");
            res += buttons[i] + " ";
        }

        // spawn hint
        Instantiate(hintObjects[0], Vector3.right * 5f + Vector3.up * 2f + Vector3.back * 6f, hintObjects[0].transform.rotation, WorldGeneration.Instance.startingRoom.transform);
        Hint.Instance.SetHintText(res);

        // spawn enemies
        for (int i = 0; i < roomsToSpawnEnemiesIn.Count; i++)
        {
            Instantiate(puzzleEnemies[i], roomsToSpawnEnemiesIn[i].transform);
        }
    }

    public override bool CheckForCompletion()
    {
        if (buttonsPressed.Count < howManyButtons) return false;

        for(int i = 1; i < buttonsPressed.Count; i++)
        {
            if (buttonsPressed[i - 1] > buttonsPressed[i]) return false;
        }

        return true;
    }

    public void ButtonPressed(int order)
    {
        buttonsPressed.Add(order);

        if(CheckForCompletion())
        {
            isSolved = true;

            Inventory.Instance.PuzzleSolved();

            HUDManager.instance.UpdateText("SUCCESS!");

            foreach (var btn in listOfButtonsInGame)
            {
                Destroy(btn.GetComponentInChildren<MazeButtonObject>());
            }
        }
        else if (buttonsPressed.Count == howManyButtons)
        {
            HUDManager.instance.UpdateText("Wrong Order of buttons!");

            foreach(var btn in listOfButtonsInGame)
            {
                if (btn.GetComponentInChildren<MazeButtonObject>() != null)
                {
                    btn.GetComponentInChildren<MazeButtonObject>().gameObject.layer = LayerMask.NameToLayer("Interactable");
                    btn.GetComponentInChildren<MazeButtonObject>().ResetButton();
                }
                else Debug.Log($"{btn.name} does not have a maze button object component");
            }
        }
    }

    public int GetValidButton()
    {
        int rand = -1;

        for (int i = 0; i < 1; i++)
        {
            rand = Random.Range(0, GetTotalButtons());
            if (unAvailableButtons.Contains(rand))
            {
                i--;
                continue;
            }
            unAvailableButtons.Add(rand);
        }
        return rand;
    }

    public void ResetList()
    {
        buttonsPressed.Clear();
    }

    public int GetTotalButtons() => howManyButtons;
}
