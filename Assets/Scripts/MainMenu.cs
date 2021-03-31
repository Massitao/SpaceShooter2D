using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button newGameButton;

    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }
}