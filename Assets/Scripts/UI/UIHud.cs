using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHud : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;

    public void SetTime(float t)
    {
        int s = Mathf.FloorToInt(t);
        timeText.text = $"{s/60:00}:{s%60:00}";
    }

    public void ShowPause(bool show) => pausePanel?.SetActive(show);
    public void ShowGameOver(bool show) => gameOverPanel?.SetActive(show);
}
