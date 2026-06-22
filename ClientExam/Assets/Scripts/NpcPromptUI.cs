using TMPro;
using UnityEngine;

public class NpcPromptUI : MonoBehaviour
{
    [SerializeField] private GameObject promptRoot;
    [SerializeField] private TextMeshProUGUI promptText;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        Hide();
    }

    private void LateUpdate()
    {
        if (promptRoot != null && !promptRoot.activeSelf)
            return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        Vector3 dir = transform.position - mainCamera.transform.position;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    public void Show(string text)
    {
        if (promptRoot != null)
            promptRoot.SetActive(true);

        if (promptText != null)
            promptText.text = text;
    }

    public void Hide()
    {
        if (promptRoot != null)
            promptRoot.SetActive(false);
    }
}
