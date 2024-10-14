using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATK; // For all attacks
using UnityEngine.Events;
using Random = System.Random;

namespace ATK
{
    public enum AttackType
    {
        Spike,
        Bubble,
        Tentacle, // Exclusive to The Kraken
        MegaBubble, // Exclusive to King Crab
        Sludge, // Exclusive to Kraken
        HeroTrident // Exclusive to final boss
    }
}

public class Enemy : MonoBehaviour
{
    enum EnemyType // In "EnemyManager", make sure to spawn "Anchored" at the bottom instead of the sides
    {
        Melee, // Swim towards player
        Ranged, // Swim and shoot towards player
        Anchored, // Does not move at all (but can shoot if it has that type of attack
        Jumper
    }
    [SerializeField] int health;
    [SerializeField] float speed;
    [SerializeField] int damage; // Melee damage
    [SerializeField] float attackSpeed;
    [SerializeField] float touchAtkSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float jumpCooldown;
    [SerializeField] EnemyType eType; // Influences behavior controlled in "Update"
    [SerializeField] AttackType[] attacks; // Only used as a list of attacks to choose from, ranged attacks only
    [SerializeField] bool isBoss;

    [SerializeField] ParticleSystem hurtEffect;
    AudioSource hurtSound;
    [SerializeField] float hurtSoundDelay;

    [SerializeField] ParticleSystem deathEffect;

    GameObject boosts;
    [SerializeField] string healType; // SmallHeart, LargeHeart etc.
    GameObject heart;

    GameObject spawnManager;
    SpawnManager sm;

    Animator animController;
    Rigidbody2D body;
    BoxCollider2D eCollider;
    SpriteRenderer sprite;
    BoxCollider2D platformCollider;
    ObjectHolder projectiles;

    GameObject target;
    PlayerController player;

    private bool isDead; // If true, then cease all operations
    private bool canAttack; // If false, cant attack again for a certain amount of time and cannot move for same amount of time
    private bool canTouchAttack;
    private bool canJump; // Only for jumpers
    private bool landed;
    private bool shooting;
    private Vector2 vel;
    private int maxHP;

    public UnityEvent<GameObject> onHit;
    public UnityEvent onDeath;
    public bool canMove;
    public bool isChamp;

    Random rand;
    void Start()
    {
        isDead = false; 
        canAttack = true; 
        canTouchAttack = true;
        canJump = false;
        landed = false;
        canMove = false;

        projectiles = GameObject.FindGameObjectWithTag("ProjManager").GetComponent<ObjectHolder>();
        animController = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        eCollider = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();

        rand = new Random();
        target = GameObject.FindGameObjectWithTag("Player");
        player = target.GetComponent<PlayerController>();

        hurtSound = GetComponent<AudioSource>();
        boosts = GameObject.FindGameObjectWithTag("Boosts");
        heart = boosts.GetComponent<ObjectHolder>().find(healType);

        spawnManager = GameObject.FindGameObjectWithTag("SpawnManager");
        sm = spawnManager.GetComponent<SpawnManager>();

        health = (int) (health * sm.difficultyMod);
        damage = (int) (damage * sm.difficultyMod);
        health = (int) (health * sm.healthModifier);
        maxHP = health;

        Invoke("enableMovement", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead || projectiles == null)
        {
            body.velocity = new Vector2();
            return;
        }

        if (player.checkAlive() == false || !canMove)
        {
            return;
        }

        // Enemy boundaries
        // XY Boundary
        gameObject.transform.position = new Vector3(Mathf.Clamp(gameObject.transform.position.x, -21, 21), Mathf.Clamp(gameObject.transform.position.y, 0, 35), gameObject.transform.position.z);

        // Move towards player if EnemyType is not "Anchored"
        // If EnemyType is "Jumper" then only move towards the player when jumping (also add jumping movement)
        if (canTouchAttack && !isChamp) // Can only move if not attacking in any way
        {
            vel = -new Vector2(gameObject.transform.position.x - target.transform.position.x, gameObject.transform.position.y - target.transform.position.y).normalized * speed;
            if (eType != EnemyType.Anchored && eType != EnemyType.Jumper)
            {
                // Move towards target
                // Control sprite direction here
                if (canAttack || isBoss)
                {
                    body.velocity = vel;
                }       
            }
            else if (eType == EnemyType.Jumper && canJump) // Make sure for jumping enemies, make them slow
            {
                canJump = false;
                landed = false;
                animController.SetBool("isJumping", true);
                // Move towards target only on the x axis
                // Control sprite direction here
                body.AddForce(new Vector2(0, jumpPower * body.mass), ForceMode2D.Impulse);

                body.velocity = new Vector2(vel.x * (1 + body.mass / 100), body.velocity.y);
            }

            if (eType == EnemyType.Jumper && Mathf.Abs(gameObject.transform.position.x - target.transform.position.x) <= 0.2f)
            {
                if (!canJump)
                {
                    body.velocity = new Vector2(0, body.velocity.y);
                }
            }
        }

        if (canAttack && attacks.Length > 0) // Check if its melee, if it is, then dont perform ranged attack (ofc)
        {
            if (!isChamp)
            {
                canAttack = false;
                animController.SetBool("isRangedAttacking", true);

                if (eType == EnemyType.Jumper)
                {
                    rangedAttack();
                }

                if (eType == EnemyType.Anchored)
                {
                    int atkNum = rand.Next(0, attacks.Length);
                    GameObject proj = projectiles.find(attacks[atkNum].ToString());
                    GameObject newProj = Instantiate(proj, gameObject.transform.position, Quaternion.identity);
                    newProj.GetComponent<Projectile>().modifyAttackPower(sm.difficultyMod);
                    Invoke("resetAttack", attackSpeed);
                }
            }  
        }

        // Sprite
        if (eType != EnemyType.Anchored && canTouchAttack)
        {
            if (canMove)
            {
                if (body.velocity.x >= 0)
                {
                    sprite.flipX = false;
                }
                else
                {
                    sprite.flipX = true;
                }
            }
        }
    }

