using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class dialogue_system : MonoBehaviour
{
    
    public List<string> text = new List<string>();
    public GameObject dialogue;
    // Start is called before the first frame update
    void Start()
    {
        print("start");
        StartCoroutine(start_dialogue(0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator start_dialogue(int index)
    {
        dialogue.GetComponent<TMP_Text>().text = "";
        for(int i = 0; i < text[index].ToString().Length; i++)
        {
            yield return new WaitForSeconds(0.05f);
            dialogue.GetComponent<TMP_Text>().text += text[index][i].ToString();
        }
    }
}
