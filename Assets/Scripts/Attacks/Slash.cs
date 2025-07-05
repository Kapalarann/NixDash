using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class Slash : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;

    private float damage = 1;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float destroyDelay = 0.1f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();
    }

    public void Setup(LineRenderer sourceLine, float damageAmount = 1f)
    {
        if (sourceLine == null) return;

        damage = damageAmount;

        int count = sourceLine.positionCount;
        lineRenderer.positionCount = count;

        Vector3[] worldPositions = new Vector3[count];
        Vector2[] colliderPoints = new Vector2[count];

        sourceLine.GetPositions(worldPositions);

        for (int i = 0; i < count; i++)
        {
            lineRenderer.SetPosition(i, worldPositions[i]);

            colliderPoints[i] = new Vector2(worldPositions[i].x, worldPositions[i].y);
        }

        edgeCollider.points = colliderPoints;

        DoHitCheck();

        StartCoroutine(FadeAndDestroy());
    }

    private void DoHitCheck()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        Collider2D[] results = new Collider2D[10];

        int hitCount = edgeCollider.Overlap(filter, results);

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D col = results[i];
            if (col == null) continue;

            Enemy enemy = col.GetComponent<Enemy>();
            Health health = col.GetComponent<Health>();

            if (enemy != null && health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }

    private System.Collections.IEnumerator FadeAndDestroy()
    {
        float time = 0f;
        Gradient startGradient = lineRenderer.colorGradient;

        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = startGradient.colorKeys;
        GradientAlphaKey[] alphaKeys = startGradient.alphaKeys;

        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            float alpha = Mathf.Lerp(1f, 0f, t);

            for (int i = 0; i < alphaKeys.Length; i++)
                alphaKeys[i].alpha = alpha;

            gradient.SetKeys(colorKeys, alphaKeys);
            lineRenderer.colorGradient = gradient;

            time += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject, destroyDelay);
    }
}
