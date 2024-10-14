using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class StartPrompt : MonoBehaviour
{
    [SerializeField] GameObject startPrompt;
    public UnityEvent textUpdated;

    public void updatePrompt(int num)
    {
        if (num == 0)
        {
            startPrompt.GetComponent<TextMeshProUGUI>().text = "START!";
        }
        else if (num == -1)
        {
            startPrompt.GetComponent<TextMeshProUGUI>().text = "";
        }
        else
        {
            startPrompt.GetComponent<TextMeshProUGUI>().text = num.ToString();
        }

        textUpdated?.Invoke();
    }

    public void changeYellow()
    {
        startPrompt.GetComponent<TextMeshProUGUI>().color = new Color(125, 125, 0);
    }

    public void changeWhite()
    {
        startPrompt.GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255);
    }
}
