using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAbilites : MonoBehaviour
{
    public GameObject BarrierPrefab;
    public GameObject DecoyPrefab;

    public float BarrierLifetime = 3f;
    public float BarrierCooldown = 5f;
    public float DecoyLifetime = 4f;
    public float DecoyCooldown = 6f;
    public float TeleportRange = 5f;
    public float DecoyRange = 6f;
    public LayerMask GroundLayer;

    private float NextBarrierTime = 0f;
    private float NextDecoyTime = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && Time.time >= NextBarrierTime)
        {
            Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SpawnBarrier(MousePos);
        }

        if (Input.GetKeyDown(KeyCode.E) && Time.time >= NextDecoyTime)
        {
            Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SpawnDecoy(MousePos);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            TeleportProtectedCreature(MousePos);
        }
    }

    void SpawnBarrier(Vector2 Position)
    {
        GameObject Barrier = Instantiate(BarrierPrefab, Position, Quaternion.identity);
        Destroy(Barrier, BarrierLifetime);
        NextBarrierTime = Time.time + BarrierCooldown;
    }

    void SpawnDecoy(Vector2 Position)
    {
        Vector2 playerPos = transform.position;

        if (Vector2.Distance(Position, playerPos) > DecoyRange)
        {
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(Position, Vector2.down, 2f, GroundLayer);

        if (hit.collider != null)
        {
            Vector2 spawnPos = hit.point;
            spawnPos.y += 1.0f;

            GameObject decoy = Instantiate(DecoyPrefab, spawnPos, Quaternion.identity);
            Destroy(decoy, DecoyLifetime);
            NextDecoyTime = Time.time + DecoyCooldown;
        }
        else
        {
            Debug.Log("No valid ground to place decoy.");
        }
    }

    void TeleportProtectedCreature(Vector2 TargetPos)
    {
        // Check if the target point is within teleport radius
        if (Vector2.Distance(transform.position, TargetPos) > TeleportRange)
        {
            Debug.Log("Target point is out of teleport range.");
            return;
        }

        // Check if ground exists at the target point
        RaycastHit2D groundHit = Physics2D.Raycast(TargetPos, Vector2.down, 2f, GroundLayer);
        if (groundHit.collider == null)
        {
            Debug.Log("Teleport destination is not on valid ground.");
            return;
        }

        // Find the nearest Protected creature within teleport range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, TeleportRange);
        Transform ClosestProtected = null;
        float ClosestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Protected"))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < ClosestDist)
                {
                    ClosestDist = dist;
                    ClosestProtected = hit.transform;
                }
            }
        }

        if (ClosestProtected != null)
        {
            Vector2 teleportPos = groundHit.point;
            teleportPos.y += 1.0f; // adjust height slightly above ground
            ClosestProtected.position = teleportPos;
        }
        else
        {
            Debug.Log("No protected creature found within teleport range.");
        }
    }
}
