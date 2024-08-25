using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public float speed = 10f;
    public bool is_Clicked = false;
    public float friction = 0.9f;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float in_x = Input.GetAxis("Horizontal");

        if(is_Clicked)
        {
            rb.velocity = new Vector2(speed * in_x, rb.velocity.y);
        }
        else
        {
            rb.AddForce(new Vector2(in_x*speed, 0));
            rb.velocity = new Vector2(rb.velocity.x * friction, rb.velocity.y);
        }
    }
    private void FixedUpdate() {
        if(Input.GetMouseButtonDown(0))
        {
            is_Clicked = true;
        }
        if(Input.GetMouseButtonDown(1))
        {
            is_Clicked = false;
        }
    }
}
