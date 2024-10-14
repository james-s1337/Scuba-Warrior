using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class holding the information of all the boosts in the game
public class Boosts
{
    // In seconds
    private float dmgBoostTime = 5f;
    // Multiplier
    private float dmgBoost = 2f;

    private float spdBoostTime;
    private float spdBoost;

    private float jmpBoostTime;
    private float jmpBoost;

    // A fraction
    private float healAmount = 0.25f;
    public Boosts()
    {

    }

    public float getDamageBoostTime()
    {
        return dmgBoostTime;
    }

    public float getDamageBoost()
    {
        return dmgBoost;
    }

    public float getSpeedBoostTime()
    {
        return spdBoostTime;
    }

    public float getSpeedBoost()
    {
        return spdBoost;
    }

    public float getJumpBoostTime()
    {
        return jmpBoostTime;
    }

    public float getJumpBoost()
    {
        return jmpBoost;
    }

    public float getHealAmount()
    {
        return healAmount;
    }
}
