using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GC : MonoBehaviour
{
    public bool isGround = true; // 플레이어가 바닥에 있는지 여부

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.layer == 6)
        {
            isGround = true;
        }
    }
    private void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.layer == 6)
        {
            isGround = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.layer == 6)
        {
            isGround = false;
        }
    }
}
