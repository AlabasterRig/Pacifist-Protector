using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    public static ScoreController Instance;

    [Header("UI References")]
    public TMP_Text ScoreText;
    public TMP_Text RemainingText;

    [Header("Settings")]
    public int WinScoreThreshold = 40;
    public int TotalCreatures = 5;

    private int Score = 0;
    private int RemainingCreatures;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        RemainingCreatures = TotalCreatures;
    }

    void Start()
    {
        UpdateScoreUI();
        UpdateRemainingUI();
    }

    public void AddScore(int amount)
    {
        Score += amount;
        UpdateScoreUI();
        CheckWinCondition();
    }

    public void OnCreatureSaved()
    {
        RemainingCreatures--;
        UpdateRemainingUI();
    }

    public void ResetScore()
    {
        Score = 0;
        RemainingCreatures = TotalCreatures;
        UpdateScoreUI();
        UpdateRemainingUI();
    }

    private void UpdateScoreUI()
    {
        if (ScoreText != null)
        {
            ScoreText.text = $"Score: {Score} / {WinScoreThreshold}";
        }
    }

    private void UpdateRemainingUI()
    {
        if (RemainingText != null)
        {
            RemainingText.text = $"Creatures Remaining: {RemainingCreatures}";
        }
    }

    private void CheckWinCondition()
    {
        if (Score >= WinScoreThreshold)
        {
            LevelController.Instance?.ShowGameOverPanel();
        }
    }
}
