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
    private float verticalVelocity = 0f;

    [Header("Combat")]
    [SerializeField] GameObject axeHitbox;
    [SerializeField] float attackDuration = 0.5f;

    [Header("Ground Alignment")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rayDistanceDown = 3f;

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

        float horizontalMove = direction.x * speed * Time.deltaTime;
        float verticalMove = verticalVelocity * Time.deltaTime;

        if (direction.x != 0)
        {
            float targetAngle = (direction.x < 0f) ? -90f : 90f;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, targetAngle, 0), Time.deltaTime * 15f);
        }

        bool isWallAhead = false;
        if (direction.x != 0)
        {
            Vector3 rayStart = transform.position + Vector3.up * 0.6f;
            Vector3 rayDirection = new Vector3(direction.x, 0f, 0f).normalized;
            float checkDistance = 0.45f;

            if (Physics.Raycast(rayStart, rayDirection, checkDistance))
            {
                isWallAhead = true;
            }
        }

        if (isWallAhead)
        {
            horizontalMove = 0f;
        }

        transform.position += new Vector3(horizontalMove, verticalMove, 0f);

        AlignWithGround();

        CapsuleCollider myCollider = GetComponent<CapsuleCollider>();
        if (myCollider != null)
        {
            int layerMask = ~LayerMask.GetMask("Ground");

            Collider[] overlappedColliders = Physics.OverlapCapsule(
                transform.position + Vector3.up * myCollider.radius,
                transform.position + Vector3.up * (myCollider.height - myCollider.radius),
                myCollider.radius,
                layerMask
            );

            foreach (var wallCollider in overlappedColliders)
            {
                if (wallCollider == myCollider || wallCollider.isTrigger) continue;

                Vector3 directionToPush;
                float distanceToPush;

                if (Physics.ComputePenetration(
                    myCollider, transform.position, transform.rotation,
                    wallCollider, wallCollider.transform.position, wallCollider.transform.rotation,
                    out directionToPush, out distanceToPush
                ))
                {
                    directionToPush.y = 0f;
                    directionToPush = directionToPush.normalized;
                    transform.position += directionToPush * distanceToPush;
                }
            }
        }

        float currentSpeed = (direction == Vector2.zero) ? 0f : 0.4f;
        animator.SetFloat("Speed", currentSpeed, 0.01f, Time.deltaTime);
    }

    private void AlignWithGround()
    {
        Vector3 rayStart = transform.position + Vector3.up * 1f;
        RaycastHit hit;

        Debug.DrawRay(rayStart, Vector3.down * rayDistanceDown, Color.yellow);

        if (Physics.Raycast(rayStart, Vector3.down, out hit, rayDistanceDown, groundLayer))
        {
            float targetY = hit.point.y;
            transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
        }
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