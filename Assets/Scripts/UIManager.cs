using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoSingleton<UIManager>
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI restartText;

    [Header("Images")]
    [SerializeField] private Image livesImage;
    [SerializeField] private Sprite[] livesSprites;


    public void UpdateScore(int updatedScore)
    {
        scoreText.text = $"Score: {updatedScore.ToString("000000")}";
    }

    public void UpdateLives(int updatedPlayerHealth)
    {
        livesImage.sprite = livesSprites[updatedPlayerHealth];

        if (updatedPlayerHealth == 0)
        {
            gameOverText.gameObject.SetActive(true);
            restartText.gameObject.SetActive(true);
        }
    }
}