using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveCounter : MonoBehaviour
{
    [SerializeField] GameObject waveCounter;

    public void updateCounter(int wave)
    {
        waveCounter.GetComponent<TextMeshProUGUI>().text = "Wave " + wave;
    }
}
