using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCharacterController : MonoBehaviour
{
    private bool completed = false;
    public GameObject Target;

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if (collision.gameObject.tag.Equals("Player") && !completed){
            Target.GetComponent<CharacterChangeManager>().enabled=true;
            completed = true;
        }
    }
}
