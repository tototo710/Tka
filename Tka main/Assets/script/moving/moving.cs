using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moving : MonoBehaviour
{
    Rigidbody2D rb;
    public float lastRotation = 1; // 플레이어가 바라보고 있는 방향.
    
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("a")&&lastRotation==1){
                    transform.localScale = new Vector3(-1,1,0);
        }
        if(Input.GetKeyDown("d")&&lastRotation==-1){
                transform.localScale = new Vector3(1,1,0);
            }
        float horizontalInput = Input.GetAxis("Horizontal");
         if (horizontalInput != 0)
            {
                if (horizontalInput > 0) { lastRotation = 1; }
                else { lastRotation = -1; }
            }
    }
}
