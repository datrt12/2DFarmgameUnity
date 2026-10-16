using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Wander Area")]
    [SerializeField] private BoxCollider2D wanderArea;

    [Header("Wander Settings")]
    [SerializeField] private float minWaitTime = 1f;
    [SerializeField] private float maxWaitTime = 3f;
    [SerializeField] private float arriveDistance = 0.1f;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;

    private Vector2 movement;
    private Vector2 lastDirection = Vector2.down;
    private Vector2 targetPosition;

    private bool isWaiting;
    private bool isTalking;

    private float waitTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        ChooseRandomTarget();
    }

    private void Update()
    {
        // NPC đang hội thoại → đứng im
        if (isTalking)
        {
            movement = Vector2.zero;
            UpdateAnimator();
            return;
        }

        // NPC đang chờ
        if (isWaiting)
        {
            movement = Vector2.zero;

            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                isWaiting = false;
                ChooseRandomTarget();
            }

            UpdateAnimator();
            return;
        }

        // Tính hướng tới vị trí mục tiêu
        Vector2 direction =
            targetPosition - (Vector2)transform.position;

        // Đã tới mục tiêu
        if (direction.magnitude <= arriveDistance)
        {
            movement = Vector2.zero;

            isWaiting = true;

            waitTimer = Random.Range(
                minWaitTime,
                maxWaitTime
            );
        }
        else
        {
            // Di chuyển tới mục tiêu
            movement = direction.normalized;

            UpdateLastDirection();
        }

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        // Đang nói chuyện → bắt buộc đứng im
        if (isTalking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = movement * moveSpeed;
    }

    private void ChooseRandomTarget()
    {
        if (wanderArea == null)
        {
            Debug.LogWarning(
                "NPC chưa được gán Wander Area!",
                this
            );

            return;
        }

        Bounds bounds = wanderArea.bounds;

        float randomX = Random.Range(
            bounds.min.x,
            bounds.max.x
        );

        float randomY = Random.Range(
            bounds.min.y,
            bounds.max.y
        );

        targetPosition = new Vector2(
            randomX,
            randomY
        );
    }

    private void UpdateLastDirection()
    {
        if (Mathf.Abs(movement.x) >
            Mathf.Abs(movement.y))
        {
            lastDirection =
                new Vector2(
                    Mathf.Sign(movement.x),
                    0
                );
        }
        else
        {
            lastDirection =
                new Vector2(
                    0,
                    Mathf.Sign(movement.y)
                );
        }
    }

    private void UpdateAnimator()
    {
        bool isWalking =
            movement.magnitude > 0.01f;

        if (animator == null)
            return;

        animator.SetBool(
            "isWalking",
            isWalking
        );

        animator.SetFloat(
            "InputX",
            movement.x
        );

        animator.SetFloat(
            "InputY",
            movement.y
        );

        animator.SetFloat(
            "LastInputX",
            lastDirection.x
        );

        animator.SetFloat(
            "LastInputY",
            lastDirection.y
        );
    }

    // =====================================================
    // DIALOGUE
    // =====================================================

    public void SetTalking(bool talking)
    {
        isTalking = talking;

        if (isTalking)
        {
            movement = Vector2.zero;

            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            UpdateAnimator();
        }
    }
}