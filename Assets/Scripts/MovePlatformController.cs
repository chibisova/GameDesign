using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatformController : MonoBehaviour
{

    
    [SerializeField]
    private Transform targetA, targetB;
    private bool targetReached = false;
    private float speed = 2.0f;

    public float distance;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(targetReached == false){
            transform.position = Vector3.MoveTowards(transform.position, targetB.position, speed * Time.deltaTime);
        }else if(targetReached == true){
            transform.position = Vector3.MoveTowards(transform.position, targetA.position, speed * Time.deltaTime);
        }

        if(transform.position == targetB.position){
            targetReached = true;
        }

        if(transform.position == targetA.position){
            targetReached = false;
        }

    }

    private void OnCollisionEnter2D(Collision2D other){
        if(other.gameObject.tag.Equals("Player")){
            Debug.Log("Should stick now");
            other.transform.parent = this.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D other){
        if(other.gameObject.tag.Equals("Player")){
            Debug.Log("Should not stick now");
            other.transform.parent = null;
        }
    }


}
