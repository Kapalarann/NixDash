using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float hp = 1f;

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
