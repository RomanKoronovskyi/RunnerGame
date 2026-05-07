using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Patrool")]
    [SerializeField] Transform[] wayPoints;
    [SerializeField] float moveSpeed;
    private int patrolDestination;

    [Header("State and animation")]
    [SerializeField] Animator animator;
    public bool isAlive = true;

    [Header("Ray")]
    private float rayDistance = 0.5f;
    private Vector3 rayOffSet = Vector3.up * 1.5f;
    private Color rayColor = Color.red;
    private RaycastHit rayHit;
    bool MoveToPlayer = false;
    [SerializeField] GameObject player;

    [SerializeField] BoxCollider attackCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isAlive = true;
        transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        attackCollider = GetComponent<BoxCollider>();
        attackCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (MoveToPlayer == false && isAlive == true)
        {
            Patrol();
        }
        else
        {
            FollowPlayer();
        }
        DetectPlayer();
    }
    private void Patrol()
    {
        if (patrolDestination == 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, wayPoints[0].position, moveSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, wayPoints[0].position) < 2f)
            {
                transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                patrolDestination = 1;
            }
        }

        if (patrolDestination == 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, wayPoints[1].position, moveSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, wayPoints[1].position) < 2f)
            {
                transform.rotation = Quaternion.Euler(0f, -90f, 0f);
                patrolDestination = 0;
            }
        }
    }
    private void DetectPlayer()
    {
        Vector3 rayStart = transform.position + rayOffSet;
        Vector3 rayDirection = transform.forward;

        Debug.DrawRay(rayStart, rayDirection * rayDistance, rayColor);

        if (Physics.Raycast(rayStart, rayDirection, out rayHit, rayDistance))
        {
            if (rayHit.collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("faced");
                MoveToPlayer = true;
            }
        }
        else
        {
            MoveToPlayer = false;
        }
    }
    private void FollowPlayer()
    {
        if (player == null || !isAlive) return;
        if (player != null && isAlive == true)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
        }
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= 1f){
            animator.SetTrigger("Attack");
        }
        else
        {
            Patrol();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(15f);
            }
        }
    }
    public void GetHit()
    {
        isAlive = false;
        animator.SetTrigger("Death");

        CapsuleCollider collider = GetComponent<CapsuleCollider>();
        collider.enabled = false;
    }
}
