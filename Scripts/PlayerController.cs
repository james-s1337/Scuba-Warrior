using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// Controls the player and their collisions (and events related to the collisions such as boosts)
/*
 * A/Left Arrow for moving left
 * D/Right Arrow for moving right
 * Space to jump
 * Q to dash
 * S to go down a platform
 */
public class PlayerController : MonoBehaviour
{
    enum FaceDirection
    {
        Right,
        Left
    }
    PlayerStats stats;

    bool canDash;
    bool isDashing;
    bool canJump;
    bool isAlive;
    bool canMove;
    bool canTurn;

    float jumpCooldown = 0.3f;
    float dashCooldown = 2.0f;
    float dashDuration = 0.2f;
    float defaultDashForce = 2f; // Dash is 2x the normal walking speed
    Vector2 vel;
    Vector3 wp; // World pos of mouse

    Rigidbody2D rb2d;
    BoxCollider2D plrCollider;
    GameObject currentPlatform;
    SpriteRenderer sprite;
    [SerializeField] SpriteRenderer weapArmSprite;
    [SerializeField] SpriteRenderer weapSprite;
    [SerializeField] SpriteRenderer crownSprite;
    [SerializeField] GameObject weapHolder;
    [SerializeField] ObjectHolder weapList;
    Animator animController;
    FaceDirection dir;

    [SerializeField] ParticleSystem hurtEffect;
    [SerializeField] AudioSource hurtSound;

    [SerializeField] ParticleSystem healEffect;
    [SerializeField] AudioSource healSound;

    [SerializeField] ParticleSystem deathEffect;

    [SerializeField] string weapon;

    public UnityEvent<GameObject> onHitWithReference;
    public UnityEvent onPlayerDeath;
    public UnityEvent<Sprite> onWeapChange; 
    void Start()
    {
        canJump = true;
        canDash = true;
        isDashing = false;
        isAlive = true;
        canMove = true;
        canTurn = true;
        stats = new PlayerStats(weapon);
        
        plrCollider = gameObject.GetComponent<BoxCollider2D>();
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        currentPlatform = null;
        dir = FaceDirection.Right;
        vel = new Vector2(rb2d.velocity.x, rb2d.velocity.y);

        sprite = GetComponent<SpriteRenderer>();
        animController = GetComponent<Animator>();

        healSound.volume = 0;
        changeWeap(weapon);
    }

    void Update()
    {
        if (!isAlive || !canMove)
        {
            return;
        }

        var hInput = Input.GetAxis("Horizontal");

        // For the sprite
        if (canTurn)
        {
            if (hInput > 0)
            {
                animController.SetBool("isWalking", true);
                sprite.flipX = false;
                weapArmSprite.flipX = false;
                crownSprite.flipX = false;
                crownSprite.transform.localPosition = new Vector3(0.154f, 0.685f, -2);
                dir = FaceDirection.Right;
            }
            else if (hInput < 0)
            {
                animController.SetBool("isWalking", true);
                sprite.flipX = true;
                weapArmSprite.flipX = true;
                crownSprite.flipX = true;
                crownSprite.transform.localPosition = new Vector3(-0.154f, 0.685f, -2);
                dir = FaceDirection.Left;
            }
            else
            {
                animController.SetBool("isWalking", false);
            }
        }

        if (rb2d.velocity.y < 0)
        {
            animController.SetBool("isJumping", true);
        }

        // For moving left to right
        float moveForce = hInput * stats.getSpeed();
        if (!isDashing)
        {
            vel = new Vector2(moveForce, rb2d.velocity.y);
            rb2d.velocity = vel;
        }

        // For jumping
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            if (stats.getMaxJumps() > stats.getCurrentJumps())
            {
                canJump = false;
                animController.SetBool("isJumping", true);
                stats.addJump();
                Vector2 jumpForce = new Vector2(0, stats.getJumpPower()); 
                rb2d.velocity = new Vector2(rb2d.velocity.x, 1f);
                rb2d.AddForce(jumpForce, ForceMode2D.Impulse);
                Invoke("resetJumpDebounce", jumpCooldown);
            }          
        }

        // For dashing
        if (Input.GetKeyDown(KeyCode.Q) && canDash)
        {
            canDash = false;
            Vector2 dashForce = new Vector2(stats.getSpeed(), 0) * defaultDashForce;

            if (dir == FaceDirection.Left) {
                dashForce *= -1;
            }

            rb2d.AddForce(dashForce, ForceMode2D.Impulse);
            isDashing = true;
            Invoke("resetDashing", dashDuration);
            Invoke("resetDashDebounce", dashCooldown);
        }

        // Detect if trying to go down a platform
        if (Input.GetKey(KeyCode.S))
        {
            if (currentPlatform != null)
            {
                StartCoroutine(disableCollision());
            }
        }

        wp = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float angleRad = Mathf.Atan2(wp.y - weapHolder.transform.position.y, wp.x - weapHolder.transform.position.x);
        float angleDeg = (180 / Mathf.PI) * angleRad - 90;

