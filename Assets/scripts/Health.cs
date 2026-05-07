using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] Slider healthSlider;
    [SerializeField] Animator animator;
    [SerializeField] PlayerInput playerInput;

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

            playerInput.enabled = false;
            animator.SetBool("IsDeadth", true);

            Invoke("Respawn", 3f);
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

    private void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
