using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Rnd = UnityEngine.Random;
using UnityEngine.Events;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject[] normalEnemies;
    [SerializeField] GameObject[] bosses;
    [SerializeField] GameObject enemies;
    [SerializeField] GameObject crown;

    [SerializeField] int wave = 1;
    int bossWave = 3;
    [SerializeField] int minEnemiesPerWave = 5;
    [SerializeField] int maxEnemiesPerWave = 30;
    [SerializeField] int enemiesPerWave = 0;
    [SerializeField] float spawnRate = 1f;
    [SerializeField] float maxSpawnRate = 0.5f;

    [SerializeField] int borderX = 24;
    [SerializeField] int borderY = 10;

    int bossNum = 0;
    int enemiesLeft = 0;
    bool spawning = true;
    bool allBossesDefeated = false;
    GameObject player;
    Random rand;

    public float difficultyMod = 1;
    public bool isSurvivalist = false;
    public float healthModifier = 1;
    public UnityEvent<int> waveChanged, counter, enemyCountChanged, onBossBattleStart;
    public UnityEvent onBossCDStart, onBossCDEnd;
    public UnityEvent onBossBattleEnd;
    public UnityEvent<int, string, GameObject> onBossSpawn;
    void Start()
    {
        rand = new Random();
        player = GameObject.FindGameObjectWithTag("Player");
        
        if (PlayerPrefs.GetString("difficulty") == "Easy")
        {
            difficultyMod = 0.5f;
        }
        else if (PlayerPrefs.GetString("difficulty") == "Normal")
        {
            difficultyMod = 0.75f;
        }
        else
        {
            difficultyMod = 1.25f;
            if (PlayerPrefs.GetString("difficulty") == "Survivalist")
            {
                isSurvivalist = true;
            }
        }
        enemiesPerWave = minEnemiesPerWave;

        waveChanged?.Invoke(wave);
        StartCoroutine(spawnWave());
    }

    // Update is called once per frame
    void Update()
    {
        if (enemiesLeft != 0)
        {
            return;
        }

        if (enemies.transform.childCount == 0 && !spawning)
        {
            spawning = true;
            wave++;
            if (enemiesPerWave < maxEnemiesPerWave && wave % 2 == 0)
            {
                enemiesPerWave++;
            }
            waveChanged?.Invoke(wave);

            if (wave % bossWave == 1 && wave > bossWave)
            {
                onBossBattleEnd?.Invoke();
                healthModifier += 0.5f;
                spawnRate -= 0.1f;
            }

            if (wave % bossWave != 0) {
                StartCoroutine(spawnWave());
            } 
            else
            {
                StartCoroutine(spawnBoss());
            }
            
        }
    }

    IEnumerator spawnWave()
    {
        spawning = true;
        int i = 0;
        int totalEnemies = enemiesPerWave;
        setEnemiesLeft(0);

        yield return new WaitForSeconds(1f);
        // 3
        counter?.Invoke(3);
        yield return new WaitForSeconds(1f);
        // 2
        counter?.Invoke(2);
        yield return new WaitForSeconds(1f);
        // 1
        counter?.Invoke(1);
        yield return new WaitForSeconds(1f);
        // GO
        counter?.Invoke(0);
        yield return new WaitForSeconds(1f);
        // Clear
        counter?.Invoke(-1);
        setEnemiesLeft(enemiesPerWave);

        while (i < totalEnemies)
        {
            int roll = rand.Next(0, normalEnemies.Length);
            GameObject enemy = normalEnemies[roll];
            Vector3 randomPos = getRandomPos(false, enemy);

            spawnEnemy(enemy, randomPos, false);
            if (spawning)
            {
                spawning = false;
            }
            i++;
            yield return new WaitForSeconds(spawnRate);
        }
    }

    void spawnEnemy(GameObject enemy, Vector3 randomPos, bool isBoss)
    {
        GameObject e = Instantiate(enemy, randomPos, Quaternion.identity);
        e.transform.SetParent(enemies.transform);

        if (isBoss)
        {
            setupBossHP(e, enemy.name);
        }
    }

    Vector3 getRandomPos(bool isBoss, GameObject enemy)
    {
        float x = Rnd.Range(-borderX + enemy.transform.localScale.x, borderX + 1 - enemy.transform.localScale.x);
        float y = Rnd.Range(enemy.transform.localScale.y, borderY + 1);
        float z = -2; // Fixed
        Vector3 randomPos = new Vector3(x, y, z);

        float dist = 4f;

        if (isBoss)
        {
            dist = enemy.transform.localScale.x + 4f;
        }

        while (Vector3.Distance(randomPos, player.transform.position) <= dist || Mathf.Abs(randomPos.x - player.transform.position.x) <= dist)
        {
            x = Rnd.Range(-borderX + enemy.transform.localScale.x, borderX + 1 - enemy.transform.localScale.x);
            y = Rnd.Range(enemy.transform.localScale.y, borderY + 1);
            randomPos = new Vector3(x, y, z);
        }

        return randomPos;
    }

    IEnumerator spawnBoss()
    {
        spawning = true;
        setEnemiesLeft(0);

        if (bossNum == bosses.Length)
        {
            allBossesDefeated = true;
        }

        if (allBossesDefeated)
        {
            bossNum = rand.Next(0, bosses.Length);
        }

        GameObject boss = bosses[bossNum];
        onBossBattleStart?.Invoke(bossNum);
        onBossCDStart?.Invoke();

        yield return new WaitForSeconds(1f);
        counter?.Invoke(10);
        yield return new WaitForSeconds(1f);
        counter?.Invoke(9);
        yield return new WaitForSeconds(1f);
        counter?.Invoke(8);
        yield return new WaitForSeconds(1f);
        counter?.Invoke(7);
        yield return new WaitForSeconds(1f);
        counter?.Invoke(6);
        yield return new WaitForSeconds(1f);
        counter?.Invoke(5);
        yield return new WaitForSeconds(1f);
        counter?.Invoke(4);
        yield return new WaitForSeconds(1f);
        // 3
        counter?.Invoke(3);
        yield return new WaitForSeconds(1f);
        // 2
        counter?.Invoke(2);
        yield return new WaitForSeconds(1f);
        // 1
        counter?.Invoke(1);
        yield return new WaitForSeconds(1f);
        // GO
        counter?.Invoke(0);
        yield return new WaitForSeconds(1f);
        // Clear
        counter?.Invoke(-1);
        onBossCDEnd?.Invoke();

        setEnemiesLeft(1);
        Vector3 pos = getRandomPos(true, boss);
        spawnEnemy(boss, pos, true);

        bossNum++;
        spawning = false;
    }

    void setupBossHP(GameObject boss, string name)
    {
        int maxHP = boss.GetComponent<Enemy>().getCurrentHP();
        onBossSpawn?.Invoke(maxHP, name, boss);
    }

    void setEnemiesLeft(int count)
    {
        count = Mathf.Clamp(count, 0, enemiesPerWave);
        enemiesLeft = count;
        enemyCountChanged?.Invoke(enemiesLeft);
    }

    public void decrementEnemies()
    {
        setEnemiesLeft(enemiesLeft - 1);
    }

    public bool isLastBoss()
    {
        if (bossNum == bosses.Length)
        {
            return true;
        }

        return false;
    }

    // On the event that the game ends, run this
    public void setNewHighScore()
    {
        if (PlayerPrefs.GetInt("highestWave" + PlayerPrefs.GetString("difficulty")) < wave)
        {
            PlayerPrefs.SetInt("highestWave" + PlayerPrefs.GetString("difficulty"), wave);
        }   
    }

    public void showCrown()
    {
        crown.SetActive(true);
    }
}
