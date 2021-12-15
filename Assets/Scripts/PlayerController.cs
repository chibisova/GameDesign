using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public Camera mainCamera;

    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private Animator anim;
    private Transform t;

    private float dirX = 0f;
    private bool facingRight = true;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 10f;

    private Vector2 velocity;

    [SerializeField] private LayerMask jumpableGround;

    // Respawn
    public bool dead = false;
    public GameObject SpawnPoint;

    public GameObject background;


    // Soul Dialogue
    public bool neverMetSoul = true;

    public GameObject Level;

    void Start (){
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        t = GetComponent<Transform>();

        background.GetComponent<Renderer>().material.color = Color.white;
    }

    void Update(){
        dirX = Input.GetAxisRaw("Horizontal");
        velocity.x = dirX * moveSpeed;


        if (dirX != 0f){
            anim.SetBool("running", true);
            if (dirX > 0 && !facingRight)
            {
                facingRight = true;
                t.localScale = new Vector3(Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
            }
            if (dirX < 0 && facingRight)
            {
                facingRight = false;
                t.localScale = new Vector3(-Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
            }
        } else{
            anim.SetBool("running", false);
        };


        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (dead){
            Respawn();
        }
    
        //ChangeMode();

        // if (Input.GetKeyDown(KeyCode.Q)){

        // }

        if (Level.GetComponent<CharacterChangeManager>().currentState == CharacterChangeManager.State.Stress){
            moveSpeed = 9.0f;
        } else{
            moveSpeed = 7.0f;
        }
    }

    void FixedUpdate(){
        rb.velocity = new Vector2(velocity.x, rb.velocity.y);
        HandleAnimations();
    }

    private bool IsGrounded()
    {
        // if (rb.velocity.y = 0f) {return false;} 
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }


    private void HandleAnimations()
    {
        if (!IsGrounded())
        {
            anim.SetBool("isGrounded", false);

            //Set the animator velocity equal to 1 * the vertical direction in which the player is moving 
            anim.SetFloat("velocityY", 1 * Mathf.Sign(rb.velocity.y));
        }

        if (IsGrounded())
        {   
            if (rb.velocity.y <= 0.0001f) {
                anim.SetBool("isGrounded", true);
                anim.SetFloat("velocityY", 0);
            }
        }
    }

    public void Respawn(){
        transform.position = SpawnPoint.transform.position;
        //some effect
        dead = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {   
        //Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag.Equals("soul")) {
            // Add wildcard slot
            //CharacterChangeManager.collectedEmotions[4]++;
            int val = Level.GetComponent<CharacterChangeManager>().collectedEmotions[4];

            if (val < 3){
                Level.GetComponent<CharacterChangeManager>().collectedEmotions[4]++;
                GameObject.Find("Wildcard (new)").transform.Find("Counter").Find("Text").GetComponent<Text>().text = (Level.GetComponent<CharacterChangeManager>().collectedEmotions[4] - 1).ToString();
                Debug.Log("Bonus: " + Level.GetComponent<CharacterChangeManager>().collectedEmotions[4]);
            }

            Destroy(collision.gameObject);
        }
    }

    // private void onTriggerEnter2D(Collision2D collision){
    //     Debug.Log(collision.gameObject.tag);
    //     if (collision.gameObject.tag.Equals("soul")) {
    //         Debug.Log("Something collided");
    //         // Add wildcard slot
    //         //CharacterChangeManager.collectedEmotions[4]++;
    //         int val = gameObject.GetComponent<CharacterChangeManager>().collectedEmotions[4];

    //         if (val < 3){
    //             gameObject.GetComponent<CharacterChangeManager>().collectedEmotions[4]++;
    //         }

    //         Destroy(collision.gameObject);
    //     }
    // }
}