using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int soulsCollected;

    // UI Related
    public Text fragmentCountText;
    public Slider progress;
    //public Text soulsCountText;

    // public GameObject popup;
    // private bool popupActive;

    //Player control
    public GameObject Player;

    // Pause Management
    private bool gamePaused = false;
    public GameObject PauseMenu;

    // Music Select
    private bool selectActive = false;
    public GameObject SelectMenu;

    private bool forceChange = false;
    public GameObject ForceChange;

    void Start()
    {
        soulsCollected = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // if (popupActive){
        //     popup.SetActive(true);
        //     Player.GetComponent<PlayerController>().enabled = false;
        //     if(Input.GetKeyDown(KeyCode.Space)){
        //         popupActive=false;
        //     }
        // } else {
        //     popup.SetActive(false);
        //     Player.GetComponent<PlayerController>().enabled = true;
        // }

        if(Input.GetKeyDown(KeyCode.Escape)){
            gamePaused = !gamePaused; // check for this first
        }

        if (Input.GetKeyDown(KeyCode.Q) && !gamePaused){
            selectActive = !selectActive;
        }

        if (Input.GetKeyDown(KeyCode.F) && !forceChange)
        {
            forceChange = !forceChange;
        }

        if (gamePaused){
            Player.GetComponent<PlayerController>().enabled= false;
            PauseMenu.SetActive(true);
        } else {
            Player.GetComponent<PlayerController>().enabled= true;
            PauseMenu.SetActive(false);
        }

        if (selectActive){
            Player.GetComponent<PlayerController>().enabled= false;
            SelectMenu.SetActive(true);
        } else {
            Player.GetComponent<PlayerController>().enabled= true;
            SelectMenu.SetActive(false);
        }


        if (forceChange)
        {
            Player.GetComponent<PlayerController>().enabled = false;
            ForceChange.SetActive(true);
        }
        else
        {
            Player.GetComponent<PlayerController>().enabled = true;
            ForceChange.SetActive(false);
        }
    }

    // public void collectSoul(){
    //     soulsCollected++;
    //     //updateText();
    //     if(soulsCollected == 5){
    //         popupActive = true;
    //         soulsCollected = 0;
    //     }

    //     progress.value = soulsCollected;
    // }


    public void ExitToMap(){
        SceneManager.LoadScene("MapScene");
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void Resume(){
        gamePaused = false;
    }

    public void SetAtmosActive(){
        selectActive = true;
        gameObject.GetComponent<MusicController>().PauseTimer();
    }
    public void SetAtmosInActive(){
        selectActive = false;
        gameObject.GetComponent<MusicController>().ContinueTimer();
    }

}
