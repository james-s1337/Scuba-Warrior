using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InnerText : MonoBehaviour
{
    [SerializeField] GameObject baseText;
    [SerializeField] GameObject textObj;

    public void updateText()
    {
        textObj.GetComponent<TextMeshProUGUI>().text = baseText.GetComponent<TextMeshProUGUI>().text;
    }
}
