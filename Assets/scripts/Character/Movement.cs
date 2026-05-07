using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Movement : MonoBehaviour
{
    PlayerInput player_input;
    Animator animator;
    InputAction moveAction;
    InputAction attackAction;

    [SerializeField] float speed = 5f;

    [Header("Combat")]
    [SerializeField] GameObject axeHitbox;
    [SerializeField] float attackDuration = 0.5f;

    void Start()
    {
        player_input = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        moveAction = player_input.actions.FindAction("Move");
        attackAction = player_input.actions.FindAction("Attack");

        if (axeHitbox != null)
        {
            axeHitbox.GetComponent<Collider>().enabled = false;
        }
    }

    void Update()
    {
        MovePlayer();
        HandleAttack();
    }

    void MovePlayer()
    {
        Vector2 direction = moveAction.ReadValue<Vector2>();
        Vector3 movement = new Vector3(direction.x, direction.y) * speed * Time.deltaTime;
        transform.position += movement;

        if (direction.x != 0)
        {
            float targetAngle = (direction.x < 0f) ? -90f : 90f;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, targetAngle, 0), Time.deltaTime * 15f);
        }

        float currentSpeed = (direction == Vector2.zero) ? 0f : 0.4f;
        animator.SetFloat("Speed", currentSpeed, 0.01f, Time.deltaTime);
    }

    void HandleAttack()
    {
        if (attackAction != null && attackAction.WasPressedThisFrame())
        {
            animator.SetTrigger("IsAttack");
            StopAllCoroutines();
            StartCoroutine(ActivateAxeCollider());
        }
    }

    IEnumerator ActivateAxeCollider()
    {
        if (axeHitbox == null) yield break;
        axeHitbox.GetComponent<Collider>().enabled = true;
        yield return new WaitForSeconds(attackDuration);
        axeHitbox.GetComponent<Collider>().enabled = false;
    }
}