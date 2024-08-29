using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
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
    public bool is_attacking = false; // 플레이어가 공격 중인지 여부
    
    Transform tr;
    // Collider2D col;
    Rigidbody2D rb; // 플레이어의 Rigidbody2D 컴포넌트
    SpriteRenderer spriteRenderer;
    public Camera cam;

    public PostProcessVolume volume;


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
        if(Mathf.Abs(rb.velocity.x)>0.1f)
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
        if(is_attacking == true)   return;
        Move();
    }
    void Move()
    {
        if(transform.position.y<-50){
            transform.position = new Vector2(0,0);
        }
        if(Mathf.Abs(rb.velocity.y)<0.0001f)    isGround = true;

        
        Vector2 clampedVelocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y);
        rb.velocity = clampedVelocity;
        int dir = rb.velocity.x > 0?1:rb.velocity.x<0?-1:0;
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

    void Attack()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(is_attacking == false)
            {
                is_attacking = true;
                Time.timeScale = 0.6f;
                StopCoroutine(Post_Process_change(volume, 0.1f));
                volume.weight = 0;
                StartCoroutine(Post_Process_change(volume, 0.1f));
                Run.SetBool("Attack", true);
                StartCoroutine(AttackCoroutine());
            }
        }
    }
    IEnumerator Post_Process_change(PostProcessVolume volume1, float time)
    {
        float t = 0;
        while(volume1.weight < 0.9f)
        {
            t += Time.deltaTime;
            volume1.weight = Mathf.Lerp(volume1.weight, 1, Time.deltaTime*6);
            yield return new WaitForFixedUpdate();
        }
        while(volume1.weight > 0f)
        {
            t += Time.deltaTime;
            volume1.weight = Mathf.Lerp(volume1.weight, -0.1f, Time.deltaTime*6);
            yield return new WaitForFixedUpdate();
        }
    }
    IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        // 플레이어에게 가해지는 마찰력을 계산합니다.
        Vector2 frictionForce = new Vector2(-rb.velocity.x * friction*2, 0);
        rb.AddForce(frictionForce, ForceMode2D.Force);
        // rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(transform.localScale.x * 3, 0), ForceMode2D.Impulse);
        StartCoroutine(Camera_Shake(4, 0.2f));
        yield return new WaitForSeconds(0.25f);
        is_attacking = false;
        Run.SetBool("Attack", false);
        Time.timeScale = 1f;
    }

    IEnumerator Camera_Shake(int cnt, float time, float power = 0.3f)
    {
        Vector3 initialPosition = cam.transform.position;
        for(int i=0; i<cnt; i++)
        {
            cam.transform.position =Random.insideUnitSphere*power + initialPosition;
            yield return new WaitForSeconds(time/cnt);
        }
    }


    private void Jump()
    {
        rb.AddForce(new Vector2(0, jumpingPower *0.8f), ForceMode2D.Impulse);
    }
}