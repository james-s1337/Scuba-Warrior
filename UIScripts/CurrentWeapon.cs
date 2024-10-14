using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentWeapon : MonoBehaviour
{
    [SerializeField] GameObject player;
    PlayerController controller;

    void Start()
    {
        controller = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = "> " + controller.getWeap();
    }
}
