using System.Collections.Generic;
using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    public float Speed = 2f;
    public float DetectionRadius = 5f;
    public float ChaseCooldown = 3f;

    public List<Transform> PatrolPoints;
    private int CurrentPatrolIndex = 0;

    private Transform CurrentTarget;
    private Transform DecoyTarget;
    private Vector2 MovementTarget;
    private bool HasTarget = false;
    private float DecoyTimer = 0f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (DecoyTarget != null)
        {
            DecoyTimer -= Time.deltaTime;
            if (DecoyTimer <= 0)
            {
                DecoyTarget = null;
            }
        }

        if (DecoyTarget != null)
        {
            SetTarget(DecoyTarget.position);
        }
        else if (FindNearestProtected(out Transform target))
        {
            SetTarget(target.position);
        }
        else
        {
            Patrol();
        }
    }

    void FixedUpdate()
    {
        if (HasTarget)
        {
            TryMoveTo(MovementTarget);
        }
    }

    void SetTarget(Vector2 targetPos)
    {
        MovementTarget = targetPos;
        HasTarget = true;
    }

    void TryMoveTo(Vector2 targetPos)
    {
        Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
        float moveStep = Speed * Time.fixedDeltaTime;
        Vector2 nextPos = (Vector2)transform.position + dir * moveStep;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 0.5f, LayerMask.GetMask("Obstacle"));
        if (!hit)
        {
            rb.MovePosition(nextPos);
        }
        else
        {
            HasTarget = false;
        }
    }

    void MoveTo(Vector2 targetPos)
    {
        Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
        Vector2 newPos = (Vector2)transform.position + dir * Speed * Time.deltaTime;

        // Barrier avoidance using raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 0.5f, LayerMask.GetMask("Obstacle"));
        if (!hit)
        {
            rb.MovePosition(newPos);
        }
    }

    void Patrol()
    {
        if (PatrolPoints.Count == 0) return;

        Transform point = PatrolPoints[CurrentPatrolIndex];
        MoveTo(point.position);

        if (Vector2.Distance(transform.position, point.position) < 0.2f)
        {
            CurrentPatrolIndex = (CurrentPatrolIndex + 1) % PatrolPoints.Count;
        }
    }

    bool FindNearestProtected(out Transform closest)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, DetectionRadius);
        float closestDist = Mathf.Infinity;
        closest = null;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Protected"))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = hit.transform;
                }
            }
        }

        return closest != null;
    }

    public void ChaseDecoy(Transform decoy)
    {
        DecoyTarget = decoy;
        DecoyTimer = ChaseCooldown;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRadius);
    }
}
