using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_controller : MonoBehaviour
{
    Animator Run;
    Animator jump;
    [SerializeField] float speed = 10; // 플레이어 이동 속도
    [SerializeField] float jumpingPower = 10; // 플레이어 점프 힘
    [SerializeField] float friction = 6f; // 플레이어 마찰력
    [SerializeField] float maxSpeed = 10; // 플레이어 최대 속력

    public LayerMask groundLayer; // 바닥 레이어
    private bool canJump = true; // 점프 가능 여부
    public bool isGround = false; // 플레이어가 바닥에 있는지 여부
    public float lastRotation = 1; // 플레이어가 바라보고 있는 방향.
    public int speeder;
    
    Transform tr;
    Collider2D col;
    Rigidbody2D rb; // 플레이어의 Rigidbody2D 컴포넌트
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        Application.targetFrameRate = 120;
    }

    void Start()
    {
        tr = this.GetComponent<Transform>();
        rb = this.GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Run = GetComponent<Animator>();
        jump = GetComponent<Animator>();
    }

    void Update()
    {

        // 플레이어에게 가해지는 마찰력을 계산합니다.
        Vector2 frictionForce = new Vector2(-rb.velocity.x * friction, 0);
        rb.AddForce(frictionForce, ForceMode2D.Force);
        // 플레이어의 속력을 제한하여 최대 속력을 유지합니다.
        Vector2 clampedVelocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y);
        rb.velocity = clampedVelocity;
        float horizontalInput = Input.GetAxisRaw("Horizontal");
            horizontalInput = Mathf.Round(horizontalInput);
            if (horizontalInput != 0)
            {
                transform.localScale = new Vector3(horizontalInput, 1, 1);
            }
            horizontalInput = Input.GetAxis("Horizontal");
            Vector2 moveDirection = new Vector2(horizontalInput, 0); // 이동 방향 벡터

            if (horizontalInput != 0)
            {
                if (horizontalInput > 0) { lastRotation = 1; }
                else { lastRotation = -1; }
            }

        // 플레이어에게 이동 힘을 가합니다.
            rb.AddForce(moveDirection * speed);

            if(Mathf.Abs(rb.velocity.y)<0.0001f)
            {
                canJump = true;
            }
            if(Input.GetKeyDown(KeyCode.Space))
            {
                if (Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer))
                {
                    canJump = false;
                    isGround = false;
                    Jump();
                }
            }
        
    }


    private void Jump()
    {
        rb.AddForce(new Vector2(0, jumpingPower / 28.0f * 23.5f));
        isGround = false;
        canJump = false;
    }
}