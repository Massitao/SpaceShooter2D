using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoSingleton<UIManager>
{
    #region Variables
    [Header("Ship Reference")]
    private Ship playerShip;

    [Header("Lives")]
    [SerializeField] private Image livesImage;
    [SerializeField] private Sprite[] livesSprites;

    [Header("Ammo")]
    [SerializeField] private TextMeshProUGUI ammoCountText;
    [SerializeField] private Slider ammoSlider;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Game Over")]
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI restartText;
    #endregion


    #region MonoBehaviour Methods
    protected override void Init()
    {
        playerShip = FindObjectOfType<Ship>();
    }

    private void OnEnable()
    {
        playerShip.OnShoot += UpdateAmmoCount;
        playerShip.OnEntityDamaged += UpdateLives;
    }
    private void OnDisable()
    {
        playerShip.OnShoot -= UpdateAmmoCount;
        playerShip.OnEntityDamaged -= UpdateLives;
    }

    private void Start()
    {
        UpdateLives(playerShip.EntityHealth);

        ammoSlider.maxValue = playerShip.GetMaxAmmoCount();
        UpdateAmmoCount(playerShip.GetAmmoCount());
    }
    #endregion

    #region Custom Methods
    public void UpdateLives(int updatedPlayerHealth)
    {
        livesImage.sprite = livesSprites[Mathf.Clamp(updatedPlayerHealth, 0, livesSprites.Length)];

        if (updatedPlayerHealth == 0)
        {
            gameOverText.gameObject.SetActive(true);
            restartText.gameObject.SetActive(true);
        }
    }
    public void UpdateAmmoCount(int updatedPlayerAmmoCount)
    {
        ammoCountText.text = $"Lasers: {updatedPlayerAmmoCount.ToString("00")} / {playerShip.GetMaxAmmoCount()}";
        ammoSlider.value = updatedPlayerAmmoCount;
    }
    public void UpdateScore(int updatedScore)
    {
        scoreText.text = $"Score: {updatedScore.ToString("000000")}";
    }
    #endregion
}