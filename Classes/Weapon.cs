using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    private int damage;
    private bool canDamage;
    private Vector3 wp;
    private Vector2 vel;
    [SerializeField] Rigidbody2D body;
    [SerializeField] float travelSpeed;
    [SerializeField] float travelTime;
    [SerializeField] float knockback;
    [SerializeField] bool canPierce = false;

    GameObject[] hitList = new GameObject[5]; // max 5 targets piercing
    int currentListNum = 0;

    // Also for setting up constant variables
    public void setDamage(int dmg)
    {
        canDamage = true;
        damage = dmg;
        wp = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vel = -new Vector2(gameObject.transform.position.x - wp.x, gameObject.transform.position.y - wp.y).normalized * travelSpeed;

        float angleRad = Mathf.Atan2(wp.y - transform.position.y, wp.x - transform.position.x);
        float angleDeg = (180 / Mathf.PI) * angleRad - 90;
        transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);
        Invoke("selfDestruct", travelTime);
    }

    void Update()
    {
        body.velocity = vel;
    }

    private void selfDestruct()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" && canDamage)
        {
            if (!collision.GetComponent<Enemy>().isAlive())
            {
                return;
            }
            if (!canPierce)
            {
                canDamage = false;
                // Enemy takes damage using "damage", invoked from "Enemy" script
                collision.GetComponent<Enemy>().takeDamage(damage, gameObject);
                selfDestruct();
            }
            else
            {
                // Enemy takes damage using "damage", invoked from "Enemy" script
                for (int i = 0; i < hitList.Length; i++)
                {
                    if (collision.gameObject == hitList[i])
                    {
                        return;
                    }
                }

                if (currentListNum < hitList.Length-1)
                {
                    hitList[currentListNum] = collision.gameObject;
                    currentListNum += 1;
                }
                collision.GetComponent<Enemy>().takeDamage(damage, gameObject);  
            }   
        }
    }
}
