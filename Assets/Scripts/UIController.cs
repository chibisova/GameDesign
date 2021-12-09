using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public int[] emotionSetup;
    public int[] emotionsRemaining;
    public GameObject Level;
    public GameObject[] slots = new GameObject[5];


    
    // Start is called before the first frame update
    void Start()
    {
        emotionSetup = Level.GetComponent<CharacterChangeManager>().collectedEmotions;
        //Debug.Log(emotionSetup[0]);
        //emotionsRemaining = new int[5];
        //emotionSetup = new int[5];
        // Debug.Log("Now the second spirit should be activated");
        // slots[0].transform.GetChild(2).gameObject.SetActive(true);
        StartCoroutine(updateUI());

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void renderSpirits(){
        emotionSetup = Level.GetComponent<CharacterChangeManager>().collectedEmotions;
        for (int i = 0; i<emotionSetup.Length; i++){
            for (int j = 0; j<3; j++){
                if (emotionSetup[i] > j){
                    slots[i].transform.GetChild(j).gameObject.SetActive(true);
                } else{
                    slots[i].transform.GetChild(j).gameObject.SetActive(false); 
                }
            }
        }
    }

    IEnumerator updateUI(){
        while(true){
            yield return new WaitForSeconds(0.2f);
            renderSpirits();
        }
    }
    
    public void onLoadGame(){
        for (int i = 0; i<5; i++){
            emotionsRemaining[i] = emotionSetup[i];
            Debug.Log("It passed the error");
        }
        Debug.Log("Now should render");
        renderSpirits();
    }

}
