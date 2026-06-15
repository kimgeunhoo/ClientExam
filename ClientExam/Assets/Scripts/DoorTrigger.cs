using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private DoorController door;

    private void Awake()
    {
        if (door == null)
            door = GetComponentInParent<DoorController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            door.Open();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.Close();
        }
    }
}
