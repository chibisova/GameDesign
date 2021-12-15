using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ChargingAtmos : MonoBehaviour
{

    public string[] answerlist;
    public int[] emotionSetup;
    public int[] emotionsRemaining;

    public GameObject Level;
    public GameObject Player;
    public Color32 calmColor =  new Color32(57, 187, 255, 255);
    public Color32 excitedColor = new Color32(255, 212, 57, 255);
    public Color32 stressColor = new Color32(255, 0, 0, 255);
    public Color32 focusColor = new Color32(255, 255, 255, 255);
    public Color32 baseColor = new Color32(138, 138, 138, 255);

    public string[] currAnswer;
    public int currPos = 0;

    public GameObject[] slots = new GameObject[5];
    public GameObject[] Answers = new GameObject[5];


    // Indicators
    public GameObject Fail;
    public GameObject Success;
    public GameObject CheckButton;

    void Start()
    {
        answerlist = new string[5] {"S", "F", "E", "R", "E"};
        currAnswer = new string[5];
        emotionSetup = Level.GetComponent<CharacterChangeManager>().collectedEmotions;
        //Debug.Log(emotionSetup[0]);
        emotionsRemaining = new int[5];
        //emotionSetup = new int[5];
        onLoadGame();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void renderCanvas(){
        emotionSetup = Level.GetComponent<CharacterChangeManager>().collectedEmotions;
        for (int i = 0; i<emotionsRemaining.Length; i++){
            for (int j = 0; j<3; j++){
                if (emotionsRemaining[i] > j){
                    slots[i].transform.GetChild(j).gameObject.SetActive(true);
                } else{
                    slots[i].transform.GetChild(j).gameObject.SetActive(false); 
                }
            }
        }


        for (int i= 0; i<currPos; i++){
            Answers[i].SetActive(true);
            if (currAnswer[i] == "F"){
                Answers[i].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            } else if (currAnswer[i] == "E"){
                Answers[i].GetComponent<Image>().color = new Color32(255, 209, 78, 255);
            } else if (currAnswer[i] == "R"){
                Answers[i].GetComponent<Image>().color = new Color32(78, 131, 255, 255);
            } else if (currAnswer[i] == "S"){
                Answers[i].GetComponent<Image>().color = new Color32(255, 78, 101, 255);
            } else if (currAnswer[i] == "W"){
                Answers[i].GetComponent<Image>().color = new Color32(178, 78, 255, 255);
            }
        }

    }


    public void onLoadGame(){
        for (int i = 0; i<5; i++){
            emotionsRemaining[i] = emotionSetup[i];
        }
        currPos = 0;
        currAnswer = new string[5];
        for (int i= 0; i<5; i++){
            Answers[i].SetActive(false);
        }
        renderCanvas();
    }

    public void onClickFocus(){
        if (currPos < answerlist.Length){
            currAnswer[currPos] = "F";
            //Debug.Log(currAnswer[currPos]);
            currPos++;
            emotionsRemaining[0]--;
            renderCanvas();
        }
    }

    public void onClickExcited(){
        if (currPos < answerlist.Length){
            currAnswer[currPos] = "E";
            currPos++;
            emotionsRemaining[1]--;
            renderCanvas();
        }
    }

    public void onClickStress(){
        if (currPos < answerlist.Length){
            currAnswer[currPos] = "S";
            currPos++;
            emotionsRemaining[2]--;
            renderCanvas();
        }
    }

    public void onClickCalm(){
        if (currPos < answerlist.Length){
            currAnswer[currPos] = "R";
            currPos++;
            emotionsRemaining[3]--;
            renderCanvas();
        }
    }

    public void onClickWildCard(){
        if (currPos < answerlist.Length){
            currAnswer[currPos] = "W";
            currPos++;
            emotionsRemaining[4]--;
            renderCanvas();
        }
    }

    public void exitGame(){
        gameObject.SetActive(false);
        Player.GetComponent<PlayerController>().enabled = true;
        Level.GetComponent<CharacterChangeManager>().StartCoroutine(Level.GetComponent<CharacterChangeManager>().CalculateEmotion());
        currPos = 0;
        currAnswer = new string[5];
        for (int i= 0; i<5; i++){
            Answers[i].SetActive(false);
        }
        Level.GetComponent<CharacterChangeManager>().collectedEmotions = emotionsRemaining;
    }
}
