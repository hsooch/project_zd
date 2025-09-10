using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Boot, Playing, Paused, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }
    public GameState State { get; private set; } = GameState.Boot;

    [SerializeField] private Barricade barricade;
    [SerializeField] private Spawner spawner;
    [SerializeField] private UIHud hud;

    private float _survivalTime;

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this; DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        State = GameState.Playing;
        if (barricade != null) barricade.OnBarricadeDestroyed += HandleGameOver;
        if (spawner != null) spawner.enabled = true;
    }

    private void Update()
    {
        if (State != GameState.Playing) return;
        _survivalTime += Time.deltaTime;
        if (hud) hud.SetTime(_survivalTime);
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
    }

    public void TogglePause()
    {
        if (State == GameState.Playing)
        {
            State = GameState.Paused; Time.timeScale = 0f; hud?.ShowPause(true);
        }
        else if (State == GameState.Paused)
        {
            State = GameState.Playing; Time.timeScale = 1f; hud?.ShowPause(false);
        }
    }

    private void HandleGameOver()
    {
        State = GameState.GameOver;
        Time.timeScale = 0f;
        hud?.ShowGameOver(true);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
