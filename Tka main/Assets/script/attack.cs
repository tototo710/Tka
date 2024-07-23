using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class attack : MonoBehaviour
{
    public int damage = 1;
    public float attackRange = 0.5f;
    bool canattack = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && canattack)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position + Vector3.right * transform.localScale.x, attackRange);
            foreach(Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<Enemy>().TakeDamage(damage, transform.position);
            }
            StartCoroutine(attackCooldown());
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position + Vector3.right * transform.localScale.x, attackRange);
    }

    IEnumerator attackCooldown()
    {
        canattack = false;
        yield return new WaitForSeconds(0.5f);
        canattack = true;
    }

}