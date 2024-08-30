using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UIElements;

public class Player_Movement : MonoBehaviour
{
    Animator Run;
    Animator jump;
    [SerializeField] float speed = 100; // 플레이어 이동 속도
    [SerializeField] float jumpingPower = 40; // 플레이어 점프 힘
    [SerializeField] float friction = 20f; // 플레이어 마찰력
    [SerializeField] float maxSpeed = 100; // 플레이어 최대 속력

    public LayerMask groundLayer; // 바닥 레이어
    // public Transform groundCheck; // 바닥 체크 위치

    public bool isGround = false; // 플레이어가 바닥에 있는지 여부
    // public bool is_attacking = false; // 플레이어가 공격 중인지 여부
    
    Transform tr;
    // Collider2D col;
    Rigidbody2D rb; // 플레이어의 Rigidbody2D 컴포넌트
    SpriteRenderer spriteRenderer;
    public Camera cam;

    public PostProcessVolume volume;
    bool isAttacking = false;
    public bool on_delay = false;


    void Awake()
    {
        Application.targetFrameRate = 120;
    }

    void Start()
    {
        tr = this.GetComponent<Transform>();
        rb = this.GetComponent<Rigidbody2D>();
        // footC = groundCheck.GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Run = GetComponent<Animator>();
        jump = GetComponent<Animator>();
    }

    void Update()
    {
        if(Input.GetAxisRaw("Horizontal") != 0)
        {
            Run.SetBool("Run", true);
        }
        else
        {
            Run.SetBool("Run", false);
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGround)
            {
                isGround = false;
                Jump();
            }
        }


        Attack();
        if(Run.GetBool("Attack"))   return;
        Move();
    }
    public int dir = 1;
    IEnumerator Delay(float time)
    {
        on_delay = true;
        yield return new WaitForSeconds(time);
        on_delay = false;
    }
    void Move()
    {
        if(transform.position.y<-50){
            transform.position = new Vector2(0,0);
        }
        if(Mathf.Abs(rb.velocity.y)<0.0001f)    isGround = true;

        
        Vector2 clampedVelocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y);
        rb.velocity = clampedVelocity;
        dir = rb.velocity.x > 0?1:rb.velocity.x<0?-1:0;
        if(dir!=0)
        {
            transform.localScale = new Vector3(dir, 1, 1);
        }
        float horizontalInput = Input.GetAxis("Horizontal"); // 수평 입력 값
        Vector2 moveDirection = new Vector2(horizontalInput, 0); // 이동 방향 벡터
                
        // 플레이어에게 가해지는 마찰력을 계산합니다.
        Vector2 frictionForce = new Vector2(-rb.velocity.x * friction, 0);
        rb.AddForce(frictionForce, ForceMode2D.Force);


        // 플레이어에게 이동 힘을 가합니다.
        rb.AddForce(moveDirection * speed);

    }
    // bool on_2nd_attack = false;
    void Attack()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Run.SetTrigger("Attack");
            StartCoroutine(Camera_Shake(3, 0.1f, 0.5f));
        }
    }

    IEnumerator Camera_Shake(int cnt, float time, float power = 0.5f)
    {
        Vector3 initialPosition = cam.transform.position;
        for(int i=0; i<cnt; i++)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, initialPosition + new Vector3(1*dir*power, -1*power,0), Time.deltaTime*15);
            yield return new WaitForFixedUpdate();
        }

    }


    private void Jump()
    {
        rb.AddForce(new Vector2(0, jumpingPower *0.8f), ForceMode2D.Impulse);
    }
}