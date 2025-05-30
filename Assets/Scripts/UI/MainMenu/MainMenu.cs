using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button PlayButton;

    private void Awake()
    {
        PlayButton.onClick.AddListener(OnPlayButtonClicked);
    }

    public void OnPlayButtonClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("FirstScene");
    }
}
