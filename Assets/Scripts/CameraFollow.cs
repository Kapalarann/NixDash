using UnityEngine;

[ExecuteAlways]
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Edge Limits")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 5f;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 targetPos = new Vector3(
            Mathf.Clamp(target.position.x, minX, maxX),
            Mathf.Clamp(target.position.y, minY, maxY),
            transform.position.z
        );

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.unscaledDeltaTime * smoothSpeed);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 bottomLeft = new Vector3(minX, minY, 0f);
        Vector3 topRight = new Vector3(maxX, maxY, 0f);
        Vector3 center = (bottomLeft + topRight) * 0.5f;
        Vector3 size = topRight - bottomLeft;

        Gizmos.DrawWireCube(center, size);
    }
}
