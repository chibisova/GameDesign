using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingBlocks : MonoBehaviour
{
    public GameObject Level;
    public bool breakable = true;

    public float falldelay = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        breakable = checkState();

    }

    bool checkState(){
        if (Level.GetComponent<CharacterChangeManager>().currentState == CharacterChangeManager.State.Calm){
            return false;
        } else {
            return true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {   
        if (collision.gameObject.tag.Equals("Player")){
            if (breakable){
                StartCoroutine(fall());
            } else {
                // do something;
            }
        }
            
    }

    IEnumerator fall(){
        // yield return new WaitForSeconds(falldelay);
        // TODO: find better way for falling platforms
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;

        yield return new WaitForSeconds(3.0f);

        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;

        yield return 0;
    }

}
