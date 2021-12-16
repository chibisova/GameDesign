using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlocks : MonoBehaviour
{
    public GameObject Level;
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    [SerializeField] private LayerMask jumpableGround;

    public int mass = 5;

    // Start is called before the first frame update
    void Start()
    {
        rb= GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(IsGrounded());
        // if (!IsGrounded()){
        //     rb.bodyType = RigidbodyType2D.Dynamic;
        // }
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
        rb.velocity = new Vector2(0, rb.velocity.y);
    }  
}
