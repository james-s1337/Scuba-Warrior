using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] int healAmount;
    [SerializeField] float decayTime;
    Rigidbody2D body;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;
        Invoke("selfDestruct", decayTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().heal(healAmount);
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Platform") {
            body.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    void selfDestruct()
    {
        Destroy(gameObject);
    }
}
