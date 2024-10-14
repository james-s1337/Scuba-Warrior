using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATK;

public class Projectile : MonoBehaviour
{
    private Vector2 vel;
    [SerializeField] int damage;
    private bool canDamage = false;
    bool canGetStuck = false;

    [SerializeField] Rigidbody2D body;
    [SerializeField] float travelSpeed;
    [SerializeField] float travelTime;
    [SerializeField] AttackType attack; // To determine damage

    GameObject target; // Fixed target for easier scripting and processing
    void Start()
    {
        // Very similar to "Weapon" class
        target = GameObject.FindGameObjectWithTag("Player");
        changeDirection(target.transform.position);

        Invoke("unlock", 0.3f);
        Invoke("selfDestruct", travelTime);
    }

    void unlock()
    {
        canGetStuck = true;
    }

    private void selfDestruct()
    {
        Destroy(gameObject);
    }

    public AttackType getAttackType()
    {
        return attack;
    }

    public void modifyAttackPower(float mod)
    {
        damage = (int) (damage * mod);
        canDamage = true;
    }

    public void changeDirection(Vector2 pos)
    {
        Vector3 wp = pos;
        vel = -new Vector2(gameObject.transform.position.x - wp.x, gameObject.transform.position.y - wp.y).normalized * travelSpeed;
        if (body.gravityScale == 0)
        {
            float angleRad = Mathf.Atan2(wp.y - transform.position.y, wp.x - transform.position.x);
            float angleDeg = (180 / Mathf.PI) * angleRad - 90;
            transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);
        }
        body.velocity = vel;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && canDamage)
        {
            // Enemy takes damage using "damage", invoked from "Enemy" script
            collision.GetComponent<PlayerController>().takeDamage(damage, gameObject);
            selfDestruct();
        }

        if (collision.tag == "Ground" || collision.tag == "Platform")
        {
            if (body.gravityScale != 0 && canGetStuck)
            {
                constrict();
            }
        }
    }

    void constrict()
    {
        body.constraints = RigidbodyConstraints2D.FreezeAll;
    }
}
