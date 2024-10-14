using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemiesLeft : MonoBehaviour
{
    [SerializeField] GameObject enemiesLeft;

    public void updateCounter(int enemies)
    {
        enemiesLeft.GetComponent<TextMeshProUGUI>().text = enemies.ToString() + " Enemies Left";
    }
}
