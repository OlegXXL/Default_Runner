using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float forwardSpeed = 8f;
    public float laneDistance = 3f;   // відстань між доріжками
    public float laneChangeSpeed = 15f;

    [Header("Jump & Roll")]
    public float jumpHeight = 3f;     // максимальна висота стрибка
    public float jumpDuration = 0.5f; // час підйому і спуску
    public float rollDuration = 0.8f;

    [Header("Animation")]
    public Animator animator;

    private CharacterController controller;
    private int currentLane = 1;       // 0=ліва, 1=середня, 2=права
    private bool isJumping = false;
    private bool isRolling = false;
    private float baseColliderHeight;
    private Vector3 baseColliderCenter;

    // свайпи
    private Vector2 startTouch;
    private Vector2 swipeDelta;
    private bool swipeLeft, swipeRight, swipeUp, swipeDown;

    private float jumpTimeCounter = 0f;
    private float currentY = 0f;    // поточна висота персонажа
    private bool gameIsFinish = false;
    private bool gameIsStart = false;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        baseColliderHeight = controller.height;
        baseColliderCenter = controller.center;
        transform.position = new Vector3(0.07f, 0.531f, 0); //стабілізація персонажа коли він без анімації
    }

    private void Update()
    {
        if (!gameIsFinish && gameIsStart)
        {
            animator?.SetTrigger("Run");
            HandleSwipeInput();
            MovePlayer();
        }
    }
    private IEnumerator resetRotation()
    {
        yield return new WaitForSeconds(1f);
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void MovePlayer()
    {
        float targetX = (currentLane - 1) * laneDistance;
        float newX = Mathf.MoveTowards(transform.position.x, targetX, laneChangeSpeed * Time.deltaTime);

        // стрибок
        if (isJumping)
        {
            jumpTimeCounter += Time.deltaTime;
            float t = jumpTimeCounter / jumpDuration;
            if (t < 0.5f)
            {
                // підйом
                currentY = Mathf.Lerp(0, jumpHeight, t * 2);
            }
            else if (t < 1f)
            {
                // спуск
                currentY = Mathf.Lerp(jumpHeight, 0, (t - 0.5f) * 2);
            }
            else
            {
                // завершення стрибка
                isJumping = false;
                currentY = 0;
            }
        }
        // ролл
        if (isRolling)
        {
            currentY = 0;
        }

        // рух персонажа прямо
        float newZ = transform.position.z + forwardSpeed * Time.deltaTime;

        Vector3 newPosition = new Vector3(newX, currentY, newZ);
        controller.Move(newPosition - transform.position);
    }

    private void Jump()
    {
        if (isJumping || isRolling) return;
        isJumping = true;
        jumpTimeCounter = 0f;
        animator?.SetTrigger("Jump");
        StartCoroutine(resetRotation());
    }

    private IEnumerator Roll()
    {
        if (isRolling) yield break;
        isRolling = true;
        animator?.SetTrigger("Roll");

        // зменшуємо колайдер коли рол, щоб проходити перешкоди
        controller.height = baseColliderHeight / 2f;
        controller.center = new Vector3(baseColliderCenter.x, baseColliderCenter.y / 2f, baseColliderCenter.z);

        currentY = 0; // різко опускаємось

        yield return new WaitForSeconds(rollDuration);

        // повертаємо колайдер
        controller.height = baseColliderHeight;
        controller.center = baseColliderCenter;

        isRolling = false;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        StopCoroutine(resetRotation());
        StartCoroutine(resetRotation());
    }
    // читаємо свайпи
    private void HandleSwipeInput()
    {
        swipeLeft = swipeRight = swipeUp = swipeDown = false;

        if (Input.GetMouseButtonDown(0))
        {
            startTouch = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            swipeDelta = (Vector2)Input.mousePosition - startTouch;

            if (swipeDelta.magnitude > 80f)
            {
                float x = swipeDelta.x;
                float y = swipeDelta.y;

                if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    if (x < 0) swipeLeft = true;
                    else swipeRight = true;
                }
                else
                {
                    if (y > 0) swipeUp = true;
                    else swipeDown = true;
                }
            }
            startTouch = swipeDelta = Vector2.zero;
        }

        // керування доріжками
        if (swipeLeft && currentLane > 0)
            currentLane--;
        else if (swipeRight && currentLane < 2)
            currentLane++;

        if (swipeUp)
            Jump();
        else if (swipeDown)
            StartCoroutine(Roll());
    }
    private void StopMoving()
    {
        gameIsFinish = true;
        animator.speed = 0f;
    }
    private void StartGame()
    {
        gameIsStart = true;
    }
    private void OnEnable()
    {
        EventManager.OnGameOver += StopMoving;
        EventManager.OnGameStart += StartGame;
        EventManager.OnGameWin += StopMoving;
    }
}
