using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    int maxDifficulty = 3;
    int minDifficulty = 0;
    int defaultDifficulty = 1;

    [SerializeField] int currentDifficulty;
    [SerializeField] TextMeshProUGUI diff;
    [SerializeField] TextMeshProUGUI waves;

    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject controlsMenu;

    [SerializeField] Image loadScreen;
    [SerializeField] GameObject loadMessage;
    [SerializeField] TextMeshProUGUI proTip;
    [SerializeField] AudioSource lobbyMusic;

    [SerializeField] string[] proTips;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetString("difficulty") == "" || PlayerPrefs.GetString("difficulty") == "Normal")
        {
            PlayerPrefs.SetString("difficulty", "Normal");
            currentDifficulty = 1;
        }
        else if (PlayerPrefs.GetString("difficulty") == "Easy")
        {
            currentDifficulty = 0;
        }
        else if (PlayerPrefs.GetString("difficulty") == "Hard")
        {
            currentDifficulty = 2;
        }
        else if (PlayerPrefs.GetString("difficulty") == "Survivalist")
        {
            currentDifficulty = 3;
        }

        setDifficulty();
    }

    // Update is called once per frame
    public void updateDifficulty(int direction) // 0 = Decrement, 1 = Increment
    {
        if (currentDifficulty == maxDifficulty && direction == 1)
        {
            return;
        }

        if (currentDifficulty == minDifficulty && direction == 0)
        {
            return;
        }

        if (direction == 1)
        {
            currentDifficulty++;
        }
        else
        {
            currentDifficulty--;
        }
        setDifficulty();
    }

    public void setDifficulty()
    {
        if (currentDifficulty == 1)
        {
            PlayerPrefs.SetString("difficulty", "Normal");
        }
        else if (currentDifficulty == 0)
        {
            PlayerPrefs.SetString("difficulty", "Easy");
        }
        else if (currentDifficulty == 2)
        {
            PlayerPrefs.SetString("difficulty", "Hard");
        }
        else if (currentDifficulty == 3)
        {
            PlayerPrefs.SetString("difficulty", "Survivalist");
        }

        int highestWave = PlayerPrefs.GetInt("highestWave" + PlayerPrefs.GetString("difficulty"));
        string diffName = PlayerPrefs.GetString("difficulty");

        diff.text = diffName;
        waves.text = "Highest Wave: " + highestWave;
    }

    public void startGame()
    {
        StartCoroutine(loadGameThread());
    }

    IEnumerator loadGameThread()
    {
        loadScreen.gameObject.SetActive(true);
        while (loadScreen.color.a < 1)
        {
            loadScreen.color += new Color(0, 0, 0, 0.05f);
            lobbyMusic.volume -= 0.005f;
            yield return new WaitForSeconds(0.05f);
        }
        loadMessage.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        string message = proTips[Random.Range(0, proTips.Length)];
        proTip.text = "Pro Tip: " + message;
        proTip.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("Arena");
    }

    public void openMain()
    {
        controlsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void openControls()
    {
        mainMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }
}
