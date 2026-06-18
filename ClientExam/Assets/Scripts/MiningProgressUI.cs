using UnityEngine;
using UnityEngine.UI;

public class MiningProgressUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Image fillImage;

    private void Awake()
    {
        Hide();
    }
    public void Show()
    {
        if (root != null)
            root.SetActive(true);

        SetProgress(0f);
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);
    }

    public void SetProgress(float value)
    {
        if (fillImage != null)
            fillImage.fillAmount = Mathf.Clamp01(value);
    }
}
