using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private PlayerController playerController;

    public bool IsOpen { get; private set; }

    private void Start()
    {
        inventoryPanel.SetActive(false);
        IsOpen = false;
    }

    public void OpenInventory(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        ToggleInventory();
    }

    public void ToggleInventory()
    {
        IsOpen = !IsOpen;
        inventoryPanel.SetActive(IsOpen);
        playerController.SetInputBlocked(IsOpen);
        if (IsOpen)
            inventoryManager.RefreshAfterOpen();
    }

    public void CloseInventory()
    {
        IsOpen = false;
        inventoryPanel.SetActive(false);
    }
}
