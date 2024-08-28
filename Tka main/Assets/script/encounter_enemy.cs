using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class encounter_enemy : MonoBehaviour
{
    public float detect_range = 1.0f;
    public GameObject fight_screen;
    public bool on_fight = false;   
    public GameObject target_point;
    RectTransform rt;
    public Transform Dummy;
    public GameObject camera_main;
    public CanvasGroup canvasGroup;
    private GameObject dd;
    // Start is called before the first frame update
    void Start()
    {
        rt = fight_screen.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(on_fight==false)
            detect_enemy();
        if(Input.GetKeyDown(KeyCode.Escape)) Time.timeScale = 1f;
    }
    void detect_enemy()
    {
        Collider2D[] detected = Physics2D.OverlapCircleAll(transform.position,detect_range);
        foreach(Collider2D d in detected)
        {
            if(d.tag == "Enemy")
            {
                Debug.Log("Enemy Detected");
                dd = d.gameObject;
                StartCoroutine(show_fight_screen(d.transform.position));
                on_fight = true;
                GetComponent<Player_Movement>().enabled = false;
                d.GetComponent<Rigidbody2D>().velocity = new Vector2(0.5f,0);
                Time.timeScale = 0f;
                break;
            }
        }
    }
    IEnumerator show_fight_screen(Vector3 en)
    {
        while(rt.anchoredPosition.y<0 || canvasGroup.alpha<0.99 || camera_main.transform.position.x<en.x+4.5f-0.2f)
        {
            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, new Vector2(200,0.5f), 0.05f);
            camera_main.transform.position = Vector3.Lerp(camera_main.transform.position, new Vector3(en.x+4.5f, en.y, -3), 0.1f);
            GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1, 0.05f);
            yield return new WaitForSecondsRealtime(0.002f);
        }
        select_and_show_target();
    }
    IEnumerator hide_fight_screen()
    {
        while(rt.anchoredPosition.y>-600 && canvasGroup.alpha>0.01)
        {
            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, new Vector2(200,-600), 0.05f);
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, 0.05f);
            yield return new WaitForSecondsRealtime(0.002f);
        }
        on_fight = false;
    }
    void select_and_show_target()
    {
        for(int i=0; i<4; i++)
        {
            GameObject point = Instantiate(target_point, new Vector3(0,0,0), Quaternion.identity, Dummy);
            point.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-40,40), Random.Range(-40,40));
        }
        StartCoroutine(click_point());
    }
    IEnumerator click_point()
    {
        while(true)
        {
            GameObject[] obj = GameObject.FindGameObjectsWithTag("Point");
            if(obj.Length==0) break;
            yield return new WaitForSecondsRealtime(0.1f);
        }
        Destroy(dd);
        on_fight = false;
        StartCoroutine(hide_fight_screen());
        Time.timeScale = 1f;
        GetComponent<Player_Movement>().enabled = true;
    }
}
