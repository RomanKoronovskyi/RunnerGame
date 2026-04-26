using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] Slider healthSlider;

    private float maxHealthe = 100f;
    private float currentHealthe;

    private void Start()
    {
        currentHealthe = maxHealthe;
        healthSlider.value = currentHealthe;
    }

    public void TakeDamage(float damage)
    {
        currentHealthe -= damage;

        if (currentHealthe <= 0f)
        {
            currentHealthe = 0f;
        }

        healthSlider.value = currentHealthe;
    }

    public void AddHealth(float health)
    {
        currentHealthe += health;

        if(currentHealthe >= 100f)
        {
            currentHealthe = 100f;
        }

        healthSlider.value = currentHealthe;
    }
}
