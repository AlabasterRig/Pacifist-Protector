using UnityEngine;

public class PlayerDeathController : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerMovementController>() != null)
        {
            LevelController.Instance.ShowGameLostPanel();
            Debug.Log("Player Died from falling!");
        }
    }
}
