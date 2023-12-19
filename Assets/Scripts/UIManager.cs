using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textTimer;
    [SerializeField] private TextMeshProUGUI textLife;
    [SerializeField] private TextMeshProUGUI textEndGame;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject endedMenu;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float timePassed = GameManager.Instance.timePassed;
        int minutes = Mathf.FloorToInt(timePassed / 60);
        int seconds = Mathf.FloorToInt(timePassed % 60);
        int milliseconds = Mathf.FloorToInt((timePassed - Mathf.FloorToInt(timePassed)) * 100);
        textTimer.text = "Time : " + string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
        textLife.text = "Life : " + GameManager.Instance.life;
    }


    private void PauseGame()
    {
        if (GameManager.Instance.isGameEnded)
            return;

        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }
        else
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
    }

    public void FinishGame()
    {
        if (GameManager.Instance.isGameEnded)
        {
            Time.timeScale = 0;
            float timePassed = GameManager.Instance.timePassed;
            int minutes = Mathf.FloorToInt(timePassed / 60);
            int seconds = Mathf.FloorToInt(timePassed % 60);
            int milliseconds = Mathf.FloorToInt((timePassed - Mathf.FloorToInt(timePassed)) * 100);
            textEndGame.text = "Well done ! Your time is : " + string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
            endedMenu.SetActive(true);
        }
        
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        GameManager.Instance.Restart();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }
}
