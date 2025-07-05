using UnityEngine;

public class Drag : MonoBehaviour
{
    [SerializeField] private float timeSlow = 0.2f;
    [SerializeField] private float timeLerpSpeed = 5f;

    [SerializeField] private float maxTeleportDistance = 10f;
    [SerializeField] private float forceMultiplier = 5f;

    [SerializeField] float damage = 1f;
    [SerializeField] private GameObject slashPrefab;

    private Camera cam;
    private Rigidbody2D rb;
    private LineRenderer line;

    private Vector3 mouseStart;
    private Vector3 mouseEnd;
    private Vector3 teleportDestination;

    private float targetTimeScale = 1f;
    private bool slowingTime = false;
    private bool returningTime = false;

    void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        HandleInput();
        UpdateTimeScale();
    }

    private void HandleInput()
    {
        // MOUSE INPUT (Right-click)
        if (Input.GetMouseButtonDown(1))
            OnDragStart();

        if (Input.GetMouseButton(1))
            OnDragging();

        if (Input.GetMouseButtonUp(1))
            OnDragRelease();

        // TOUCH INPUT (Single finger)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnDragStart();
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    OnDragging();
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    OnDragRelease();
                    break;
            }
        }
    }


    private void OnDragStart()
    {
        mouseStart = GetMouseWorldPosition();
        BeginTimeSlow();
    }

    private void OnDragging()
    {
        mouseEnd = GetMouseWorldPosition();

        // Compute slingshot direction from mouse drag (inverted)
        Vector3 dragVector = mouseStart - mouseEnd;
        Vector3 direction = dragVector.normalized;

        float distance = Mathf.Min(dragVector.magnitude, maxTeleportDistance);

        teleportDestination = transform.position + direction * distance;

        UpdateLine();
    }

    private void OnDragRelease()
    {
        TeleportAndPush();
        ClearLine();
        ReturnToNormalTime();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0f;
        return pos;
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
