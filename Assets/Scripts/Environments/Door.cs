using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject DoorPuzzle;
    public GameObject Player;
    public GameObject Level;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if (collision.gameObject.tag.Equals("Player")) {
            DoorPuzzle.SetActive(true);
            DoorPuzzle.GetComponent<DoorPuzzle>().onLoadGame();
            Player.GetComponent<PlayerController>().enabled = false;
            Level.GetComponent<CharacterChangeManager>().StopCoroutine(Level.GetComponent<CharacterChangeManager>().CalculateEmotion());
        }
    }
}
