using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    #region Variables
    [Header("Ship Reference")]
    private Ship playerShip;

    [Header("Game Status")]
    [SerializeField] private int score;
    [SerializeField] private bool isGameOver;
    #endregion


    #region MonoBehaviour Methods
    protected override void Init()
    {
        playerShip = FindObjectOfType<Ship>();
    }

    private void OnEnable()
    {
        playerShip.OnEntityKilled += GameOver;
    }
    private void OnDisable()
    {
        playerShip.OnEntityKilled -= GameOver;
    }

    private void Start()
    {
        AddScore(0);
    }
    #endregion

    #region Custom Methods
    public void RestartLevel(UnityEngine.InputSystem.InputAction.CallbackContext input)
    {
        if (isGameOver)
        {
            if (input.performed)
            {
                LevelManager.Instance.LoadScene(SpaceShooterData.Levels.GameScene);
            }
        }
    }
    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        UIManager.Instance?.UpdateScore(score);
    }
    public void GameOver(IDamageable playerKilled)
    {
        isGameOver = true;
        SpawnManager.Instance?.StopAllSpawns();
    }
    public bool IsGameOver()
    {
        return isGameOver;
    }
    #endregion
}