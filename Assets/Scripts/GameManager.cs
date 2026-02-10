using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Configuración del juego")]
    public int totalScore = 0;
    public float countdownTime = 60f;
    public bool isGameActive = true;
    private float timeRemaining;

    [Header("Referencias de la UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    public string sceneName = "MainMenu";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        timeRemaining = countdownTime;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        isGameActive = true;
    }

    void Update()
    {
        if (isGameActive)
        {
            timeRemaining -= Time.deltaTime;

            UpdateTimeUI();

            if (timeRemaining <= 0)
            {
                EndGame();
            }
        }
    }

    void UpdateTimeUI()
    {
        if (timeText != null)
        {
            float minutes = Mathf.FloorToInt(timeRemaining / 60);
            float seconds = Mathf.FloorToInt(timeRemaining % 60);

            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void AddPoints(int amount)
    {
        if (!isGameActive) return;

        totalScore += amount;

        if (scoreText != null)
        {
            scoreText.text = "Puntos: " + totalScore.ToString();
        }
    }

    void EndGame()
    {
        isGameActive = false;
        timeRemaining = 0;
        UpdateTimeUI();

        Debug.Log("¡Juego Terminado!");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (finalScoreText != null)
            {
                finalScoreText.text = "Puntos: " + totalScore.ToString();
            }
        }

        EliminarEnemigosRestantes();
    }

    void EliminarEnemigosRestantes()
    {
        // 1. Buscar todos los objetos en la escena que tengan el Tag "Enemigo"
        // (Asegúrate de haber creado y asignado el Tag "Enemigo" a tu prefab)
        GameObject[] enemigosVivos = GameObject.FindGameObjectsWithTag("Enemy");

        // 2. Recorrer la lista y destruirlos uno por uno
        foreach (GameObject enemigo in enemigosVivos)
        {
            // Opcional: Podrías poner un efecto de partícula aquí si quisieras que "exploten" al final
            Destroy(enemigo);
        }
        
        // Opcional: También podrías limpiar las bolas de nieve que quedaron en el suelo
        GameObject[] bolasSueltas = GameObject.FindGameObjectsWithTag("Snowball");
        foreach (GameObject bola in bolasSueltas)
        {
            Destroy(bola);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(sceneName);
    }
}