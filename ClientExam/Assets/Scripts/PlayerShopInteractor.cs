using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShopInteractor : MonoBehaviour
{
    [SerializeField] private float interactRange = 2f;

    [SerializeField] private LayerMask merchantLayer;

    [SerializeField] private ShopUI shopUI;

    private Merchant nearestMerchant;
    private Merchant previousMerchant;

    private void Update()
    {
        FindNearestMerchant();
        UpdateMerchantText();
    }


    private void FindNearestMerchant()
    {
        Collider[] hits = Physics.OverlapSphere(
               transform.position,
               interactRange,
               merchantLayer);

        nearestMerchant = null;

        float nearestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            Merchant merchant =
                hit.GetComponentInParent<Merchant>();

            if (merchant == null)
                continue;

            float distance =
                Vector3.Distance(
                    transform.position,
                    merchant.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestMerchant = merchant;
            }
        }
    }

    private void UpdateMerchantText()
    {
        if (previousMerchant != null &&
            previousMerchant != nearestMerchant)
        {
            previousMerchant.HideInteractText();
        }

        if (nearestMerchant != null)
        {
            nearestMerchant.ShowInteractText();
        }
        else if (previousMerchant != null)
        {
            previousMerchant.HideInteractText();
        }

        previousMerchant = nearestMerchant;
    }
    
    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;

        if (nearestMerchant == null)
            return;

        shopUI.Open(nearestMerchant);
    }
}
