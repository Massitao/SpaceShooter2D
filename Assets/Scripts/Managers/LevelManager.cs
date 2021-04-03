using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoSingleton<LevelManager>
{
    protected override void Init()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void QuitGame(UnityEngine.InputSystem.InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            Application.Quit();
        }
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void LoadScene(SpaceShooterData.Levels sceneToLoad)
    {
        SceneManager.LoadScene((int)sceneToLoad);
    }
}