using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCollider : MonoBehaviour
{
    private bool completed = false;
    private bool active = false;
    public GameObject Canvas;
    public Text Text;
    public string TutorialMessage = "";
    public GameObject Player;

    // Update is called once per frame
    void Update()
    {
        if (active){
            if (Input.GetKeyDown(KeyCode.Return)){
                completed = true;
                active = false;
                Player.GetComponent<PlayerController>().enabled = true;
                Canvas.SetActive(false);
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if (collision.gameObject.tag.Equals("Player") && !completed){
            active = true;
            Player.GetComponent<PlayerController>().enabled = false;
            Canvas.SetActive(true);
            Text.text = TutorialMessage;
        }
    }
}
