using UnityEngine;

public class ProtectedCreatureController : MonoBehaviour
{
    public float FollowDistance = 1.5f;
    public float FollowSpeed = 3f;
    public Transform Player;
    private bool IsFollowing = false;
    public Animator animator;
    private Vector2 LastPosition;

    void Update()
    {
        if (!IsFollowing && Vector2.Distance(transform.position, Player.position) <= FollowDistance)
        {
            IsFollowing = true;
        }

        if (IsFollowing)
        {
            Vector2 targetPos = Player.position + (Vector3)(-Player.right);
            transform.position = Vector2.MoveTowards(transform.position, targetPos, FollowSpeed * Time.deltaTime);

            float speed = (transform.position - (Vector3)LastPosition).magnitude / Time.deltaTime;
            animator.SetFloat("Speed", speed);

            UpdateFacingDirection();
            LastPosition = transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Safezone"))
        {
            NotifyEnemiesBeforeDestruction();
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Enemy"))
        {
            NotifyEnemiesBeforeDestruction();
            Debug.Log("Creature Captured!");
            Destroy(gameObject);
        }
    }

    private void NotifyEnemiesBeforeDestruction()
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, 10f, LayerMask.GetMask("Enemy"));
        foreach (var enemyCol in nearbyEnemies)
        {
            EnemyAIController enemy = enemyCol.GetComponent<EnemyAIController>();
            if (enemy != null)
            {
                enemy.OnProtectedCreatureDestroyed(transform);
            }
        }
    }

    private void UpdateFacingDirection()
    {
        Vector2 currentPosition = transform.position;
        float deltaX = currentPosition.x - LastPosition.x;

        if (Mathf.Abs(deltaX) > 0.01f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(deltaX) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        LastPosition = currentPosition;
    }
}
