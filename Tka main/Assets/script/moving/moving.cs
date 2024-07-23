using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moving : MonoBehaviour
{
    Rigidbody2D rb;
    public float lastRotation = 1; // 플레이어가 바라보고 있는 방향.
    public float speed = 5;
    public float jumpPower = 5;
    
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        horizontalInput = Mathf.Round(horizontalInput);
        if (horizontalInput != 0)
        {
            transform.localScale = new Vector3(horizontalInput, 1, 1);
        }
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
        if (Input.GetKeyDown(KeyCode.Space) && gg())
        {
            jump();
        }
    }


    bool gg()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, 1.2f, 1<<LayerMask.NameToLayer("Ground"));
    }
    
    void jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
    }
}