    public void disableMovement()
    {
        canMove = false;
    }

    public void enableMovement()
    {
        canMove = true;
    }

    public void resetRangedAttacking()
    {
        animController.SetBool("isRangedAttacking", false);
        shooting = false;
    }

    public void rangedAttack()
    {
        if (!shooting)
        {
            shooting = true;
        }
        else
        {
            return;
        }

        int atkNum = rand.Next(0, attacks.Length);
        GameObject proj = projectiles.find(attacks[atkNum].ToString());
        GameObject newProj = Instantiate(proj, gameObject.transform.position, Quaternion.identity);
        newProj.GetComponent<Projectile>().modifyAttackPower(sm.difficultyMod);
        Invoke("resetAttack", attackSpeed);
    }

    private void resetAttack()
    {
        canAttack = true;
        shooting = false;
        animController.SetBool("isRangedAttacking", false);
    }

    public bool isAlive()
    {
        return isDead == false;
    }

    public int getCurrentHP()
    {
        return health;
    }

    public int getMaxHP()
    {
        return maxHP;
    }

    public void takeDamage(int amount, GameObject obj)
    {
        if (isDead)
        {
            return;
        }

        health -= amount;

        sprite.color = new Color(255, 0, 0);
        StopCoroutine("resetColor");
        StartCoroutine("resetColor");

        if (eType != EnemyType.Anchored && health > 0)
        {
            onHit?.Invoke(obj);
        }

        hurtEffect.Play();
        if (health <= 0)
        {
            isDead = true;
            hurtSound.pitch = 0.7f;
            sm.decrementEnemies();
            StartCoroutine("deathEffects");
            deathEffect.Play();

            // Have a chance to drop a heal
            int dropNum = rand.Next(0, 10); // 10% chance to drop
            int healDrop = 3; // Random number
            if (dropNum == healDrop && !sm.isSurvivalist)
            {
                Instantiate(heart, gameObject.transform.position, Quaternion.identity);
            }
            
            if (!sm.isSurvivalist)
            {
                if (isBoss && dropNum == healDrop + 2) // Random number, 20% chance to drop heart for bosses
                {
                    Instantiate(heart, gameObject.transform.position, Quaternion.identity);
                }
            }
        }

        hurtSound.Stop();
        hurtSound.time = hurtSoundDelay;
        hurtSound.Play();
    }

    IEnumerator deathEffects()
    {
        yield return new WaitForSeconds(0.1f);
        sprite.color = new Color(0, 0, 0);
        yield return new WaitForSeconds(0.1f);
        sprite.color = new Color(255, 0, 0);
        yield return new WaitForSeconds(0.1f);
        sprite.color = new Color(0, 0, 0);
        yield return new WaitForSeconds(0.1f);
        sprite.color = new Color(255, 0, 0);
        yield return new WaitForSeconds(0.1f);
        sprite.color = new Color(0, 0, 0, 0);
        yield return new WaitForSeconds(0.2f);
        onDeath?.Invoke();
        Destroy(gameObject);
    }

    IEnumerator resetColor()
    {
        yield return new WaitForSeconds(0.3f);
        sprite.color = new Color(255, 255, 255);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead || player.checkAlive() == false)
        {
            return;
        }

