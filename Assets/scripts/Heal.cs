using UnityEngine;

public class Heal : MonoBehaviour
{
    [SerializeField] float heal;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.AddHealth(heal);
            }
            Destroy(gameObject);
        }
    }
}
