using UnityEngine;

public class PlayerAbilites : MonoBehaviour
{
    public GameObject BarrierPrefab;
    public GameObject DecoyPrefab;

    public float BarrierLifetime = 3f;
    public float BarrierCooldown = 5f;
    public float DecoyLifetime = 4f;
    public float DecoyCooldown = 6f;
    public float TeleportRange = 5f;

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
        GameObject Decoy = Instantiate(DecoyPrefab, Position, Quaternion.identity);
        Destroy(Decoy, DecoyLifetime);
        NextDecoyTime = Time.time + DecoyCooldown;
    }

    void TeleportProtectedCreature(Vector2 TargetPos)
    {
        Collider2D Hit = Physics2D.OverlapPoint(TargetPos);
        if (Hit != null && Hit.CompareTag("Protected"))
        {
            float distance = Vector2.Distance(transform.position, Hit.transform.position);
            if (distance <= TeleportRange)
            {
                Hit.transform.position = TargetPos;
            }
            else
            {
                Debug.Log("Target too far to teleport!");
            }
        }
    }
}
