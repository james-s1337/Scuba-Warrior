using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATK;

public class EnemyAttacks : MonoBehaviour
{
    Dictionary<AttackType, int> atkDamages;

    public EnemyAttacks()
    {
        atkDamages = new Dictionary<AttackType, int>();

        // Set damages
        atkDamages.Add(AttackType.Spike, 10);
        atkDamages.Add(AttackType.Bubble, 15);
        atkDamages.Add(AttackType.Tentacle, 35);
        atkDamages.Add(AttackType.MegaBubble, 25);
        atkDamages.Add(AttackType.Sludge, 30);
        atkDamages.Add(AttackType.HeroTrident, 30);
    }

    public int getDamage(AttackType atk)
    {
        if (!atkDamages.ContainsKey(atk))
        {
            return 0;
        }

        return atkDamages[atk];
    }
}
