using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DifficultyController : MonoBehaviour
{
    public UnityEvent<int> onClickUpdated;
    [SerializeField] int direction;

    public void onClick()
    {
        onClickUpdated?.Invoke(direction);
    }
}

