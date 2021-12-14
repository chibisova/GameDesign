using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ForceChange : MonoBehaviour
{
    public GameObject Character;
    public GameObject Player;

    private Animator anim;
    public AnimatorOverrideController CalmAnim;
    public AnimatorOverrideController ExcitedAnim;
    public AnimatorOverrideController BaselineAnim;
    // Start is called before the first frame update
    void Start()
    {
        anim = Player.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void onClickCalm()
    {
        if (Character.GetComponent<CharacterChangeManager>().collectedEmotions[0] > 0)
        {
            anim.runtimeAnimatorController = CalmAnim as RuntimeAnimatorController;
            Character.GetComponent<CharacterChangeManager>().collectedEmotions[0]--;
            //timer.set;
            //Close Window
        }
    }

    public void onClickExcited()
    {
        if (Character.GetComponent<CharacterChangeManager>().collectedEmotions[2] > 0)
        {
            anim.runtimeAnimatorController = ExcitedAnim as RuntimeAnimatorController;
            Character.GetComponent<CharacterChangeManager>().collectedEmotions[2]--;
            //timer.set;
            //Close Window
        }

    }

    public void onClickFocus()
    {
        
        
    }

   
    public void onClickStress()
    {
        
    }

    
}
