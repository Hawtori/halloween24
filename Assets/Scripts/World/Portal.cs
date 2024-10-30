using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : InteractableObject
{
    public override string GetTooltip()
    {
        return "go to the next level";
    }

    public override void Interact()
    {
        Invoke(nameof(GoNextLevel), 0.25f);
    }

    private void GoNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public override void Use()
    {
        throw new System.NotImplementedException();
    }
}
