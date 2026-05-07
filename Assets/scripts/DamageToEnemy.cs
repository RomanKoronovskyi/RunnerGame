using UnityEngine;

public class DamageToEnemy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy.isAlive)
            {
                enemy.GetHit();
            }
        }
    }
}
