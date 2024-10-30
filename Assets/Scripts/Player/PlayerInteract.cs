using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField]
    private InputActionAsset playerControls;

    private InputAction interactAction;

    [Header("Interaction")]
    [SerializeField]
    private LayerMask interactableLayer;
    [SerializeField]
    private float interactableRange = 3f;

    private bool interactable = false;

    private void Awake()
    {
        interactAction = playerControls.FindActionMap("Interact").FindAction("Interact");
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactableRange, interactableLayer))
        {
            InteractableObject objectInFrontOfUs;
            hit.transform.TryGetComponent(out objectInFrontOfUs);
            if (objectInFrontOfUs)
            {
                interactable = true;
                HUDManager.instance.UpdateTextPrompt(objectInFrontOfUs.GetTooltip());
            }
        }
        else if (interactable)
        {
            HUDManager.instance.HidePrompt();
            interactable = false;
        }
    }

    private void OnEnable()
    {
        interactAction.Enable();
        interactAction.performed += ctx => Interact();
    }

    private void OnDisable()
    {
        interactAction.Disable();
        interactAction.performed -= ctx => Interact();
    }

    private void Interact()
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactableRange, interactableLayer))
        {
            InteractableObject interactableObject;
            hit.transform.TryGetComponent(out interactableObject);
            if (interactableObject)
                interactableObject.Interact();
            else
                Debug.Log($"Can not interact with {hit.transform.name}");

            return;
        }

        Debug.Log("Nothing here to interact with");
    }
}
