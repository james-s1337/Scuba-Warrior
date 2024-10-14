using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHolder : MonoBehaviour
{
    public GameObject[] Objects;

    public GameObject find(string name)
    {
        for (int i = 0; i < Objects.Length; i++)
        {
            if (Objects[i].name == name)
            {
                return Objects[i];
            }
        }

        return null;
    }

    public int size()
    {
        return Objects.Length;
    }
}
