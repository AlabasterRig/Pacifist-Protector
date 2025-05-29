using Unity.Cinemachine;
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

    private void SpawnBarrier(Vector2 Position)
    {
        GameObject Barrier = Instantiate(BarrierPrefab, Position, Quaternion.identity);
        Destroy(Barrier, BarrierLifetime);
        NextBarrierTime = Time.time + BarrierCooldown;
    }

    private void SpawnDecoy(Vector2 Position)
    {
        Vector2 PlayerPos = transform.position;

        if (Vector2.Distance(Position, PlayerPos) > DecoyRange)
        {
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(Position, Vector2.down, 2f, GroundLayer);

        if (hit.collider != null)
        {
            Vector2 SpawnPos = hit.point;
            SpawnPos.y += 1.0f;

            GameObject decoy = Instantiate(DecoyPrefab, SpawnPos, Quaternion.identity);
            Destroy(decoy, DecoyLifetime);
            NextDecoyTime = Time.time + DecoyCooldown;
        }
        else
        {
            Debug.Log("No valid ground to place decoy.");
        }
    }

    private void TeleportProtectedCreature(Vector2 TargetPos)
    {
        if (Vector2.Distance(transform.position, TargetPos) > TeleportRange)
        {
            Debug.Log("Target point is out of teleport range.");
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(TargetPos, Vector2.down, 2f, GroundLayer);

        if (hit.collider == null)
        {
            Debug.Log("Teleport destination is not on valid ground.");
            return;
        }

        Vector2 SpawnPos = hit.point;
        SpawnPos.y += 1.0f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, TeleportRange);
        Transform ClosestProtected = null;
        float ClosestDist = Mathf.Infinity;

        foreach (var obj in hits)
        {
            if (obj.CompareTag("Protected"))
            {
                float dist = Vector2.Distance(transform.position, obj.transform.position);
                if (dist < ClosestDist)
                {
                    ClosestDist = dist;
                    ClosestProtected = obj.transform;
                }
            }
        }

        if (ClosestProtected != null)
        {
            ClosestProtected.position = SpawnPos;
        }
        else
        {
            Debug.Log("No protected creature found within teleport range.");
        }
    }
}
