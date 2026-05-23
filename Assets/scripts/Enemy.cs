using UnityEngine;
public class Enemy : MonoBehaviour

{
    [Header("Patrol")]
    [SerializeField] Transform[] wayPoints;
    [SerializeField] protected float moveSpeed = 3f;

    private int patrolDestination = 0;

    [Header("State and animation")]
    [SerializeField] protected Animator animator;
    public bool isAlive = true;

    [Header("Raycast Detection")]
    [SerializeField] private float rayDistance = 5f;
    private Vector3 rayOffSet = Vector3.up * 1.5f;
    private Color rayColor = Color.red;
    private RaycastHit rayHit;
    protected bool MoveToPlayer = false;
    [SerializeField] protected GameObject player;

    [Header("Combat (Прості Таймери)")]
    [SerializeField] protected BoxCollider attackCollider;
    [SerializeField] protected float attackCooldown = 2.5f;
    [SerializeField] protected float delayBeforeHit = 0.6f;
    [SerializeField] protected float hitDuration = 0.5f;
    [SerializeField] protected float attackDistance = 1.8f;

    protected float nextAttackTime = 0f;
    protected float hitEnableTime = 0f;
    protected float hitDisableTime = 0f;
    protected float attackEndTime = 0f;

    protected bool isAttacking = false;
    protected bool hasAttackedThisCycle = false;

    protected virtual void Start()
    {
        isAlive = true;

        if (animator == null) animator = GetComponent<Animator>();
        if (attackCollider == null) attackCollider = GetComponent<BoxCollider>();
        if (attackCollider != null) attackCollider.enabled = false;
    }

    protected virtual void Update()
    {
        if (!isAlive) return;

        DetectPlayer();
        HandleAttackTimers();

        if (isAttacking)
        {
            if (player != null)
            {
                Vector3 direction = (player.transform.position - transform.position).normalized;

                if (direction.x != 0)
                {
                    float targetAngle = (direction.x < 0f) ? -90f : 90f;
                    transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
                }
            }
            return;
        }

        if (!MoveToPlayer)
        {
            Patrol();
        }
        else
        {
            FollowPlayer();
        }
    }

    private void Patrol()
    {
        if (wayPoints == null || wayPoints.Length < 2) return;

        Transform targetPoint = wayPoints[patrolDestination];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        Vector3 direction = (targetPoint.position - transform.position).normalized;
        if (direction.x != 0)
        {
            float targetAngle = (direction.x < 0f) ? -90f : 90f;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }

        if (animator != null) animator.SetFloat("Speed", 0.5f);
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.5f)
        {
            patrolDestination = (patrolDestination == 0) ? 1 : 0;
        }
    }

    private void DetectPlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= rayDistance)
        {
            MoveToPlayer = true;
        }
    }

    protected virtual void FollowPlayer()
    {
        if (player == null || !isAlive) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= attackDistance)
        {
            if (animator != null) animator.SetFloat("Speed", 0f);
            if (Time.time >= nextAttackTime)
            {
                StartAttack();
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
            if (animator != null) animator.SetFloat("Speed", 1f);

            Vector3 direction = (player.transform.position - transform.position).normalized;
            if (direction.x != 0)
            {
                float targetAngle = (direction.x < 0f) ? -90f : 90f;
                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            }
        }
    }

    protected void StartAttack()
    {
        isAttacking = true;
        hasAttackedThisCycle = false;

        hitEnableTime = Time.time + delayBeforeHit;
        hitDisableTime = hitEnableTime + hitDuration;
        attackEndTime = Time.time + delayBeforeHit + hitDuration + 0.4f;
        nextAttackTime = Time.time + attackCooldown;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    protected void HandleAttackTimers()
    {
        if (!isAttacking) return;
        if (Time.time >= hitEnableTime && attackCollider != null && !attackCollider.enabled && !hasAttackedThisCycle)
        {
            attackCollider.enabled = true;
        }

        if (Time.time >= hitDisableTime && attackCollider != null && attackCollider.enabled)
        {
            attackCollider.enabled = false;
        }

        if (Time.time >= attackEndTime)
        {
            isAttacking = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (hasAttackedThisCycle) return;
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(15f);
                hasAttackedThisCycle = true;
            }
        }
    }

    public virtual void GetHit()
    {
        isAlive = false;
        if (attackCollider != null) attackCollider.enabled = false;
        if (animator != null) animator.SetTrigger("Death");

        CapsuleCollider collider = GetComponent<CapsuleCollider>();

        if (collider != null) collider.enabled = false;
    }

    public virtual void AttackCollisionEnabled() { }

    public virtual void AttackCollisionDisabled() { }

}