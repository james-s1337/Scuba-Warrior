using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDrop : MonoBehaviour
{
    [SerializeField] float delay;
    [SerializeField] string weapName;
    Weapons weapons;
    Rigidbody2D body;

    void Start()
    {
        weapons = new Weapons();
        body = GetComponent<Rigidbody2D>();
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        Invoke("delete", delay);
    }

    void delete()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            PlayerController controller = other.GetComponent<PlayerController>();

            if (weapons.getDamage(weapName) <= weapons.getDamage(controller.getWeap()))
            {
                return;
            }

            controller.changeWeap(weapName);
            delete();
        }

        if (other.tag == "Ground")
        {
            body.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
}
