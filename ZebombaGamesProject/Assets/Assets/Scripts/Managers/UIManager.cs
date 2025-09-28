using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI mainMenuTitleText;

    [Header("Buttons")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button restartGameButton;
    [SerializeField] private Button toMainMenuButton;
    [SerializeField] private Button zoomInOutTutorialButton;

    private void Awake()
    {
        GameplayManager.OnScoreUpdate.AddListener(UpdateScoreUI);
        GameplayManager.OnGameOver.AddListener(GameOver);

        startGameButton.onClick.AddListener(StartGame);
        restartGameButton.onClick.AddListener(RestartGame);
        toMainMenuButton.onClick.AddListener(OpenMainMenu);
        zoomInOutTutorialButton.onClick.AddListener(CloseZoomInOutTutorial);
    }

    private void Start()
    {
        scoreText.text = "Score: " + GameplayManager.Instance.Score;
    }

    private void OnDisable()
    {
        GameplayManager.OnScoreUpdate.RemoveListener(UpdateScoreUI);
        GameplayManager.OnGameOver.RemoveListener(GameOver);

        startGameButton.onClick.RemoveListener(StartGame);
        restartGameButton.onClick.RemoveListener(RestartGame);
        toMainMenuButton.onClick.RemoveListener(OpenMainMenu);
        zoomInOutTutorialButton.onClick.RemoveListener(CloseZoomInOutTutorial);
    }

    private void StartGame()
    {
        mainMenuPanel.SetActive(false);
        gameplayPanel.SetActive(true);

        GameplayManager.Instance.Play();
    }

    private void RestartGame()
    {
        gameOverPanel.SetActive(false);
        gameplayPanel.SetActive(true);

        GameplayManager.Instance.Play();
    }

    private void GameOver()
    {
        gameplayPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    private void OpenMainMenu()
    {
        gameOverPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    private void CloseZoomInOutTutorial()
    {
        zoomInOutTutorialButton.gameObject.SetActive(false);
        GameplayManager.Instance.isTutorial = false;
    }

    private void UpdateScoreUI(int newScore)
    {
        scoreText.text = "Score: " + newScore;
    }
}
