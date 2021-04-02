using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoSingleton<UIManager>
{
    #region Variables
    [Header("Ship Reference")]
    private Ship playerShip;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI restartText;

    [Header("Images")]
    [SerializeField] private Image livesImage;
    [SerializeField] private Sprite[] livesSprites;
    #endregion


    #region MonoBehaviour Methods
    protected override void Init()
    {
        playerShip = FindObjectOfType<Ship>();
    }

    private void OnEnable()
    {
        playerShip.OnEntityDamaged += UpdateLives;
    }
    private void OnDisable()
    {
        playerShip.OnEntityDamaged -= UpdateLives;
    }

    private void Start()
    {
        UpdateLives(playerShip.EntityHealth);
    }
    #endregion

    #region Custom Methods
    public void UpdateScore(int updatedScore)
    {
        scoreText.text = $"Score: {updatedScore.ToString("000000")}";
    }
    public void UpdateLives(int updatedPlayerHealth)
    {
        livesImage.sprite = livesSprites[Mathf.Clamp(updatedPlayerHealth, 0, livesSprites.Length)];

        if (updatedPlayerHealth == 0)
        {
            gameOverText.gameObject.SetActive(true);
            restartText.gameObject.SetActive(true);
        }
    }
    #endregion
}