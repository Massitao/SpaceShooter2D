using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button newGameButton;

    public void LoadGame()
    {
        LevelManager.Instance.LoadScene(SpaceShooterData.Levels.GameScene);
    }
}