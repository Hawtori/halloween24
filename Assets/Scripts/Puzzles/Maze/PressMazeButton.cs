using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressMazeButton : MonoBehaviour
{
    public int placeItShouldBeIn;

    MazeWithButtons master;

    private void Awake()
    {
        master = GameObject.FindGameObjectWithTag("Manager").GetComponent<MazeWithButtons>();
        placeItShouldBeIn = master.GetValidButton();
    }

    public void AddButtonToList()
    {
        master.ButtonPressed(placeItShouldBeIn);
    }

    public void ResetList()
    {
        master.ResetList();
    }

    private int GetPlace()
    {
        return placeItShouldBeIn;
    }

}
