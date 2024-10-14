using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Champion : MonoBehaviour
{
    [SerializeField] GameObject trident;
    [SerializeField] GameObject megaBubble;
    [SerializeField] Enemy e;

    GameObject spawnManager;
    SpawnManager sm;
    GameObject playerObj;
    PlayerController player;
    Rigidbody2D body;
    Animator animController;
    public UnityEvent onChargeBegin, onChargeDone;
    int attacks = 4;
    bool canAttack = true;
    bool canCharge = true;
    bool canMove = false;
    public float speed = 12f;
    public float attackCooldown = 1.2f;
    public float chargeCooldown = 2f;
    public float chargeTime = 1f;

    void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.GetComponent<PlayerController>();
        spawnManager = GameObject.FindGameObjectWithTag("SpawnManager");
        body = GetComponent<Rigidbody2D>();
        animController = GetComponent<Animator>();
        sm = spawnManager.GetComponent<SpawnManager>();

        Invoke("toggleMove", 0.5f);
    }

    void toggleMove()
    {
        canMove = true;
    }

    void Update()
    {
        if (!e.isAlive() || trident == null)
        {
            body.velocity = new Vector2();
            return;
        }

        if (player.checkAlive() == false || !canMove)
        {
            return;
        }

        if (canCharge)
        {
            canCharge = false;
            chargeToPlayerPos();
        }

        if (canAttack)
        {
            canAttack = false;
            int attackNum = Random.Range(0, attacks);
            if (attackNum == 0)
            {
                animController.SetBool("isExploding", true);
            }
            else
            {
                animController.SetBool("isAttacking", true);
                throwTrident();
            }
        }
    }

    void resetAttack()
    {
        canAttack = true;
    }

    void resetAttackAnim()
    {
        animController.SetBool("isAttacking", false);
    }

    IEnumerator resetCharge()
    {
        yield return new WaitForSeconds(chargeTime); // Charge Time
        body.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(chargeCooldown);
        canCharge = true;
    }

    void chargeToPlayerPos()
    {
        // go to player's last position
        // do this every 4 seconds
        body.velocity = -new Vector2(gameObject.transform.position.x - playerObj.transform.position.x, gameObject.transform.position.y - playerObj.transform.position.y).normalized * speed;
        StartCoroutine(resetCharge());
    }

    public void throwTrident()
    {
        // normal trident throw
        GameObject newProj = Instantiate(trident, gameObject.transform.position, Quaternion.identity);
        newProj.GetComponent<Projectile>().modifyAttackPower(sm.difficultyMod);
        Invoke("resetAttackAnim", 0.1f);
        Invoke("resetAttack", attackCooldown);
    }

    public void startTridentAOE()
    {
        StartCoroutine(tridentAOE());
    }

    IEnumerator tridentAOE()
    {
        // pause for 1 second
        // Throw 8 tridents around in a circle (0.1 nsecond delay between each)
        canMove = false;
        body.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(0.2f);

        float deg = 0;
        float factor = 22.5f;
        for (int i = 0; i < 16; i++)
        {
            GameObject newProj = Instantiate(megaBubble, gameObject.transform.position, Quaternion.identity);
            newProj.GetComponent<Projectile>().modifyAttackPower(sm.difficultyMod);
            yield return new WaitForSeconds(0.05f);

            if (newProj != null) {
                newProj.GetComponent<Projectile>().changeDirection(new Vector2(transform.position.x + Mathf.Cos(deg) * 5, transform.position.y + Mathf.Sin(deg) * 5));
            }

            deg += factor;
        }

        canMove = true;
        animController.SetBool("isExploding", false);
        Invoke("resetAttack", attackCooldown);
    }
}
