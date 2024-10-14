using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
    Easy, // x0.5 Health, damage - 0
    Normal, // x1 Health, damage - 1
    Hard, // x1.5 Health, damage - 2
    Survivalist // Same as hard, but no heals - 3
 */

public class GameManager : MonoBehaviour
{
    [SerializeField] Image blackScreen;
    [SerializeField] GameObject gameOverPrompt;
    public string[] difficulties;
    public UnityEvent onVolumeUpdate, onGameOver;
    public bool gameOver;
    // Start is called before the first frame update
    void Start()
    {
        blackScreen.color = new Color(0, 0, 0, 1);
        StartCoroutine(toggleBlackScreen());
        gameOver = false;
        Debug.Log("Highest Score Reached" + " (" + PlayerPrefs.GetString("difficulty") + "): " + PlayerPrefs.GetInt("highestWave" + PlayerPrefs.GetString("difficulty")));
    }

    public void setEasy()
    {
        PlayerPrefs.SetString("difficulty", "Easy");
    }

    public void setNormal()
    {
        PlayerPrefs.SetString("difficulty", "Normal");
    }

    public void setHard()
    {
        PlayerPrefs.SetString("difficulty", "Hard");
    }

    public void setSurvivalist()
    {
        PlayerPrefs.SetString("difficulty", "Survivalist");
    }

    public void setDifficulty(int diff)
    {
        if (diff >= difficulties.Length || diff < 0)
        {
            Debug.Log("Difficulty Error!");
            return;
        }

        PlayerPrefs.SetString("difficulty", difficulties[diff]);
        Debug.Log(PlayerPrefs.GetString("difficulty"));
    }

    public void setMusicVolume(float vol)
    {
        float MAX_VOL = 0.4f;
        vol *= MAX_VOL;
        vol = Mathf.Clamp(vol, 0.01f, MAX_VOL);
        PlayerPrefs.SetFloat("mVolume", vol);
        onVolumeUpdate?.Invoke();
    }

    public void setSFXVolume(float vol)
    {
        PlayerPrefs.GetFloat("sfxVolume", vol);
    }

    public void pauseGame()
    {
        // Open pause menu
        if (!gameOver) {
            Time.timeScale = 0;
        }   
    }

    public void unpauseGame()
    {
        Time.timeScale = 1;
        // Close pause menu
    }

    public void mainMenu()
    {
        // Load main menu + lobby scene
        onGameOver?.Invoke();
        Time.timeScale = 1;
        Invoke("loadLobby", 0.1f);
    }

    void loadLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void endGame()
    {
        gameOver = true;
        onGameOver?.Invoke();
        StartCoroutine(toggleBlackScreen());
        Invoke("restartGame", 5f); // put this here for now
    }

    public void restartGame()
    {
        SceneManager.LoadScene("Arena");
    }

    IEnumerator toggleBlackScreen()
    {
        yield return new WaitForSeconds(1f);

        if ((int) blackScreen.color.a == 0)
        {
            while (blackScreen.color.a < 1)
            {
                blackScreen.color += new Color(0, 0, 0, 0.05f);
                yield return new WaitForSeconds(0.1f);
            }
            gameOverPrompt.SetActive(true);
        }
        else
        {
            while (blackScreen.color.a > 0)
            {
                blackScreen.color -= new Color(0, 0, 0, 0.05f);
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
