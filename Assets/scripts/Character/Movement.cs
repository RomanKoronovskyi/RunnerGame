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

    [Header("Ground Alignment (Фікс нерівної підлоги)")]
    [SerializeField] private LayerMask groundLayer; // Сюди в інспекторі виберемо шар Ground
    [SerializeField] private float rayDistanceDown = 3f; // Довжина променя вниз

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

        // Базове зміщення
        float horizontalMove = direction.x * speed * Time.deltaTime;
        float verticalMove = verticalVelocity * Time.deltaTime;

        // 1. Поворот персонажа (над переміщенням)
        if (direction.x != 0)
        {
            float targetAngle = (direction.x < 0f) ? -90f : 90f;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, targetAngle, 0), Time.deltaTime * 15f);
        }

        // 2. Рейкаст-радар (суто для зупинки анімації бігу біля стіни)
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

        // 3. Зсуваємо персонажа через transform
        transform.position += new Vector3(horizontalMove, verticalMove, 0f);

        // --- НОВИЙ БЛОК: ВИРІВНЮВАННЯ ВИСОТИ ПО НЕРІВНІЙ ЗЕМЛІ ---
        AlignWithGround();

        // 4. ЗАХИСТ ВІД ПРОХОДЖЕННЯ (З виправленням польоту)
        CapsuleCollider myCollider = GetComponent<CapsuleCollider>();
        if (myCollider != null)
        {
            // Створюємо маску фільтру: беремо ВСІ шари, ОКРІМ шару "Ground"
            // Завдяки цьому земля ніколи не штовхатиме персонажа вгору
            int layerMask = ~LayerMask.GetMask("Ground");

            // Шукаємо колайдери навколо, використовуючи фільтр шарів (layerMask)
            Collider[] overlappedColliders = Physics.OverlapCapsule(
                transform.position + Vector3.up * myCollider.radius,
                transform.position + Vector3.up * (myCollider.height - myCollider.radius),
                myCollider.radius,
                layerMask
            );

            foreach (var wallCollider in overlappedColliders)
            {
                // Игнорируем самого себя та тригеры
                if (wallCollider == myCollider || wallCollider.isTrigger) continue;

                Vector3 directionToPush;
                float distanceToPush;

                // Вираховуємо пересікання
                if (Physics.ComputePenetration(
                    myCollider, transform.position, transform.rotation,
                    wallCollider, wallCollider.transform.position, wallCollider.transform.rotation,
                    out directionToPush, out distanceToPush
                ))
                {
                    directionToPush.y = 0f;
                    directionToPush = directionToPush.normalized;

                    // Виштовхуємо лицаря назад зі стіни
                    transform.position += directionToPush * distanceToPush;
                }
            }
        }

        // Аніматор
        float currentSpeed = (direction == Vector2.zero) ? 0f : 0.4f;
        animator.SetFloat("Speed", currentSpeed, 0.01f, Time.deltaTime);
    }

    // Метод, який зчитує рельєф під ногами і змінює висоту Y
    private void AlignWithGround()
    {
        // Початок променя піднімаємо на 1 метр вгору від центру об'єкта (приблизно район колін/пояса)
        Vector3 rayStart = transform.position + Vector3.up * 1f;
        RaycastHit hit;

        // Малюємо жовту лінію в редакторі, щоб ви бачили роботу лазера вниз
        Debug.DrawRay(rayStart, Vector3.down * rayDistanceDown, Color.yellow);

        // Стріляємо строго вниз і реагуємо ТІЛЬКИ на шар groundLayer
        if (Physics.Raycast(rayStart, Vector3.down, out hit, rayDistanceDown, groundLayer))
        {
            // hit.point.y — це точна висота 3D-моделі землі в цій точці
            float targetY = hit.point.y;

            // Примусово фіксуємо позицію гравця на цій висоті
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