using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameObject subject;
    void Start()
    {
        
    }

    void LateUpdate()
    {
        gameObject.transform.position = new Vector3(subject.transform.position.x, subject.transform.position.y, -10);
    }
}
