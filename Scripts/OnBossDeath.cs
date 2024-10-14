using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class OnBossDeath : MonoBehaviour
{
    [SerializeField] GameObject weap;
    SpawnManager sm;
    Random rand;

    void Start()
    {
        sm = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
    }

    public void dropWeap()
    {
        rand = new Random();
        int num = rand.Next(0, 2);
        Vector3 pos = gameObject.transform.position;
        Vector2 force = new Vector2(3f, 3f);
        if (pos.x >= 0)
        {
            force.x = -force.x;
        }


        GameObject copy = Instantiate(weap, pos, Quaternion.identity);
        copy.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
        if (sm.isLastBoss())
        {
            sm.showCrown();
        }
    }
}
