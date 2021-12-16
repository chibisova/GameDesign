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
    public GameObject excitedTimeText;
    public GameObject focusTimeText;
    public GameObject stressTimeText;
    public GameObject relaxedTimeText;
    
    public enum ChargeState {Calm, Stress, Excited, Focus}
    //public enum HeartRateBar { HeartRate }
    public ChargeState currentChargeState;
    public GameObject chargePopUp;
    public GameObject notEnoughPopup;
    public float chargeTime = 30.0f;
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
        calTimeRemaining = chargeTime;
        excTimeRemaining = chargeTime;
        focTimeRemaining = chargeTime;
        strTimeRemaining = chargeTime;
        Debug.Log("Start is called");
        chargePopUp.SetActive(false);
        notEnoughPopup.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAtmosTimers();
        excitedTimeText.GetComponent<UnityEngine.UI.Text>().text = Mathf.FloorToInt(excTimeRemaining / 60).ToString() + ":" + Mathf.FloorToInt(excTimeRemaining % 60).ToString();
        focusTimeText.GetComponent<UnityEngine.UI.Text>().text = Mathf.FloorToInt(focTimeRemaining / 60).ToString() + ":" + Mathf.FloorToInt(focTimeRemaining % 60).ToString();
        stressTimeText.GetComponent<UnityEngine.UI.Text>().text = Mathf.FloorToInt(strTimeRemaining / 60).ToString() + ":" + Mathf.FloorToInt(strTimeRemaining % 60).ToString();
        relaxedTimeText.GetComponent<UnityEngine.UI.Text>().text = Mathf.FloorToInt(calTimeRemaining / 60).ToString() + ":" + Mathf.FloorToInt(calTimeRemaining % 60).ToString();

        if (atmTimerIsRunning)
        {
            if (atmTimeRemaining > 0)
            {
                atmTimeRemaining -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Time has run out!");
                atmTimeRemaining = 0;
                StopAtmTimer();
                playBase();
                atmTimerIsRunning = false;
                Debug.Log("Should play Base now");
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
            gameObject.GetComponent<LevelManager>().SetAtmosInActive();
            StopAtmTimer();
            Debug.Log("Calm Time remaining: " + calTimeRemaining);
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
            StartCoroutine(NotEnoughCharge());
        }
    }

    public void playExcited(){
        if(excTimeRemaining > 0){
            gameObject.GetComponent<LevelManager>().SetAtmosInActive();
            StopAtmTimer();
            Debug.Log("Excited Time remaining: " + excTimeRemaining);
            source.clip = excited;
            Character.boostedState = CharacterChangeManager.State.Excited;
            source.Play();
            Fog.GetComponent<SpriteRenderer>().color = new Color32(255, 245, 102, 255);
            AtmosphereTitleText.text = "Excited";
            AtmosphereTitle.GetComponent<UnityEngine.UI.Text>().color= new Color32(255, 245, 102, 255);
            CalmParticles.SetActive(false);
            ExcitedParticles.SetActive(true);
            StressParticles.SetActive(false);
            atmTimeRemaining = excTimeRemaining;
            atmTimerIsRunning = true;  
        }
        else{
            Debug.Log("Not enough charge!");
            StartCoroutine(NotEnoughCharge());
        }
    }

    public void playStress(){
        if(strTimeRemaining > 0){
            gameObject.GetComponent<LevelManager>().SetAtmosInActive();
            StopAtmTimer();
            Debug.Log("StressTime remaining: " + strTimeRemaining);
            source.clip = stress;
            Character.boostedState = CharacterChangeManager.State.Stress; 
            source.Play();
            Fog.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255);
            AtmosphereTitleText.text = "Stressed";
            AtmosphereTitle.GetComponent<UnityEngine.UI.Text>().color= new Color32(255, 0, 0, 255);
            CalmParticles.SetActive(false);
            ExcitedParticles.SetActive(false);
            StressParticles.SetActive(true);
            atmTimeRemaining = strTimeRemaining;
            atmTimerIsRunning = true;  
        }
        else{
            Debug.Log("Not enough charge!");
            StartCoroutine(NotEnoughCharge());
        }
    }

    public void playFocus(){
        if(focTimeRemaining > 0){
            gameObject.GetComponent<LevelManager>().SetAtmosInActive();
            StopAtmTimer();
            Debug.Log("Focus Time remaining: " + focTimeRemaining);
            source.clip = focus;
            Character.boostedState = CharacterChangeManager.State.Focus;
            source.Play();
            Fog.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 150);
            AtmosphereTitleText.text = "Focused";
            AtmosphereTitle.GetComponent<UnityEngine.UI.Text>().color= new Color32(255, 255, 255, 150);
            CalmParticles.SetActive(false);
            ExcitedParticles.SetActive(false);
            StressParticles.SetActive(false);
            atmTimeRemaining = focTimeRemaining;
            atmTimerIsRunning = true;  
        }
        else{
            Debug.Log("Not enough charge!");
            StartCoroutine(NotEnoughCharge());
        }
    }

    IEnumerator NotEnoughCharge(){
        notEnoughPopup.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        notEnoughPopup.SetActive(false);
    }
    public void StopAtmTimer(){
        atmTimerIsRunning = false;
        UpdateAtmosTimers();
        //playBase();
    }

    public void UpdateAtmosTimers(){
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
    }
    public void PauseTimer(){
        atmTimerIsRunning = false;
    }

    public void ContinueTimer(){
        atmTimerIsRunning = true;
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
        //int amount = chargePopUp.GetComponent<ChargingAtmos>().Answers.Length;
        int amount = chargePopUp.GetComponent<ChargingAtmos>().currPos;
        Debug.Log(amount);
         switch(currentChargeState){
            case ChargeState.Calm:
                calTimeRemaining += (float) chargeTime * amount;
                break;
            case ChargeState.Focus:
                focTimeRemaining += (float) chargeTime * amount;
                break;
            case ChargeState.Excited:
                excTimeRemaining += (float) chargeTime * amount;
                break;
            case ChargeState.Stress:
                strTimeRemaining += (float) chargeTime * amount;
                break;
        }
    }
}
