using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance { get; private set; }

    [Header("UI Panels")]
    public CanvasGroup GameOverPanel;
    public CanvasGroup GameLostPanel;

    [Header("Fadeing settings")]
    public float FadeDuration = 1f;

    private void Awake()
    {
        Instance = this;
        if (GameOverPanel != null)
        {
            SetCanvasGroupVisible(GameOverPanel, false);
        }
        if (GameLostPanel != null)
        {
            SetCanvasGroupVisible(GameLostPanel, false);
        }
    }

    public void ShowGameOverPanel()
    {
        if (GameOverPanel != null)
        {
            StartCoroutine(FadeInPanel(GameOverPanel));
        }
        PauseManager.Instance?.OnGameEnd();
    }

    public void ShowGameLostPanel()
    {
        if (GameLostPanel != null)
        {
            StartCoroutine(FadeInPanel(GameLostPanel));
        }
        PauseManager.Instance?.OnGameEnd();
    }

    private IEnumerator FadeInPanel(CanvasGroup Panel)
    {
        Panel.gameObject.SetActive(true);
        Panel.alpha = 0f;
        Panel.interactable = true;
        Panel.blocksRaycasts = true;

        float t = 0f;
        while (t < FadeDuration)
        {
            t += Time.unscaledDeltaTime;
            Panel.alpha = Mathf.Lerp(0f, 1f, t / FadeDuration);
            yield return null;
        }
        Panel.alpha = 1f;
    }

    private void SetCanvasGroupVisible(CanvasGroup Group, bool Visible)
    {
        Group.alpha = Visible ? 1f : 0f;
        Group.interactable = Visible;
        Group.blocksRaycasts = Visible;
        Group.gameObject.SetActive(Visible);
    }
}
