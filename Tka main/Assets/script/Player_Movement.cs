using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class Player_Movement : MonoBehaviour
{
    Animator Run;
    float jumpingPower = 40; // 플레이어 점프 힘
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
        
    }
    
    public GameObject impactEffect;
    public bool is_old_move = false;
    void Update()
    {
         
        if(Run.GetBool("stop_player"))
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            Run.SetBool("stop_player", false); 
        }

        // if(move_ff)
        // {
        //     rb.AddForce(new Vector2(32*transform.localScale.x, 0), ForceMode2D.Impulse);
        //     move_ff = false;
        // }

        if(transform.position.y<-50){
            transform.position = new Vector2(0,1);
        }

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
        
        if(Run.GetBool("shakecam") && Run.GetBool("on_strong_attack") && !Run.GetBool("stop_player"))
        {
            Run.SetBool("onLastattack", false);    
            Run.SetBool("shakecam", false);
            Run.SetBool("on_strong_attack", true);
            StartCoroutine(Camera_Shake(4, 1, 1f));
            Run.SetBool("stop_player", false);
            StartCoroutine(late_move(10, 0.01f));  
            // move_ff = true;
        }

        if(Run.GetBool("shakecam") && !Run.GetBool("onLastattack") && !Run.GetBool("on_strong_attack"))
        {
            StartCoroutine(late_attack(4, 1, 1f, 0.024f));
            rb.AddForce(new Vector2(10*transform.localScale.x, 0), ForceMode2D.Impulse);
            Run.SetBool("shakecam", false);
            Run.SetBool("onLastattack", false);
        }
        else if(Run.GetBool("shakecam") && Run.GetBool("onLastattack") && !Run.GetBool("on_strong_attack"))
        {
            Run.SetBool("onLastattack", false);
            StartCoroutine(late_attack(6, 1, 2f, .1f));
            rb.AddForce(new Vector2(20*transform.localScale.x, 0), ForceMode2D.Impulse);
            Run.SetBool("shakecam", false);
            Run.SetBool("onLastattack", false);
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
        if(Run.GetBool("onattacking"))  StartCoroutine(감속());
        if(Run.GetBool("onattacking") || Run.GetBool("on_land_attack") || Run.GetBool("on_strong_attack"))   return;

        Move();
        
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (isGround)
            {
                isGround = false;
                Jump();
                Run.SetTrigger("jump");
            }
        }
    }
    public bool move_ff = false;
    
    IEnumerator 감속()
    {
        while(Mathf.Abs(rb.velocity.x)>0.1f)
        {
            rb.velocity = new Vector2(rb.velocity.x*0.999f, rb.velocity.y);
            yield return new WaitForFixedUpdate();
        }
    }
    void late_back()
    {
        Run.SetBool("onattacking", false);
        Run.SetBool("on_land_attack", false);
    }
    public int dir = 1;
    void Move()
    {
        dir = (int)Input.GetAxisRaw("Horizontal") * (int)Mathf.Abs(Input.GetAxisRaw("Horizontal"));
        if(dir!=0)
        {
            transform.localScale = new Vector3(dir, 1, 1);
        }
        float horizontalInput = Input.GetAxis("Horizontal") * (Input.GetAxis("Horizontal") != 0 && Input.GetAxisRaw("Horizontal") == 0 && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) ? 0 : 1);
        // if(Input.GetAxisRaw("Horizontal") == 0)
        // {
        //     rb.velocity = new Vector2(rb.velocity.x * 0.9f, rb.velocity.y);
        // }
        // Vector2 moveDirection = new Vector2(horizontalInput, 0); // 이동 방향 벡터
        rb.velocity = new Vector2(horizontalInput*16.5f, rb.velocity.y);
        
        if(Input.GetAxis("Horizontal") * Mathf.Abs(Input.GetAxisRaw("Horizontal")) != 0)
        {
            Run.SetBool("Run", true);
        }
        else 
        {
            Run.SetBool("Run", false);
        }

    }
    void Attack()
    {
        if(Input.GetMouseButtonDown(1) && Run.GetBool("onattacking")==false && Run.GetBool("on_ground")==true && !Input.GetKey(KeyCode.S) && !Run.GetBool("on_strong_attack"))
        {
            rb.velocity = new Vector2(0,rb.velocity.y);
            Run.SetTrigger("Strong_attack");
        }

        if(Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.S) && Run.GetBool("onattacking")==false && Run.GetBool("on_ground")==true)
        {
            Run.SetTrigger("onkick");
            rb.velocity = new Vector2(0,rb.velocity.y);
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
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    IEnumerator late_move(float power, float delay)
    {
        yield return new WaitForSeconds(delay);
        Run.SetBool("stop_player", false);
        yield return new WaitForSeconds(0.001f);
        rb.AddForce(new Vector2(power*transform.localScale.x, 0), ForceMode2D.Impulse);
    }
    private void Jump()
    {
        Run.SetBool("on_ground", false);
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0, jumpingPower *0.8f), ForceMode2D.Impulse);
    }
}