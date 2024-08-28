using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_click : MonoBehaviour
{
    void isClickedTrue()
    {
        Destroy(this.gameObject);
    }
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(isClickedTrue);
    }

}
