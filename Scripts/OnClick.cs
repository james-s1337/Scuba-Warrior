using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Primarily for controlling the weapon
public class OnClick : MonoBehaviour
{
    PlayerController player;
    [SerializeField] Animator animController;
    Weapons weapons;
    [SerializeField] GameObject weapTemplates;
    [SerializeField] AudioSource weapSound;
    [SerializeField] Transform spawnPos;

    private bool canShoot;
    private float defaultCooldown = 0.3f;

    public UnityEvent onThrowStart, onThrowDone;
    void Start()
    {
        player = gameObject.GetComponent<PlayerController>();
        weapons = new Weapons();
        canShoot = true;
    }

    void Update()
    {
        // Check if stats loaded in
        if (!player.checkStatStatus() || !player.checkAlive())
        {
            return;
        }

        if (Time.timeScale == 0) // Paused
        {
            return;
        }

        int weapDamage = weapons.getDamage(player.getWeap());
        // No weapon found
        if (weapDamage == -1)
        {
            return;
        }

        if (Input.GetMouseButton(0) && canShoot)
        {
            canShoot = false;
            animController.SetBool("isThrowing", true);
            onThrowStart?.Invoke();
            // Spawn in weapon projectile (each weapon will have an "OnHit" script with damage value that can be changed only through here)
            GameObject weap = weapTemplates.GetComponent<ObjectHolder>().find(player.getWeap());
            if (!weap)
            {
                return;
            }
            // Create weapon into world
            StartCoroutine("spawnWeap");
            GameObject newWeap = Instantiate(weap, spawnPos.position, Quaternion.identity);
            // Every weapon will have a "sprite" object which is the visual sprite, but the actual hitbox is the parent
            newWeap.GetComponent<Weapon>().setDamage(weapDamage);
            Invoke("resetShoot", defaultCooldown);

            weapSound.time = 0.1f;
            weapSound.Play();
        }
    }

    public void resetShoot()
    {
        canShoot = true;
        animController.SetBool("isThrowing", false);
        onThrowDone?.Invoke();
    }
    IEnumerator spawnWeap()
    {
        // Animation delay
        yield return new WaitForSeconds(0.2f);
    }
}
