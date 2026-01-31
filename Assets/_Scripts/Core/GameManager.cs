using UnityEngine;

public enum GameState
{
    MainMenu = 0,
    Playing = 1,
    Paused = 2,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartGame()
    {
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
        SceneLoader.Instance.Load(SceneNames.GameScene);
        Debug.Log("Game started");
    }

    public void GoToMenu()
    {
        CurrentState = GameState.MainMenu;
        Time.timeScale = 1f;
        SceneLoader.Instance.Load(SceneNames.MainMenu);
        Debug.Log("Go to Main Menu");
    }

    public void Pause()
    {
        if (CurrentState != GameState.Playing)
            return;

        CurrentState = GameState.Paused;
        Time.timeScale = 0f; // простой вариант паузы
        EventBus.Instance.RaiseGamePaused();
        Debug.Log("Game paused");
    }

    public void Resume()
    {
        if (CurrentState != GameState.Paused)
            return;

        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
        EventBus.Instance.RaiseGameResumed();
        Debug.Log("Game resumed");
    }
}