using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons
{
    Dictionary<string, int> weapons = new Dictionary<string, int>();

    // Add weapons
    // All ranged
    public Weapons()
    {
        weapons.Add("Classic Trident", 5);
        weapons.Add("Neptune's Trident", 30);
        weapons.Add("Speargun", 15);
        weapons.Add("Ghost Speargun", 20);
        weapons.Add("Pistol Claw", 20);
        weapons.Add("Hero's Trident", 45);
        weapons.Add("Mega Bubble", 20);
        weapons.Add("Kraken Slayer", 30);
        weapons.Add("Shark Tooth", 15);
    }

    public int getDamage(string weap)
    {
        if (!weapons.ContainsKey(weap))
        {
            return -1;
        }

        return weapons[weap];
    }
}
