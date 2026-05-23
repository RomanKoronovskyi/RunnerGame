using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyBoss : Enemy
{
    [Header("Boss Stats")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float raceDistance = 10f;
    [SerializeField] private Slider enemySlider;

    protected override void Start()
    {
        base.Start();

        isAlive = true;
        if (enemySlider != null)
        {
            enemySlider.maxValue = health;
            enemySlider.value = health;
        }
    }

    protected override void Update()
    {
        if (!isAlive || player == null) return;

        HandleAttackTimers();

        if (isAttacking)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            if (direction.x != 0)
            {
                float targetAngle = (direction.x < 0f) ? -90f : 90f;
                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            }
            return;
        }

        FollowPlayer();
    }

    protected override void FollowPlayer()
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
        else if (distanceToPlayer <= raceDistance)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;

            transform.position += new Vector3(direction.x, 0f, 0f) * moveSpeed * Time.deltaTime;

            if (direction.x != 0)
            {
                float targetAngle = (direction.x < 0f) ? -90f : 90f;
                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            }

            if (animator != null) animator.SetFloat("Speed", 1f);
        }
        else
        {
            if (animator != null) animator.SetFloat("Speed", 0f);
        }
    }

    public override void GetHit()
    {
        if (!isAlive) return;

        health -= 20f;
        if (enemySlider != null) enemySlider.value = health;

        if (health <= 0f)
        {
            isAlive = false;
            if (attackCollider != null) attackCollider.enabled = false;
            if (animator != null) animator.SetTrigger("Death");

            CapsuleCollider collider = GetComponent<CapsuleCollider>();
            if (collider != null) collider.enabled = false;

            Invoke("GoToMenu", 3f);
        }
    }

    private void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public override void AttackCollisionEnabled() { }
    public override void AttackCollisionDisabled() { }
}