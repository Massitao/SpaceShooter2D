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

    [Header("Thrusters")]
    [SerializeField] private Slider thrusterFuelSlider;
    [SerializeField] private Slider thrusterCooldownSlider;

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
        playerShip.OnEntityHealed += UpdateLives;
        playerShip.OnEntityDamaged += UpdateLives;

        playerShip.OnShoot += UpdateAmmoCount;
        playerShip.OnAmmoRefill += UpdateAmmoCount;

        playerShip.OnThrusterFuelChange += UpdateThrusterFuelSlider;
        playerShip.OnThrusterCooldownChange += UpdateThrusterCooldownSlider;
    }
    private void OnDisable()
    {
        playerShip.OnEntityHealed -= UpdateLives;
        playerShip.OnEntityDamaged -= UpdateLives;

        playerShip.OnShoot -= UpdateAmmoCount;
        playerShip.OnAmmoRefill -= UpdateAmmoCount;

        playerShip.OnThrusterFuelChange -= UpdateThrusterFuelSlider;
        playerShip.OnThrusterCooldownChange -= UpdateThrusterCooldownSlider;
    }

    private void Start()
    {
        UpdateLives(playerShip.EntityHealth);

        ammoSlider.maxValue = playerShip.GetMaxAmmoCount();
        UpdateAmmoCount(playerShip.GetAmmoCount());

        UpdateThrusterFuelSlider(1f);
        UpdateThrusterCooldownSlider(0f);
    }
    #endregion

    #region Custom Methods
    public void UpdateLives(int updatedPlayerHealth)
    {
        livesImage.sprite = livesSprites[Mathf.Clamp(updatedPlayerHealth, 0, livesSprites.Length - 1)];

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
    public void UpdateThrusterFuelSlider(float updatedPlayerFuel)
    {
        thrusterFuelSlider.value = updatedPlayerFuel;
    }
    public void UpdateThrusterCooldownSlider(float updatedPlayerThrusterCooldown)
    {
        thrusterCooldownSlider.value = updatedPlayerThrusterCooldown;
    }
    public void UpdateScore(int updatedScore)
    {
        scoreText.text = $"Score: {updatedScore.ToString("000000")}";
    }
    #endregion
}