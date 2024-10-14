using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    private float defaultSpeed = 10.0f;
    private float speed;

    private float defaultJumpPower = 9.0f;
    private float jumpPower;

    private int defaultMaxJumps = 2;
    private int maxJumps;

    private int currentJumps;

    private int health;
    private int maxHealth = 150;

    private int defaultDamage = 1; // Melee damage
    private int damage;

    private bool hasBoost;
    private Boosts boosts;

    private string weap;
    public PlayerStats(string weapon) {
        // default values
        speed = defaultSpeed;
        jumpPower = defaultJumpPower;
        maxJumps = defaultMaxJumps;
        currentJumps = 0;

        health = maxHealth;
        damage = defaultDamage;
        hasBoost = false;
        boosts = new Boosts();

        weap = weapon; // Starter weapon
    }

    public float getSpeed()
    {
        return speed;
    }

    public float getJumpPower()
    {
        return jumpPower;
    }

    public int getMaxJumps()
    {
        return maxJumps;
    }

    public void setMaxJumps(int newCap)
    {
        maxJumps = newCap;
    }

    public void setJumpPower(float newPower)
    {
        jumpPower = newPower;
    }

    public void resetJumps()
    {
        currentJumps = 0;
    }

    public void addJump()
    {
        currentJumps += 1;
    }

    public int getCurrentJumps()
    {
        return currentJumps;
    }

    public int getMaxHealth()
    {
        return maxHealth;
    }

    public bool isAlive()
    {
        return health > 0;
    }

    public void takeDamage(int damage)
    {
        damage = Mathf.Clamp(damage, 0, health);
        health -= damage;
    }

    public int getDamage()
    {
        return damage;
    }

    public void setDamage(int newDamage)
    {
        damage = newDamage;
    }

    // For health bar
    public float getHealthPercentage()
    {
        if (health <= 0)
        {
            return 0;
        }

        return (float) health / (float) maxHealth;
    }

    public void heal(int amount)
    {
        int healAmount = amount;
        healAmount = Mathf.Clamp(healAmount, 0, maxHealth - health);

        health += healAmount; 
    }

    // Implement later
    public void addSpeedBoost()
    {

    }

    public void addJumpBoost()
    {

    }

    public void addDamageBoost()
    {

    }

    public string getWeap()
    {
        return weap;
    }

    public void setWeap(string newWeap)
    {
        weap = newWeap;
    }
}
