using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ForceChange : MonoBehaviour
{
    public GameObject Character;
    public GameObject Player;

    public int[] emotionSetup;


    private Animator anim;
    public AnimatorOverrideController CalmAnim;
    public AnimatorOverrideController ExcitedAnim;
    public AnimatorOverrideController BaselineAnim;

    public bool forceActive = false;

    public float timeRemaining = 30;
    public bool timerIsRunning = false;


    public string[] currAnswer;
    public int currPos = 0;

    public GameObject MainUIController;

    // Update is called once per frame
    void Update()
    {

    }


    // Start is called before the first frame update
    void Start()
    {
        anim = Player.GetComponent<Animator>();

        emotionSetup = Character.GetComponent<CharacterChangeManager>().collectedEmotions;
    }

    public void onClickCalm()
    {
        if (Character.GetComponent<CharacterChangeManager>().collectedEmotions[3] > 0)
        {
            anim.runtimeAnimatorController = CalmAnim as RuntimeAnimatorController;
            MainUIController.GetComponent<UIController>().slots[3].transform.GetChild(Character.GetComponent<CharacterChangeManager>().collectedEmotions[3]).gameObject.SetActive(false);
            //Debug.Log("UI change: " + MainUIController.GetComponent<UIController>().slots[3].transform.GetChild(Character.GetComponent<CharacterChangeManager>().collectedEmotions[0]-1));
            Character.GetComponent<CharacterChangeManager>().collectedEmotions[3] -= 1;
            forceIsActive();
            //Debug.Log("Check: " + MainUIController.GetComponent<UIController>().slots[3].transform.GetChild(Character.GetComponent<CharacterChangeManager>().collectedEmotions[3]));
            timerIsRunning = true;
            //renderCanvas();
        }
    }

    public void onClickExcited()
    {
        if (Character.GetComponent<CharacterChangeManager>().collectedEmotions[1] > 0)
        {
            anim.runtimeAnimatorController = ExcitedAnim as RuntimeAnimatorController;
            MainUIController.GetComponent<UIController>().slots[1].transform.GetChild(Character.GetComponent<CharacterChangeManager>().collectedEmotions[1]).gameObject.SetActive(false);
            //Debug.Log("UI change: " + MainUIController.GetComponent<UIController>().slots[1].transform.GetChild(Character.GetComponent<CharacterChangeManager>().collectedEmotions[2]-1));

            Character.GetComponent<CharacterChangeManager>().collectedEmotions[1] -= 1;
            forceIsActive();
            //Debug.Log("Check 2: " + Character.GetComponent<CharacterChangeManager>().collectedEmotions[1]);
            timerIsRunning = true;
            //renderCanvas();
        }

    }

    public void onClickFocus()
    {


    }


    public void onClickStress()
    {

    }

    public void forceIsActive()
    {
        forceActive = true;

    }
    /*
    public void renderCanvas()
    {
        emotionSetup = Character.GetComponent<CharacterChangeManager>().collectedEmotions;
        for (int i = 0; i < emotionsRemaining.Length; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (emotionsRemaining[i] > j)
                {
                    slots[i].transform.GetChild(j).gameObject.SetActive(true);
                }
                else
                {
                    slots[i].transform.GetChild(j).gameObject.SetActive(false);
                }
            }
        }

    }*/
}
