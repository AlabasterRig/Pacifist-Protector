using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    public CanvasGroup PauseMenu;
    private bool IsPaused = false;
    private bool GameEnded = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } 
        else
        {
            Destroy(gameObject);
        }

        if (PauseMenu != null)
        {
            SetVisible(PauseMenu, false);
        }
    }

    private void Update()
    {
        if (GameEnded)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }

        }
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        if (PauseMenu != null)
        {
            SetVisible(PauseMenu, true);
        }
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        if (PauseMenu != null)
        {
            SetVisible(PauseMenu, false);
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OnGameEnd()
    {
        GameEnded = true;
        Time.timeScale = 0f;
    }

    private void SetVisible(CanvasGroup group, bool visible)
    {
        group.alpha = visible ? 1f : 0f;
        group.interactable = visible;
        group.blocksRaycasts = visible;
        group.gameObject.SetActive(visible);
    }
}
