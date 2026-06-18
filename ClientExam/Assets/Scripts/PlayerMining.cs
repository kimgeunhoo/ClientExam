using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMining : MonoBehaviour
{
    [Header("Mining")]
    [SerializeField] private float miningRange = 2f;
    [SerializeField] private LayerMask oreLayer;
    [SerializeField] private MiningProgressUI progressUI;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string miningBoolName = "IsMining";
    [SerializeField] private Transform playerVisual;
    private MineableOre nearestOre;
    private bool isMining;

    private Coroutine miningRoutine;
    private MineableOre previousNearestOre;

    public bool IsMining => isMining;
    private void Update()
    {
        if (!isMining)
            FindNearestOre();

        UpdateOreText();
    }

    private void UpdateOreText()
    {
        if (previousNearestOre != null && previousNearestOre != nearestOre)
            previousNearestOre.HideInteractText();

        if (nearestOre != null)
            nearestOre.ShowInteractText();
        else if (previousNearestOre != null)
            previousNearestOre.HideInteractText();

        previousNearestOre = nearestOre;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log($"OnInteract »£√‚µ : {context.phase}");
        if (!context.performed)
            return;
        if (isMining)
        {
            Debug.Log("¿ÃπÃ √§±§ ¡ﬂ");
            return;
        }

        if (nearestOre == null)
        {
            Debug.Log("√§±§ Ω«∆–: ±Ÿ√≥ ±§π∞ æ¯¿Ω");
            return;
        }

        Debug.Log($"√§±§ Ω√¿€: {nearestOre.name}");
        miningRoutine = StartCoroutine(MiningRoutine(nearestOre));
    }

    public void CancelMiningByInput()
    {
        if (!isMining)
            return;

        CancelMining();
    }

    private void FindNearestOre()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, miningRange, oreLayer);
        nearestOre = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            MineableOre ore = hit.GetComponentInParent<MineableOre>();
            if (ore == null)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, ore.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestOre = ore;
            }
        }

    }

    private IEnumerator MiningRoutine(MineableOre ore)
    {
        isMining = true;

        FaceOre(ore);

        if (animator != null)
            animator.SetBool(miningBoolName, true);

        if (progressUI != null)
            progressUI.Show();

        float elapsed = 0f;
        float duration = ore.MiningTime;

        while (elapsed < duration)
        {
            if (ore == null)
            {
                CancelMining();
                yield break;
            }

            float distance = Vector3.Distance(transform.position, ore.transform.position);
            if (distance > miningRange)
            {
                CancelMining();
                yield break;
            }

            elapsed += Time.deltaTime;

            if (progressUI != null)
                progressUI.SetProgress(elapsed / duration);

            yield return null;
        }

        if(animator != null)
            animator.SetBool(miningBoolName, false);

        if (progressUI != null)
        {
            progressUI.Hide();
        }

        ore.MineComplete();
        ore.HideInteractText();

        nearestOre = null;
        previousNearestOre = null;
        miningRoutine = null;
        isMining = false;
    }

    private void FaceOre(MineableOre ore)
    {
        Vector3 dir = ore.transform.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f)
            return;

        playerVisual.rotation = Quaternion.LookRotation(dir);
    }

    private void CancelMining()
    {
        if (miningRoutine != null)
        {
            StopCoroutine(miningRoutine);
            miningRoutine = null;
        }

        if (animator != null)
            animator.SetBool(miningBoolName, false);

        if (progressUI != null)
            progressUI.Hide();
        isMining = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, miningRange);
    }
}
