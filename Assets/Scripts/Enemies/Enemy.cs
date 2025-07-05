using UnityEngine;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public Vector2 dir;

    [SerializeField] private float moveSpeed = 2f;

    public void Init(Vector2 direction)
    {
        dir = direction;
    }

    void Update()
    {
        transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);

        // Example: return to pool when off-screen (or dead, etc.)
        if (Mathf.Abs(transform.position.x) > 20f)
        {
            EnemyObjectPool.instance?.ReturnToPool(gameObject);
        }
    }
}
