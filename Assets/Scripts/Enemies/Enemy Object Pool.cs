using System.Collections.Generic;
using UnityEngine;

public class EnemyObjectPool : MonoBehaviour
{
    public static EnemyObjectPool instance;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int poolSize = 20;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        for (int i = 0; i < poolSize; i++)
        {
            Create();
        }
    }

    void Create()
    {
        GameObject enemy = Instantiate(enemyPrefab);
        enemy.SetActive(false);
        pool.Enqueue(enemy);
    }

    public GameObject GetFromPool()
    {
        if (pool.Count <= 0) Create();

        GameObject enemy = pool.Dequeue();
        enemy.SetActive(true);
        return enemy;
    }

    public void ReturnToPool(GameObject enemy)
    {
        enemy.SetActive(false);
        pool.Enqueue(enemy);
    }
}
