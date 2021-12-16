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
    public AnimatorOverrideController FocusAnim;
    public AnimatorOverrideController StressAnim;


    public bool forceActive = false;
    public bool forcedCalm = false;
    public bool forcedExcited = false;
    public bool forcedFocus = false;
    public bool forcedStress = false;
    public Image fillTimer;

    public float timeRemaining = 30;
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
        if (Character.GetComponent<CharacterChangeManager>().collectedEmotions[3] > 0 && !timerIsRunning){
            calmButton.GetComponent<Button>().interactable = true;

        } else {
            calmButton.GetComponent<Button>().interactable = false;
        }
        if (Character.GetComponent<CharacterChangeManager>().collectedEmotions[1] > 0 && !timerIsRunning) {
            excitedButton.GetComponent<Button>().interactable = true;
        } else {
            excitedButton.GetComponent<Button>().interactable = false;
        }

        if (Character.GetComponent<CharacterChangeManager>().collectedEmotions[0] > 0 && !timerIsRunning) {
            focusButton.GetComponent<Button>().interactable = true;
        } else {
            focusButton.GetComponent<Button>().interactable = false;
        }
        if (Character.GetComponent<CharacterChangeManager>().collectedEmotions[2] > 0 && !timerIsRunning) {
            stressButton.GetComponent<Button>().interactable = true;
        } else {
            stressButton.GetComponent<Button>().interactable = false;
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
        anim.runtimeAnimatorController = CalmAnim as RuntimeAnimatorController;
        Player.GetComponent<SpriteRenderer>().color = Color.white;
        Character.GetComponent<CharacterChangeManager>().currentState = CharacterChangeManager.State.Calm;
        MainUIController.GetComponent<UIController>().slots[3].transform.GetChild(Character.GetComponent<CharacterChangeManager>().collectedEmotions[3]).gameObject.SetActive(false);
        Character.GetComponent<CharacterChangeManager>().collectedEmotions[3] -= 1;

        timerIsRunning = true;
        forcedCalm = true;
        forceActive = true;
        //renderCanvas();
    }

    public void onClickExcited()
    {
        anim.runtimeAnimatorController = ExcitedAnim as RuntimeAnimatorController;
        Player.GetComponent<SpriteRenderer>().color = Color.white;
        Character.GetComponent<CharacterChangeManager>().currentState = CharacterChangeManager.State.Excited;
        MainUIController.GetComponent<UIController>().slots[1].transform.GetChild(Character.GetComponent<CharacterChangeManager>().collectedEmotions[1]).gameObject.SetActive(false);
        Character.GetComponent<CharacterChangeManager>().collectedEmotions[1] -= 1;

        timerIsRunning = true;
        forcedExcited = true;
        forceActive = true;
        //renderCanvas();

    }

    public void onClickFocus()
    {
        anim.runtimeAnimatorController = FocusAnim as RuntimeAnimatorController;
        Player.GetComponent<SpriteRenderer>().color = Color.gray;
        Character.GetComponent<CharacterChangeManager>().currentState = CharacterChangeManager.State.Focus;
        MainUIController.GetComponent<UIController>().slots[0].transform.GetChild(Character.GetComponent<CharacterChangeManager>().collectedEmotions[0]).gameObject.SetActive(false);
        Character.GetComponent<CharacterChangeManager>().collectedEmotions[0] -= 1;

        timerIsRunning = true;
        forcedFocus = true;
        forceActive = true;
    }


    public void onClickStress()
    {
        anim.runtimeAnimatorController = StressAnim as RuntimeAnimatorController;
        Player.GetComponent<SpriteRenderer>().color = Color.red;
        Character.GetComponent<CharacterChangeManager>().currentState = CharacterChangeManager.State.Stress;
        MainUIController.GetComponent<UIController>().slots[2].transform.GetChild(Character.GetComponent<CharacterChangeManager>().collectedEmotions[2]).gameObject.SetActive(false);
        Character.GetComponent<CharacterChangeManager>().collectedEmotions[2] -= 1;

        timerIsRunning = true;
        forcedStress = true;
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
