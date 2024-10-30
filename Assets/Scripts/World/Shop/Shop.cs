using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shop : InteractableObject
{
    public override string GetTooltip()
    {
        return "check out the shop";
    }

    public override void Interact()
    {
        if (SceneManager.loadedSceneCount > 1) return;

        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        SceneManager.LoadScene("Shop", LoadSceneMode.Additive);
    }

    public override void Use()
    {
        Debug.Log("Shop has no use");
    }

    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.B))
    //    {
    //        Time.timeScale = 0;
    //        Cursor.lockState = CursorLockMode.Confined;
    //        Cursor.visible = true;
    //        SceneManager.LoadScene("Shop", LoadSceneMode.Additive);
    //    }

    //    if(Input.GetKeyDown(KeyCode.C))
    //    {
    //        Time.timeScale = 1;
    //        Cursor.lockState = CursorLockMode.Locked;
    //        Cursor.visible = false;

    //        SceneManager.UnloadSceneAsync("Shop");
    //    }
    //}


}
