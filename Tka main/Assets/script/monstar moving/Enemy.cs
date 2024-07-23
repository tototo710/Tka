using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 100;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(int damage, Vector3 ps)
    {
        health -= damage;
        rb.AddForce((transform.position - ps) * 10, ForceMode2D.Impulse);
    }
}
