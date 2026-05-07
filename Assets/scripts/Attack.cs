using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Animator animator;
    InputAction attack;

    private bool isAttacking = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attack = playerInput.actions.FindAction("Attack");
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AttackMethod();
        OnAttackComplete();
    }

    private void AttackMethod()
    {
        if (attack.IsPressed() && isAttacking == true)
        {
            animator.SetBool("IsAttack", true);
            isAttacking = false;
        }
    }

    private void OnAttackComplete()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            isAttacking = true;
            animator.SetBool("IsAttack", false);
        }
    }
}
