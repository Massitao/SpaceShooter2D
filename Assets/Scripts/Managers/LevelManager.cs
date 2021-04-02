using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoSingleton<LevelManager>
{
    protected override void Init()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            LoadScene(SpaceShooterData.Levels.MainMenuScene);
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