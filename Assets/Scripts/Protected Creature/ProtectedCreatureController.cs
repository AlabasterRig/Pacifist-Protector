using UnityEngine;

public class ProtectedCreatureController : MonoBehaviour
{
    public float FollowDistance = 1.5f;
    public float FollowSpeed = 3f;
    public Transform Player;
    private bool IsFollowing = false;

    void Update()
    {
        if (!IsFollowing && Vector2.Distance(transform.position, Player.position) <= FollowDistance)
        {
            IsFollowing = true;
        }

        if (IsFollowing)
        {
            Vector2 targetPos = Player.position + (Vector3)(-Player.right); // Follow behind player
            transform.position = Vector2.MoveTowards(transform.position, targetPos, FollowSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Safezone"))
        {
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Enemy"))
        {
            // Handle capture (e.g., destroy or notify game manager)
            Debug.Log("Creature Captured!");
            Destroy(gameObject);
        }
    }
}
