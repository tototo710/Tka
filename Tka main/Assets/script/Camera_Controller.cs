using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    public Transform player;
    public float follow_speed =10f;
    void Update()
    {
        if(GameObject.FindWithTag("Player").GetComponent<encounter_enemy>().on_fight==true) return;
        transform.position = Vector3.Lerp(transform.position, new Vector3(player.position.x, 0, -3), Time.deltaTime* follow_speed);
    }
}
