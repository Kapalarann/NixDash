using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private float leftX = -10f;
    [SerializeField] private float rightX = 10f;
    [SerializeField] private float minY = -2f;
    [SerializeField] private float maxY = 2f;
    [SerializeField] private float spawnInterval = 2f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        bool spawnLeft = Random.value < 0.5f;
        float xPos = spawnLeft ? leftX : rightX;
        float yPos = Random.Range(minY, maxY);
        Vector3 spawnPos = new Vector3(xPos, yPos, 0f);

        Vector2 dir = spawnLeft ? Vector2.right : Vector2.left;

        GameObject enemyGO = EnemyObjectPool.instance.GetFromPool();
        enemyGO.transform.position = spawnPos;

        Enemy enemy = enemyGO.GetComponent<Enemy>();
        enemy.Init(dir);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 leftStart = new Vector3(leftX, minY, 0f);
        Vector3 leftEnd = new Vector3(leftX, maxY, 0f);
        Gizmos.DrawLine(leftStart, leftEnd);

        Vector3 rightStart = new Vector3(rightX, minY, 0f);
        Vector3 rightEnd = new Vector3(rightX, maxY, 0f);
        Gizmos.DrawLine(rightStart, rightEnd);

        Gizmos.DrawWireCube(new Vector3(leftX, (minY + maxY) / 2f, 0f), new Vector3(0.2f, maxY - minY, 0.2f));
        Gizmos.DrawWireCube(new Vector3(rightX, (minY + maxY) / 2f, 0f), new Vector3(0.2f, maxY - minY, 0.2f));
    }
}
