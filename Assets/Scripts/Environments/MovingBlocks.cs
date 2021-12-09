using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlocks : MonoBehaviour
{
    public GameObject Level;
    private Rigidbody2D rb;

    public int mass = 5;

    // Start is called before the first frame update
    void Start()
    {
        rb= GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool canPush(){
        if (Level.GetComponent<CharacterChangeManager>().currentState == CharacterChangeManager.State.Excited){
            return true;
        } else {
            return false;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision){ 
        if (canPush()){
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.mass = mass;
        } else {
            rb.bodyType = RigidbodyType2D.Static;
        }
    }  

    private void OnCollisionExit2D(Collision2D collision){ 
        if (canPush()){
            rb.bodyType = RigidbodyType2D.Static;
        }
    }  
}
