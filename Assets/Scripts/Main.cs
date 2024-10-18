using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static private Main S;  // A private singleton for Main

    [Header("Inscribed")]
    public bool spawnEnemies = true;
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemyInsetDefault = 1.5f;
    public float gameRestartDelay = 2;
    public GameObject prefabPowerUp;

    private BoundsCheck bndCheck;

    // Add a counter for destroyed enemies
    static private int enemiesDestroyed = 0;

    private void Awake()
    {
        S = this;
        bndCheck = GetComponent<BoundsCheck>();
        Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
    }

    public void SpawnEnemy()
    {
        // If spawnEnemies is false, skip to the next invoke of SpawnEnemy()
        if (!spawnEnemies)
        {
            Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
            return;
        }

        // Pick a random enemy prefab to instantiate
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        float enemyInset = enemyInsetDefault;

        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyInset = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyInset;
        float xMax = bndCheck.camWidth - enemyInset;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyInset;
        go.transform.position = pos;

        // Schedule the next enemy spawn
        Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
    }

    // Call this method when an enemy is destroyed
    static public void EnemyDestroyed()
    {
        enemiesDestroyed++;

        if (enemiesDestroyed >= 10)
        {
            Hero.S.RechargeShield(); // Call the method to recharge the shield in Hero class
            enemiesDestroyed = 0; // Reset the counter after shield recharge
        }
    }

    void DelayedRestart()
    {
        Invoke(nameof(Restart), gameRestartDelay);
    }

    void Restart()
    {
        SceneManager.LoadScene("__Scene_1");
    }

    static public void HERO_DIED()
    {
        S.DelayedRestart();
    }
}