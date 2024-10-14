using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] Image bar;
    [SerializeField] float defaultWidth;
    [SerializeField] GameObject player;
    PlayerController controller;

    void Start()
    {
        controller = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        float percentage = controller.getHealthPercentage();
        bar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, defaultWidth * percentage);
    }
}
