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
    public bool on_delay = false;
    public GC gc;


    void Awake()
    {
        Application.targetFrameRate = 120;
        gc = transform.GetChild(0).GetComponent<GC>();
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
    public GameObject impactEffect;
    void Update()
    {
        if(Run.GetBool("stop_player"))
        {
            rb.velocity = new Vector2(0,0);
            return;
        }
        if(transform.position.y<-50){
            transform.position = new Vector2(0,01);
        }

        if(Input.GetAxisRaw("Horizontal") != 0)
        {
            Run.SetBool("Run", true);
        }
        else
        {
            Run.SetBool("Run", false);
        }
        Debug.Log(Input.GetAxisRaw("Horizontal"));

        if(Mathf.Abs(rb.velocity.y)<0.01f && gc.isGround)
        {
            isGround = true;
            Run.SetBool("on_ground", true);
        }
        else
        {
            // isGround = false;
            Run.SetBool("on_ground", false);
        }
        Run.SetFloat("y_speed", rb.velocity.y);


        Attack();
        if(Run.GetBool("shakecam") && !Run.GetBool("onLastattack"))
        {
            StartCoroutine(late_attack(4, 1, 1f, 0.024f));
            rb.AddForce(new Vector2(4*transform.localScale.x, 0), ForceMode2D.Impulse);
            Run.SetBool("shakecam", false);
        }
        else if(Run.GetBool("shakecam") && Run.GetBool("onLastattack"))
        {
            StartCoroutine(late_attack(4, 1, 2f, .1f));
            rb.AddForce(new Vector2(8*transform.localScale.x, 0), ForceMode2D.Impulse);
            Run.SetBool("shakecam", false);
        }
    
    

        if(Run.GetBool("land_f_attack"))
        {
            StartCoroutine(Camera_Shake(1, -4, 01f));
            Run.SetBool("land_f_attack", false);
            Run.SetBool("onattacking", true);
            Run.SetBool("on_land_attack", true);
            CancelInvoke("late_back");
            Invoke("late_back", 0.5f);
            Instantiate(impactEffect, transform.position + new Vector3(-2.5f, 0, 0), Quaternion.identity);
            rb.velocity = new Vector2(0,0);
        }
        // if(Run.GetBool("onattacking"))  StartCoroutine(감속());
        if(Run.GetBool("onattacking") || Run.GetBool("on_land_attack"))   return;
        Move();
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGround)
            {
                isGround = false;
                Jump();
                Run.SetTrigger("jump");
            }
        }
    }

    // IEnumerator 감속()
    // {
    //     while(Mathf.Abs(rb.velocity.x)>0.1f)
    //     {
    //         rb.velocity = new Vector2(rb.velocity.x*0.99f, rb.velocity.y);
    //         yield return new WaitForFixedUpdate();
    //     }
    // }
    void late_back()
    {
        Run.SetBool("onattacking", false);
        Run.SetBool("on_land_attack", false);
    }
    public int dir = 1;
    void Move()
    {
        Vector2 clampedVelocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y);
        rb.velocity = clampedVelocity;
        dir = (int)Input.GetAxisRaw("Horizontal");
        if(dir!=0)
        {
            transform.localScale = new Vector3(dir, 1, 1);
        }
        Debug.Log(Input.GetAxis("Horizontal"));
        float horizontalInput = Input.GetAxis("Horizontal") * (Input.GetAxis("Horizontal") != 0 && Input.GetAxisRaw("Horizontal") == 0 ? (Mathf.Abs(Input.GetAxis("Horizontal")) < 1f && Input.GetAxisRaw("Horizontal") == 0 ? 1 : 0)  : 1); // 수평 입력 값
        Vector2 moveDirection = new Vector2(horizontalInput, 0); // 이동 방향 벡터

        // 플레이어에게 가해지는 마찰력을 계산합니다.
        Vector2 frictionForce = new Vector2(-rb.velocity.x * friction, 0);
        rb.AddForce(frictionForce, ForceMode2D.Force);


        // 플레이어에게 이동 힘을 가합니다.
        rb.AddForce(moveDirection * speed);
    }
    void Attack()
    {

        // if(Input.GetMouseButtonDown(0) && Run.GetBool("onLastattack")==false && isGround==true)
        // {
        //     Run.SetTrigger("Attack");
        // }
        if(Input.GetMouseButtonDown(1) && Run.GetBool("onattacking")==false && Run.GetBool("on_ground")==true && !Input.GetKey(KeyCode.S))
        {
            rb.velocity = new Vector2(0,0);
            Run.SetTrigger("Strong_attack");
            StartCoroutine(late_attack(4, 1, 4f, 0.4f));
            StartCoroutine(late_move(18, 0.4f));
        }

        if(Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.S) && Run.GetBool("onattacking")==false && Run.GetBool("on_ground")==true)
        {
            Run.SetTrigger("onkick");
            rb.velocity = new Vector2(0,0);
            StartCoroutine(late_attack(4, 1, 4f, 0.4f/3f));
        }
        else if(Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.S) && Run.GetBool("onattacking")==false && Run.GetBool("on_ground")==false)
        {
            Run.SetTrigger("fallattack");
            Run.SetBool("on_land_attack", true);
            rb.velocity = new Vector2(0,rb.velocity.y);
        }
    }
    IEnumerator late_attack(int cnt, int y_dir=-1, float power = 0.5f, float delay = 0.4f/3f)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(Camera_Shake(cnt, y_dir, power));
    }

    IEnumerator Camera_Shake(int cnt, int y_dir=-1, float power = 0.5f)
    {
        Vector3 initialPosition = cam.transform.position;
        for(int i=0; i<cnt; i++)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, initialPosition + new Vector3(transform.localScale.x*power, y_dir*power,0), Time.deltaTime*15);
            yield return new WaitForFixedUpdate();
        }
    }
    IEnumerator late_move(float power, float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.AddForce(new Vector2(power*transform.localScale.x, 0), ForceMode2D.Impulse);
    }
    private void Jump()
    {
        Run.SetBool("on_ground", false);
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0, jumpingPower *0.8f), ForceMode2D.Impulse);
    }
}