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
    public bool forcedCalm = false;
    public bool forcedExcited = false;
    public Image fillTimer;

    public float timeRemaining = 15;
    public bool timerIsRunning = false;

    public string[] currAnswer;
    public int currPos = 0;

    public Button calmButton;
    public Button excitedButton;
    public Button stressButton;
    public Button focusButton;


    public GameObject MainUIController;

    // Update is called once per frame
    void Update()
    {
        if (Character.GetComponent<CharacterChangeManager>().collectedEmotions[3] > 0 && !timerIsRunning)
        {
            calmButton.GetComponent<Button>().interactable = true;

        }
        if (Character.GetComponent<CharacterChangeManager>().collectedEmotions[1] > 0 && !timerIsRunning)
        {
            excitedButton.GetComponent<Button>().interactable = true;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        anim = Player.GetComponent<Animator>();
        fillTimer.fillAmount = 1f;
        emotionSetup = Character.GetComponent<CharacterChangeManager>().collectedEmotions;
        focusButton.GetComponent<Button>().interactable = false;
        stressButton.GetComponent<Button>().interactable = false;
        calmButton.GetComponent<Button>().interactable = false;
        excitedButton.GetComponent<Button>().interactable = false;
    }

    public void onClickCalm()
    {
        if (Character.GetComponent<CharacterChangeManager>().collectedEmotions[3] > 0)
        {
            anim.runtimeAnimatorController = CalmAnim as RuntimeAnimatorController;
            MainUIController.GetComponent<UIController>().slots[3].transform.GetChild(Character.GetComponent<CharacterChangeManager>().collectedEmotions[3]).gameObject.SetActive(false);
            Character.GetComponent<CharacterChangeManager>().collectedEmotions[3] -= 1;

            timerIsRunning = true;
            forcedCalm = true;
            forceActive = true;
            calmButton.GetComponent<Button>().interactable = false;
            excitedButton.GetComponent<Button>().interactable = false;
            //renderCanvas();
        } else
        {
            calmButton.GetComponent<Button>().interactable = false;
        }
    }

    public void onClickExcited()
    {
        if (Character.GetComponent<CharacterChangeManager>().collectedEmotions[1] > 0)
        {
            anim.runtimeAnimatorController = ExcitedAnim as RuntimeAnimatorController;
            MainUIController.GetComponent<UIController>().slots[1].transform.GetChild(Character.GetComponent<CharacterChangeManager>().collectedEmotions[1]).gameObject.SetActive(false);
            Character.GetComponent<CharacterChangeManager>().collectedEmotions[1] -= 1;

            timerIsRunning = true;
            forcedExcited = true;
            forceActive = true;
            calmButton.GetComponent<Button>().interactable = false;
            excitedButton.GetComponent<Button>().interactable = false;
            //renderCanvas();
        } else
        {
            excitedButton.GetComponent<Button>().interactable = false;
        }

    }

    public void onClickFocus()
    {


    }


    public void onClickStress()
    {

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
