using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Transform[] doorModel;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 5f;

    private Quaternion closedRot;
    private Quaternion openedRot;
    private bool isOpen;

    private void Awake()
    {
        if (doorModel == null)
        {
            for (int i = 0; i < doorModel.Length; i++)
            {
                doorModel[i] = transform;
            }
        }
        for (int i = 0; i < doorModel.Length; i++)
        {
            closedRot = doorModel[i].localRotation;
            openedRot = closedRot * Quaternion.Euler(0f, openAngle, 0f);
        }        
    }

    private void Update()
    {
        Quaternion targetRot = isOpen ? openedRot : closedRot;

        for (int i = 0; i < doorModel.Length; i++)
        {
            doorModel[i].localRotation = Quaternion.Lerp(
                  doorModel[i].localRotation,
                  targetRot,
                  Time.deltaTime * openSpeed
            );
        }

    }
    public void Open()
    {
        isOpen = true;
    }

    public void Close()
    {
        isOpen = false;
    }
}
