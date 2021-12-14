using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{
    public AudioSource source;

    public AudioClip baseline;
    public AudioClip calm;
    public AudioClip excited;
    public AudioClip stress;
    public AudioClip focus;

    public GameObject Player;
    public GameObject Fog;
    public GameObject CalmParticles;
    public GameObject ExcitedParticles;
    public GameObject StressParticles;
    public GameObject AtmosphereTitle;
    public Text AtmosphereTitleText;
    public enum ChargeState {Calm, Stress, Excited, Focus}
    //public enum HeartRateBar { HeartRate }
    public ChargeState currentChargeState;
    public GameObject chargePopUp;
    public bool atmTimerIsRunning = false;
    public float atmTimeRemaining = 0;
    private float calTimeRemaining = 0;
    private float excTimeRemaining = 0;
    private float focTimeRemaining = 0;
    private float strTimeRemaining = 0;


    private CharacterChangeManager Character;

    // Start is called before the first frame update
    void Start()
    {
        Character = Player.GetComponent<CharacterChangeManager>();
        playBase();
        calTimeRemaining = (float) 120.0;
        excTimeRemaining = (float) 120.0;
        focTimeRemaining = (float) 120.0;
        strTimeRemaining = (float) 120.0;
    }

    // Update is called once per frame
    void Update()
    {
         if (atmTimerIsRunning)
        {
            if (atmTimeRemaining > 0)
            {
                atmTimeRemaining -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Time has run out!");
                Character.boostedState = CharacterChangeManager.State.Baseline;
                atmTimeRemaining = 0;
                atmTimerIsRunning = false;
            }
        }       
    }

    public void playBase(){
        source.clip = baseline;
        Character.boostedState = CharacterChangeManager.State.Baseline;
        source.Play();
        Fog.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        AtmosphereTitleText.text = "Baseline";
        AtmosphereTitle.GetComponent<UnityEngine.UI.Text>().color= new Color32(255, 255, 255, 255);
        CalmParticles.SetActive(false);
        ExcitedParticles.SetActive(false);
        StressParticles.SetActive(false);
    }

    public void playCalm(){
        if(calTimeRemaining > 0){
            source.clip = calm;
            Character.boostedState = CharacterChangeManager.State.Calm;
            source.Play();
            Fog.GetComponent<SpriteRenderer>().color = new Color32(52, 161, 207, 255);
            AtmosphereTitleText.text = "Relaxed";
            AtmosphereTitle.GetComponent<UnityEngine.UI.Text>().color= new Color32(52, 161, 207, 255);
            CalmParticles.SetActive(true);
            ExcitedParticles.SetActive(false);
            StressParticles.SetActive(false);  
            atmTimeRemaining = calTimeRemaining;
            atmTimerIsRunning = true;    
        }
        else{
            Debug.Log("Not enough charge!");
        }
    }

    public void playExcited(){
        if(calTimeRemaining > 0){
        source.clip = excited;
        Character.boostedState = CharacterChangeManager.State.Excited;
        source.Play();
        Fog.GetComponent<SpriteRenderer>().color = new Color32(255, 245, 102, 255);
        AtmosphereTitleText.text = "Excited";
        AtmosphereTitle.GetComponent<UnityEngine.UI.Text>().color= new Color32(255, 245, 102, 255);
        CalmParticles.SetActive(false);
        ExcitedParticles.SetActive(true);
        StressParticles.SetActive(false);
        }
        else{
            Debug.Log("Not enough charge!");
        }
    }

    public void playStress(){
        if(strTimeRemaining > 0){
        source.clip = stress;
        Character.boostedState = CharacterChangeManager.State.Stress; 
        source.Play();
        Fog.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255);
        AtmosphereTitleText.text = "Stressed";
        AtmosphereTitle.GetComponent<UnityEngine.UI.Text>().color= new Color32(255, 0, 0, 255);
        CalmParticles.SetActive(false);
        ExcitedParticles.SetActive(false);
        StressParticles.SetActive(true);
        }
        else{
            Debug.Log("Not enough charge!");
        }
    }

    public void playFocus(){
        if(focTimeRemaining > 0){
        source.clip = focus;
        Character.boostedState = CharacterChangeManager.State.Focus;
        source.Play();
        Fog.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 150);
        AtmosphereTitleText.text = "Focused";
        AtmosphereTitle.GetComponent<UnityEngine.UI.Text>().color= new Color32(255, 255, 255, 150);
        CalmParticles.SetActive(false);
        ExcitedParticles.SetActive(false);
        StressParticles.SetActive(false);
        }
        else{
            Debug.Log("Not enough charge!");
        }
    }

    public void StartAtmTimer(){
        atmTimerIsRunning = true;
    }
    public void StopAtmTimer(){
        atmTimerIsRunning = false;
        switch(Character.boostedState){
            case CharacterChangeManager.State.Calm:
                calTimeRemaining = atmTimeRemaining;
            break;
            case CharacterChangeManager.State.Excited:
                excTimeRemaining = atmTimeRemaining;
            break;
            case CharacterChangeManager.State.Focus:
                focTimeRemaining = atmTimeRemaining;
            break;
            case CharacterChangeManager.State.Stress:
                strTimeRemaining = atmTimeRemaining;
            break;
        }
        playBase();
    }

    public void ChargeCal(){
        currentChargeState = ChargeState.Calm;
        chargePopUp.SetActive(true);
    }
    
    public void ChargeFoc(){
        currentChargeState = ChargeState.Focus;
        chargePopUp.SetActive(true);
    }

    public void ChargeExc(){
        currentChargeState = ChargeState.Excited;
        chargePopUp.SetActive(true);
    }
    public void ChargeStr(){
        currentChargeState = ChargeState.Stress;
        chargePopUp.SetActive(true);
    }
    public void Charge(){
        int amount = chargePopUp.GetComponent<DoorPuzzle>().Answers.Length;
         switch(currentChargeState){
            case ChargeState.Calm:
                calTimeRemaining += (float) 120.0 * amount;
                break;
            case ChargeState.Focus:
                focTimeRemaining += (float) 120.0 * amount;
                break;
            case ChargeState.Excited:
                excTimeRemaining += (float) 120.0 * amount;
                break;
            case ChargeState.Stress:
                strTimeRemaining += (float) 120.0 * amount;
                break;
        }
    }
}
