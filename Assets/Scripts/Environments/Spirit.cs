using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spirit : MonoBehaviour
{
    //private bool collectible = false;
    // private bool isCollected = false;

    public LevelManager Level;
    public GameObject Player;
    public CharacterChangeManager Character;
    void Start()
    {
        Character = Player.GetComponent<CharacterChangeManager>();
    }

    //  private void OnCollisionEnter2D(Collision2D collision)
    // {   
    //     Debug.Log(collision.gameObject.tag);
    //     if (collision.gameObject.tag.Equals("Player")) {
    //         // Add wildcard slot
    //         Debug.Log("It collided yey");
    //         Character.collectedEmotions[4]++;
    //         Destroy(gameObject);
    //     }
    // }
}

