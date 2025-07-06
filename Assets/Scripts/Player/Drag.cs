using UnityEngine;
using UnityEngine.InputSystem;

public class Drag : MonoBehaviour
{
    [SerializeField] private float timeSlow = 0.2f;
    [SerializeField] private float timeLerpSpeed = 5f;
    [SerializeField] private float maxTeleportDistance = 10f;
    [SerializeField] private float forceMultiplier = 5f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private GameObject slashPrefab;

    private Camera cam;
    private Rigidbody2D rb;
    private LineRenderer line;

    private Vector3 inputStart;
    private Vector3 inputEnd;
    private Vector3 teleportDestination;

    private float targetTimeScale = 1f;
    private bool slowingTime = false;
    private bool returningTime = false;
    private bool isDragging = false;

    private Vector2 lastInputScreenPosition;

    private void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        line = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (isDragging)
        {
            OnDragging();
        }

        UpdateTimeScale();
    }

    public void OnPress(InputValue value)
    {
        if (value.isPressed)
        {
            isDragging = true;
            inputStart = GetInputWorldPosition();
            BeginTimeSlow();
        }
        else
        {
            isDragging = false;
            TeleportAndPush();
            ClearLine();
            ReturnToNormalTime();
        }
    }

    public void OnDrag(InputValue value)
    {
        lastInputScreenPosition = value.Get<Vector2>();
    }

    private void OnDragging()
    {
        inputEnd = GetInputWorldPosition();

        Vector3 dragVector = inputStart - inputEnd;
        Vector3 direction = dragVector.normalized;
        float distance = Mathf.Min(dragVector.magnitude, maxTeleportDistance);

        teleportDestination = transform.position + direction * distance;

        UpdateLine();
    }

    private Vector3 GetInputWorldPosition()
    {
        Vector3 worldPos = cam.ScreenToWorldPoint(lastInputScreenPosition);
        worldPos.z = 0f;
        return worldPos;
    }

    private void TeleportAndPush()
    {
        Vector3 direction = (teleportDestination - transform.position).normalized;

        transform.position = teleportDestination;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * forceMultiplier, ForceMode2D.Impulse);
    }

    private void UpdateLine()
    {
        line.positionCount = 2;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, teleportDestination);
    }

    private void ClearLine()
    {
        SpawnSlash();
        line.positionCount = 0;
    }

    private void SpawnSlash()
    {
        GameObject newSlash = Instantiate(slashPrefab, Vector3.zero, Quaternion.identity);
        Slash slash = newSlash.GetComponent<Slash>();

        if (slash != null && line != null)
        {
            slash.Setup(line, damage);
        }
    }

    private void BeginTimeSlow()
    {
        targetTimeScale = timeSlow;
        slowingTime = true;
        returningTime = false;
    }

    private void ReturnToNormalTime()
    {
        targetTimeScale = 1f;
        slowingTime = false;
        returningTime = true;
    }

    private void UpdateTimeScale()
    {
        if (!slowingTime && !returningTime) return;

        Time.timeScale = Mathf.MoveTowards(Time.timeScale, targetTimeScale, Time.unscaledDeltaTime * timeLerpSpeed);

        if (Mathf.Approximately(Time.timeScale, targetTimeScale))
        {
            Time.timeScale = targetTimeScale;
            slowingTime = false;
            returningTime = false;
        }

        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}
