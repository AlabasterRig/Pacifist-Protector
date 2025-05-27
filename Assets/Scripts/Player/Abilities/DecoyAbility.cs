using UnityEngine;

public class DecoyAbility : MonoBehaviour
{
    public float Radius = 5f;

    void Start()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, Radius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.GetComponent<EnemyAIController>()?.ChaseDecoy(transform);
            }
        }
    }
}
