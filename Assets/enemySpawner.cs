using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawner : MonoBehaviour
{
    public int maxEnemyCount = 20;
    public GameObject secondPoint;
    public GameObject Enemy;


    public int enemyCount = 0;
    private float timik = 0f;

    void Update()
    {
        if (enemyCount < 20)
        {
            float randTime = Random.value * 2 + 1;
            if (Random.value > 0.5f)
            {
                if (timik > randTime)
                {
                    Instantiate(Enemy, transform.position, Quaternion.identity);
                    timik = 0;
                    enemyCount++;
                }
            }
            else
            {
                if (timik > randTime)
                {
                    Instantiate(Enemy, secondPoint.transform.position, Quaternion.identity);
                    timik = 0;
                    enemyCount++;
                }
            }

            timik += Time.deltaTime;

        }
    }

    
}
