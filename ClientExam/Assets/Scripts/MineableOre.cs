using ShatterStone;
using System.Collections;
using TMPro;
using UnityEngine;

public class MineableOre : MonoBehaviour
{
    [Header("Ore")]
    [SerializeField] private string oreName = "±¤¹°";
    [SerializeField] private float miningTime = 3f;
    [SerializeField] private float respawnTime = 10f;

    [Header("Drop")]
    [SerializeField] private OreDropData[] drops;
    [SerializeField] private float dropRadius = 1f;

    [Header("UI")]
    [SerializeField] private GameObject interactTextRoot;
    [SerializeField] private TextMeshProUGUI interactText;

    [Header("Visual")]
    [SerializeField] private GameObject visualRoot;
    [SerializeField] private Collider oreCollider;

   // private OreNode oreNode;

    public string OreName => oreName;
    public float MiningTime => miningTime;
    private void Awake()
    {
        //oreNode = GetComponent<OreNode>();
        HideInteractText();
    }
    private void Start()
    {
        RefreshInteractText();
    }

    private bool isMined;

    public void MineComplete()
    {
        if (isMined)
            return;

        isMined = true;

        SpawnDrops();
        StartCoroutine(RespawnRoutine());
    }

    public void SpawnDrops()
    {
        foreach (OreDropData drop in drops)
        {
            if (drop.dropItemPrefab == null)
                continue;

            for (int i = 0; i < drop.dropCount; i++)
            {
                Vector3 randomOffset = Random.insideUnitSphere * dropRadius;
                randomOffset.y = 0.3f;

                DropItem item = Instantiate(
                    drop.dropItemPrefab,
                    transform.position + randomOffset,
                    Quaternion.identity
                );

                int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);
                item.SetAmount(amount);
            }
        }

    }

    private IEnumerator RespawnRoutine()
    {
        SetOreActive(false);
        Debug.Log("Ã¤±¤ ¿Ï·á");
        yield return new WaitForSeconds(respawnTime);
        Debug.Log("±¤¹° º¹±¸");
        isMined = false;
        SetOreActive(true);
    }
    private void SetOreActive(bool active)
    {
        if (oreCollider != null)
            oreCollider.enabled = active;

        HideInteractText();

        StartCoroutine(OreShatter());

        if (visualRoot != null)
            visualRoot.SetActive(active);
    }

    private IEnumerator OreShatter()
    {
        //oreNode.Interact(1);
        yield return new WaitForSeconds(1f);
    }
    public void ShowInteractText()
    {
        RefreshInteractText();

        if (interactTextRoot != null)
            interactTextRoot.SetActive(true);
    }

    public void HideInteractText()
    {
        if (interactTextRoot != null)
            interactTextRoot.SetActive(false);
    }

    private void RefreshInteractText()
    {
        if (interactText != null)
            interactText.text = $"E : Mining {oreName}";
    }
}