        GameObject obj = collision.gameObject;
        if (obj.tag == "Player" && canTouchAttack)
        {
            meleeAttack(obj);
            animController.SetBool("isAttacking", true);
            body.velocity = -body.velocity / 10; // Back up a bit   
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isDead || player.checkAlive() == false)
        {
            return;
        }

        GameObject obj = collision.gameObject;
        if (obj.tag == "Player" && canTouchAttack)
        {
            meleeAttack(obj);
            animController.SetBool("isAttacking", true);
            body.velocity = -body.velocity / 10; // Back up a bit   
        }
    }

    void meleeAttack(GameObject obj)
    {
        canTouchAttack = false;
        PlayerController controller = obj.GetComponent<PlayerController>();
        controller.takeDamage(damage, gameObject);

        Invoke("resetTouchAttack", touchAtkSpeed);
    }

    // Exclusively for jumpers
    private void OnCollisionEnter2D(Collision2D collision) 
    {
        GameObject obj = collision.gameObject;
        if (obj.tag == "Enemy")
        {
            Physics2D.IgnoreCollision(eCollider, collision.gameObject.GetComponent<BoxCollider2D>());
        }

        if (isDead || player.checkAlive() == false)
        {
            return;
        }

        if (obj.tag == "Ground" || obj.tag == "Platform")
        {
            if (eType == EnemyType.Jumper && !landed)
            {
                if (gameObject.transform.position.y > target.transform.position.y && obj.tag == "Platform")
                {
                    platformCollider = collision.gameObject.GetComponent<BoxCollider2D>();
                    animController.SetBool("isJumping", true);
                    StartCoroutine("clipPlatform");
                }
                else
                {
                    Vector3 validDirection = Vector3.up;  // What you consider to be upwards
                    float contactThreshold = 90f;          // Acceptable difference in degrees

                    if (obj.tag == "Platform")
                    {
                        for (int k = 0; k < collision.contacts.Length; k++)
                        {
                            if (Vector3.Angle(collision.contacts[k].normal, validDirection) <= contactThreshold)
                            {
                                // Collided with a surface facing mostly upwards
                                landed = true;
                                animController.SetBool("isJumping", false);
                                Invoke("resetJump", jumpCooldown);
                            }
                        }    
                    }
                    else if (obj.tag == "Ground")
                    {
                        landed = true;
                        animController.SetBool("isJumping", false);
                        Invoke("resetJump", jumpCooldown);
                    }
                }     
            }
        }

        if (obj.tag == "Player" && canTouchAttack)
        {
            if (eType == EnemyType.Jumper) {
                jumperAttack(obj);
            }
            else
            {
                meleeAttack(obj);
            } 
        }
    }

    IEnumerator disableCollision(BoxCollider2D boxCollider)
    {
        Physics2D.IgnoreCollision(eCollider, boxCollider);
        yield return new WaitForSeconds(0.4f);
        Physics2D.IgnoreCollision(eCollider, boxCollider, false);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead || player.checkAlive() == false)
        {
            return;
        }

        GameObject obj = collision.gameObject;
        if (obj.tag == "Player" && canTouchAttack)
        {
            jumperAttack(obj);
        }

        if (obj.tag == "Ground" || obj.tag == "Platform")
        {
            if (eType == EnemyType.Jumper && jumpPower == 0)
            {
                landed = true;
                animController.SetBool("isJumping", false);
                if (!canJump)
                {
                    Invoke("resetJump", jumpCooldown);
                }
            }
        }
    }

    void jumperAttack(GameObject obj)
    {
        if (!landed)
        {
            landed = true;
            animController.SetBool("isJumping", false);
            Invoke("resetJump", jumpCooldown);
        }
        canTouchAttack = false;
        PlayerController controller = obj.GetComponent<PlayerController>();
        controller.takeDamage(damage, gameObject);

        body.velocity = new Vector3(-vel.x * 2 / 10, body.velocity.y); // Back up a bit
        Invoke("resetTouchAttack", touchAtkSpeed);
    }

    IEnumerator clipPlatform()
    {
        Physics2D.IgnoreCollision(eCollider, platformCollider);
        yield return new WaitForSeconds(1f);
        Physics2D.IgnoreCollision(eCollider, platformCollider, false);

    }

    private void resetTouchAttack()
    {
        body.velocity = new Vector2(0, body.velocity.y);
        canTouchAttack = true;
        animController.SetBool("isAttacking", false);
    }

    private void resetJump()
    {
        canJump = true;
    }
}
