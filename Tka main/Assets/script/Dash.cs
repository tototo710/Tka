using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
public class Dash : MonoBehaviour
{
    public float lenth = 2f;
    public GameObject closestEnemy = null;
    float x;
    public Rigidbody2D rb;
    float y;
    public bool cancontrol=true;
    float dash_timer;

    public bool isdashing = false; 
    GameObject player;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        cancontrol =true;
        isdashing = false;
    }
    void Update()
    {   
        if (dash_timer > 0) { dash_timer -= Time.deltaTime; } else if (dash_timer < 0) { dash_timer = 0; }
        if(Input.GetMouseButtonDown(1))
        {
           x = transform.position.x;
           if(player.GetComponent<Player_controller>().lastRotation==1){
            if (cancontrol&&dash_timer==0){
                dash_timer =1.2f;
                rb.constraints = RigidbodyConstraints2D.FreezePositionY;
                rb.velocity = new Vector2(0,0);
                
                StartCoroutine(Start_Dash());
                StartCoroutine(End_Dash());
            }
           }
           if(player.GetComponent<Player_controller>().lastRotation==-1){
            if(cancontrol&&dash_timer ==0){
                dash_timer=1.2f;
                rb.constraints = RigidbodyConstraints2D.FreezePositionY;
                rb.velocity = new Vector2(0,0);

                StartCoroutine(Start_Dash());
                StartCoroutine(End_Dash());
            }
           }
        }
    }

    IEnumerator Start_Dash(){
        player.GetComponent<Player_controller>().canmove = false;
        player.GetComponent<attack>().canattack = false;
        for (int i = 1; i <= 2; i++)
        {
            rb.AddForce(new Vector2(1300*this.GetComponent<Player_controller>().lastRotation,0));
            cancontrol =false;
            isdashing = true;
            // Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"));
            rb.constraints = RigidbodyConstraints2D.FreezePositionY;
            yield return new WaitForSeconds(0.001f);
        }
        
    }

    IEnumerator End_Dash(){
        isdashing = false;
        yield return new WaitForSeconds(0.15f);
        rb.velocity = new Vector2(0,0);
        yield return new WaitForSeconds(0.2f);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        cancontrol =true;
        // Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"),false);
        player.GetComponent<Player_controller>().canmove = true;
        player.GetComponent<attack>().canattack = true;
    }
}