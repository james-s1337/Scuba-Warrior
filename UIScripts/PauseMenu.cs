using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameManager gm;
    public UnityEvent onPaused, onUnpaused;
    // Update is called once per frame
    void Update()
    {
        if (gm.gameOver)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.activeSelf == false)
            {
                pauseMenu.SetActive(true);
                onPaused?.Invoke();
            }
            else
            {
                pauseMenu.SetActive(false);
                onUnpaused?.Invoke();
            }
        }
    }
}
