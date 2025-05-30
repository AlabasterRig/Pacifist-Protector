using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Button RestartButton;
    public Button MainMenuButton;

    private void Awake()
    {
        RestartButton.onClick.AddListener(OnRestartButtonClicked);
        MainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
    }

    public void OnMainMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnRestartButtonClicked()
    {
        Time.timeScale = 1f;
        ScoreController.Instance.ResetScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