        weapHolder.transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);

        if (dir == FaceDirection.Right)
        {
            weapHolder.transform.localPosition = new Vector3(weapArmSprite.transform.localPosition.x - 0.35f, weapHolder.transform.localPosition.y, weapHolder.transform.localPosition.z);
        }
        else
        {
            weapHolder.transform.localPosition = new Vector3(weapArmSprite.transform.localPosition.x + 0.35f, weapHolder.transform.localPosition.y, weapHolder.transform.localPosition.z);
        }
    }
    public void disableMovement()
    {
        rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        canMove = false;
    }

    public void enableMovement()
    {
        canMove = true;
    }

    public void disableTurn()
    {
        canTurn = false;
    }
    public void enableTurn()
    {
        canTurn = true;
    }

    void resetJumpDebounce()
    {
        canJump = true;
    }

    void resetDashDebounce()
    {
        canDash = true;
    }

    void resetDashing()
    {
        isDashing = false;
    }

    public bool checkStatStatus()
    {
        return stats != null;
    }

    public string getWeap()
    {
        return stats.getWeap();
    }

    public float getHealthPercentage()
    {
        return stats.getHealthPercentage();
    }

    public bool checkAlive()
    {
        return stats.isAlive();
    }

    public void takeDamage(int amount, GameObject sender)
    {
        if (!isAlive)
        {
            return;
        }

        stats.takeDamage(amount);
        // Particle effects/sprite flashing + a little bit of knockback
        sprite.color = new Color(255, 0, 0);
        StopCoroutine("resetColor");
        StartCoroutine("resetColor");

        onHitWithReference?.Invoke(sender);
        hurtEffect.Play();
        if (!checkAlive())
        {
            // Extra flashing
            // Trigger "gameOver" in the "GameManager" script (UI pop-up, reload scene) -> All UI pop-ups relating to the game will be invoked in "GameManager"
            isAlive = false;
            onPlayerDeath?.Invoke();
            rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
            StartCoroutine(deathFlash());
            hurtSound.pitch = 0.5f;
            deathEffect.Play();
        }

        hurtSound.Play();
    }

    IEnumerator resetColor()
    {
        yield return new WaitForSeconds(0.3f);
        sprite.color = new Color(255, 255, 0);
    }

    IEnumerator deathFlash()
    {
        yield return new WaitForSeconds(0.2f);
        sprite.color = new Color(0, 0, 0);
        yield return new WaitForSeconds(0.2f);
        sprite.color = new Color(255, 255, 0);
        yield return new WaitForSeconds(0.2f);
        sprite.color = new Color(0, 0, 0);
        yield return new WaitForSeconds(0.2f);
        sprite.color = new Color(255, 255, 0);
        yield return new WaitForSeconds(0.2f);
        sprite.color = new Color(0, 0, 0, 0);
        weapArmSprite.gameObject.SetActive(false);
    }

    public void heal(int amount)
    {
        if (!isAlive)
        {
            return;
        }

        stats.heal(amount);
        healEffect.Play();
        healSound.Play();
    }

    public void changeWeap(string newWeap)
    {
        weapon = newWeap;
        stats.setWeap(weapon);
        // Update weapon visual
        GameObject weap = weapList.find(weapon);
        onWeapChange?.Invoke(weap.transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite);
        healSound.Play(); // Replace with new sound later
        StartCoroutine(resetHealSound()); 
    }

    IEnumerator resetHealSound()
    {
       yield return new WaitUntil(() => healSound.isPlaying == false);
        healSound.volume = 0.5f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string tag = collision.gameObject.tag;
        if (tag == "Ground")
        {
            stats.resetJumps();
            animController.SetBool("isJumping", false);
        }  

        if (tag == "Platform")
        {
            currentPlatform = collision.gameObject;
            Vector3 validDirection = Vector3.up;  // What you consider to be upwards
            float contactThreshold = 90f;          // Acceptable difference in degrees

            for (int k = 0; k < collision.contacts.Length; k++)
            {
                if (Vector3.Angle(collision.contacts[k].normal, validDirection) <= contactThreshold)
                {
                    // Collided with a surface facing mostly upwards
                    stats.resetJumps();
                    animController.SetBool("isJumping", false);
                }
            }
        }

        if (tag == "Wall")
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x/10, stats.getSpeed()/2);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (tag == "Ground" || tag == "Platform")
        {
            stats.resetJumps();
            animController.SetBool("isJumping", false);

            if (tag == "Platform")
            {
                currentPlatform = collision.gameObject;
            }
        }
    }

    // For going under platforms
    private void OnCollisionExit2D(Collision2D collision)
    {
        string tag = collision.gameObject.tag;
        if (tag == "Platform")
        {
            currentPlatform = null;
        }
    }

    private IEnumerator disableCollision()
    {
        BoxCollider2D boxCollider = currentPlatform.GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(plrCollider, boxCollider);
        yield return new WaitForSeconds(1);
        Physics2D.IgnoreCollision(plrCollider, boxCollider, false);
    }
}
