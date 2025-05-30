using System.Collections.Generic;
using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    public float Speed = 2f;
    public float DetectionRadius = 5f;
    public float ChaseCooldown = 3f;

    public List<Transform> PatrolPoints;
    public Animator animator;
    private int CurrentPatrolIndex = 0;

    private Transform ProtectedTarget;
    private float ProtectedChaseTimer = 0f;
    public float ProtectedChaseDuration = 5f;
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
            if (DecoyTimer <= 0 || !DecoyTarget.gameObject.activeInHierarchy)
            {
                DecoyTarget = null;
                HasTarget = false;
            }
        }
        if (ProtectedTarget != null)
        {
            if (!ProtectedTarget.gameObject.activeInHierarchy)
            {
                ProtectedTarget = null;
                HasTarget = false;
            }
            else
            {
                ProtectedChaseTimer -= Time.deltaTime;
                if (ProtectedChaseTimer <= 0)
                {
                    ProtectedTarget = null;
                    HasTarget = false;
                }
            }
        }

        // Priority: Decoy > Protected > Patrol
        if (DecoyTarget != null)
        {
            SetTarget(DecoyTarget.position);
        }
        else if (ProtectedTarget != null)
        {
            float distance = Vector2.Distance(transform.position, ProtectedTarget.position);

            if (distance < 0.5f)
            {
                ProtectedTarget = null;
                ProtectedChaseTimer = 0f;
                HasTarget = false;
                if (animator != null)
                {
                    animator.SetTrigger("LostTarget");
                }
                return;
            }

            SetTarget(ProtectedTarget.position);
        }
        else if (FindNearestProtected(out Transform newProtected))
        {
            if (ProtectedTarget != newProtected)
            {
                ProtectedTarget = newProtected;
                ProtectedChaseTimer = ProtectedChaseDuration;
            }
            SetTarget(ProtectedTarget.position);
        }
        else
        {
            Patrol();
        }

        if (!HasTarget && animator != null)
        {
            animator.SetFloat("Speed", Speed);
            animator.SetTrigger("StartPatrol");
        }
    }

    void FixedUpdate()
    {
        if (HasTarget)
        {
            TryMoveTo(MovementTarget);
        }
        StickToGround();
    }

    private void SetTarget(Vector2 targetPos)
    {
        MovementTarget = targetPos;
        HasTarget = true;
    }

    private void TryMoveTo(Vector2 targetPos)
    {
        Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
        float moveStep = Speed * Time.fixedDeltaTime;
        Vector2 nextPos = (Vector2)transform.position + dir * moveStep;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 0.5f, LayerMask.GetMask("Obstacle"));
        if (!hit)
        {
            rb.MovePosition(nextPos);
            if (animator != null)
            {
                animator.SetFloat("Speed", moveStep);
            }

            if (dir.x > 0.01f)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else if (dir.x < -0.01f)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }
        else
        {
            HasTarget = false;
        }
    }

    private void MoveTo(Vector2 targetPos)
    {
        Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
        Vector2 newPos = (Vector2)transform.position + dir * Speed * Time.deltaTime;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 0.5f, LayerMask.GetMask("Obstacle"));
        if (!hit)
        {
            rb.MovePosition(newPos);
            if (dir.x > 0.01f)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else if (dir.x < -0.01f)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }
    }

    private void Patrol()
    {
        if (PatrolPoints.Count == 0)
        {
            return;
        }

        if (!HasTarget && animator != null)
        {
            animator.SetTrigger("StartPatrol");
        }

        Transform point = PatrolPoints[CurrentPatrolIndex];
        MoveTo(point.position);

        if (Vector2.Distance(transform.position, point.position) < 0.2f)
        {
            CurrentPatrolIndex = (CurrentPatrolIndex + 1) % PatrolPoints.Count;
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", Speed);
        }
    }

    private void StickToGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2f, LayerMask.GetMask("Ground"));

        if (hit.collider != null)
        {
            Vector3 pos = transform.position;
            // Adjusting Y postion for standing on the ground
            pos.y = hit.point.y + GetComponent<Collider2D>().bounds.extents.y;
            transform.position = pos;
        }
    }

    public void OnProtectedCreatureDestroyed(Transform destroyedTarget)
    {
        if (ProtectedTarget == destroyedTarget)
        {
            ProtectedTarget = null;
            HasTarget = false;

            if (animator != null)
            {
                animator.SetTrigger("LostTarget");
            }
        }
    }

    private bool FindNearestProtected(out Transform closest)
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
        HasTarget = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRadius);
    }
}
