using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
//using Math;
//using LSL;

// Don't forget the Namespace import
// using Assets.LSL4Unity.Scripts.AbstractInlets;

public class CharacterChangeManager : MonoBehaviour
{
    [SerializeField]
    private HeartRatePlugin heartRatePlugin;
    private String SensorId;
    // Related to reading data from LSL

    // private float _waitTime = 2.0f; // 2 seconds
    // private float timer = 0.0f; // timer to query streams
    private List<string> listStreams = new List<string>() { };
    // private  bool _inletCreated = false;


    // controls

    // private double prevExcitement = 0f;
    // private double prevCalm = 0f;
    // private double prevStress = 0f;
    // private double prevFocus = 0f;

    // private double currExcitement = 0f;
    // private double currCalm = 0f;
    // private double currStress = 0f;
    // private double currFocus = 0f;
    private float heartBeat = 0f;
    private int modifier = 10;

    // Modes
    public enum State { Baseline, Calm, Stress, Excited, Focus, HeartRate }
    //public enum HeartRateBar { HeartRate }
    public State currentState = State.Baseline;
    public State boostedState = State.Baseline;

    // Data readin
    public GameObject Player;

    private double[] emotions;

    //collect emotions 

    public int[] collectedEmotions;
    public int averageHeartBeat = 85;


    // UI
    public Text currentMaxEmotion;

    public Slider HeartRate;
    public Slider Focus;
    public Slider Excited;
    public Slider Relax;
    public Slider Stress;

    public float focusValue;
    public float excitedValue;
    public float relaxValue;
    public float stressValue;

    // Callibration
    private bool calibrating = false;
    public Text CurrentHeartValue;
    public Text StatusText;
    public GameObject CalibrationScreen;
    public GameObject ReturnButton;

    
    public GameObject ForceChange;

    // Sprites
    private Animator anim;
    public AnimatorOverrideController CalmAnim;
    public AnimatorOverrideController ExcitedAnim;
    public AnimatorOverrideController BaselineAnim;

    private System.Random rand;

    // Couroutines

    private IEnumerator measure;

    void Start(){
        HeartRatePlugin.Event += OnHeartRateEvent; // HeartRate

        //StartCoroutine(StartScanning());

        anim = Player.GetComponent<Animator>();
        rand = new System.Random();
        collectedEmotions = new int[5]; // 3~4 emotions + wildcard : Excitement, Stress, Relaxation, Focus, Bonus

        measure = MeasureEmotion();
        StartCoroutine(measure);
        StartCoroutine(CalculateEmotion());
    }

    IEnumerator StartScanning()
    {
        // wait for HeartRatePlugin to be initialized
        yield return new WaitUntil(() => heartRatePlugin.IsInitialized);
        heartRatePlugin.StartScan();

        // Don't forget to stop scanning, if you don't need it anymore
    }

    // Update is called once per frame
    void Update()
    {

        if (!(ForceChange.GetComponent<ForceChange>().forceActive))
        {
            if (currentState == State.Calm)
            {
                anim.runtimeAnimatorController = CalmAnim as RuntimeAnimatorController;
            }
            else if (currentState == State.Excited)
            {
                anim.runtimeAnimatorController = ExcitedAnim as RuntimeAnimatorController;
            }
            else
            {
                anim.runtimeAnimatorController = BaselineAnim as RuntimeAnimatorController;
            }
        } else
        {
            if (ForceChange.GetComponent<ForceChange>().timerIsRunning)
            {
                if (ForceChange.GetComponent<ForceChange>().timeRemaining > 0)
                {
                    ForceChange.GetComponent<ForceChange>().timeRemaining -= Time.deltaTime;
                    //Debug.Log("Time: " + ForceChange.GetComponent<ForceChange>().timeRemaining);
                }
                else
                {
                    Debug.Log("Time has run out!");
                    ForceChange.GetComponent<ForceChange>().timeRemaining = 30;
                    ForceChange.GetComponent<ForceChange>().timerIsRunning = false;
                    ForceChange.GetComponent<ForceChange>().forceActive = false;
                }
            }
        }


        if (Input.GetKeyDown(KeyCode.C) && !calibrating){
            startCalibration();
        }
    }

    // Edit value multiplier here
    private double ModifyValue(double val)
    {

        // val = val + 0.2;
        return val;
    }

    public void startCalibration(){
        calibrating = true;
        CalibrationScreen.SetActive(true);
        ReturnButton.SetActive(false);

        StopCoroutine(measure);

        // This part isn't working for some reason
        Player.GetComponent<PlayerController>().enabled = false;

        StartCoroutine(CalibrateHeart());
    }

    public void endCalibration(){
        calibrating = false;
        CalibrationScreen.SetActive(false);

        StartCoroutine(measure);

        Player.GetComponent<PlayerController>().enabled = true;
    }

    IEnumerator CalibrateHeart(){
        StatusText.text = "Measuring your heart rate...";
        float currentRead = 0;
        float allReads = 0;
        for (int i = 0; i< 10; i++){
            yield return new WaitForSeconds(1.0f);
            currentRead = (float)HeartRateSensor.Sensors[SensorId].PulseRate;
            allReads += currentRead;
            CurrentHeartValue.text = currentRead.ToString();
        }

        averageHeartBeat = (int) allReads/10;
        CurrentHeartValue.text = averageHeartBeat.ToString();
        StatusText.text = "Callibration Complete!";

        ReturnButton.SetActive(true);
    }

    IEnumerator MeasureEmotion()
    {
        while (true)
        {
            yield return new WaitForSeconds(3.0f);


            heartBeat = (float)(50 + rand.NextDouble() * 50); // For testing without the device
            //heartBeat = (float) 50;
            // Reading data from the device
            /*if (SensorId != null)
            {
                if (HeartRateSensor.Sensors[SensorId] != null)
                {
                    heartBeat = (float)HeartRateSensor.Sensors[SensorId].PulseRate;
                }
            }
            else
            {
                heartBeat = 0;
                Debug.Log("NO SIGNAL");
            }*/


            float boostHeartBeat = heartBeat;
            //Debug.Log(heartBeat);
            //emotionStream = new double[7] {rand.NextDouble(), rand.NextDouble(), rand.NextDouble(), rand.NextDouble(), rand.NextDouble(), rand.NextDouble(),rand.NextDouble()};

            // prevCalm = currCalm;
            // prevStress = currStress;
            // prevExcitement = currExcitement;
            // prevFocus = currFocus;

            // TODO: check if order matters
            // currExcitement = emotionStream[2];
            // currFocus = emotionStream[3];
            // currCalm = emotionStream[5];
            // currStress = emotionStream[6];

            switch (boostedState)
            {
                case State.Calm:
                    //currentMaxEmotion.text = "Relaxed";
                    //currentMaxEmotion.text.color = new Color32(52, 161, 207, 255);
                    if (heartBeat > averageHeartBeat - 25)
                    {
                        boostHeartBeat = heartBeat - modifier;
                    }
                    break;
                case State.Excited:
                    //currentMaxEmotion.text = "Excited";
                    //currentMaxEmotion.text.color = new Color32(255, 245, 102, 255);
                    if (heartBeat > averageHeartBeat + 15)
                    {
                        boostHeartBeat = heartBeat - modifier;
                    }
                    else if (heartBeat < averageHeartBeat + 5)
                    {
                        boostHeartBeat = heartBeat + modifier;
                    }
                    break;
                case State.Focus:
                    //currentMaxEmotion.text = "Focused";
                    //currentMaxEmotion.text.color = new Color32(255, 255, 255, 150); 
                    if (heartBeat > averageHeartBeat - 5)
                    {
                        boostHeartBeat = heartBeat - modifier;
                    }
                    else if (heartBeat < averageHeartBeat - 15)
                    {
                        boostHeartBeat = heartBeat + modifier;
                    }
                    break;
                case State.Stress:
                    //currentMaxEmotion.text = "Stressed";
                    //currentMaxEmotion.text.color = new Color32(255, 0, 0, 255);
                    if (heartBeat < averageHeartBeat + 25)
                    {
                        boostHeartBeat = heartBeat + modifier;
                    }
                    break;
                default:
                    break;
            }



            //Debug.Log("Boost:" + boostHeartBeat);
           // Debug.Log("Normal:" + heartBeat);
            int focusRef = averageHeartBeat - 10;
            int calmRef = averageHeartBeat - 25;
            int excitedRef = averageHeartBeat + 10;
            int stressRef = averageHeartBeat + 25;
            focusValue = (float)(100 - Mathf.Abs(boostHeartBeat - focusRef) / 50 * 100);
            excitedValue = (float)(100 - Mathf.Abs(boostHeartBeat - excitedRef) / 50 * 100);
            relaxValue = (float)(100 - Mathf.Abs(boostHeartBeat - calmRef) / 50 * 100);
            stressValue = (float)(100 - Mathf.Abs(boostHeartBeat - stressRef) / 50 * 100);

            //Focus.value = (float) (100 - Mathf.Abs(boostHeartBeat - focusRef)/50 * 100); 
            //Excited.value = (float) (100 - Mathf.Abs(boostHeartBeat - excitedRef)/50 * 100);
            //Relax.value = (float) (100 - Mathf.Abs(boostHeartBeat - calmRef)/50 * 100);
            //Stress.value = (float) (100 - Mathf.Abs(boostHeartBeat - stressRef)/50 * 100);
            HeartRate.value = (float)(boostHeartBeat);

            HeartRate.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = ColorChange(boostHeartBeat);
            //GameObject.Find("Wildcard (new)").transform.Find("Counter").Find("Text").GetComponent<Text>().text = (Level.GetComponent<CharacterChangeManager>().collectedEmotions[4] - 1).ToString();

            //HeartRate.gameObject.transform.Find("Handle Slide Area").value = boostHeartBeat;
            //HeartRate.gameObject.transform.Find("Handle Slide Area").Find("Handle").position = boostHeartBeat;

        }
    }

    // For the HeartRate bar
    private Color ColorChange(float heartBeat)
    {
        Color color;
        if (heartBeat <= 60)
        {
            color = Color.blue;
        }
        else if (heartBeat > 60 && heartBeat <= 70)
        {
            color = Color.grey;
        }
        else if (heartBeat > 70 && heartBeat <= 80)
        {
            color = Color.white;
        }
        else if (heartBeat > 80 && heartBeat < 90)
        {
            color = Color.yellow;
        }
        else
        {
            color = Color.red;
        }
        return color;
    }


    // New Game Design

    public IEnumerator CalculateEmotion()
    {
        // Todo: Logic for deciding emotion
        // currently just mathematical max of all values
        while (true)
        {
            yield return new WaitForSeconds(3.0f); // do this every 5 seconds


            double maxVal = Math.Max(relaxValue, Math.Max(focusValue, Math.Max(stressValue, excitedValue)));

            if (maxVal == 0)
            {
                yield return 0;
            }

            if (maxVal == focusValue)
            {
                Debug.Log("Focus");
                currentState = State.Focus;
                if (Focus.value < 10)
                {
                    Focus.value += 1;
                }
                else if (Focus.value == 10)
                {
                    collectedEmotions[0] = AddMax(collectedEmotions[0]);
                    Focus.value = 0;
                }
            }
            else if (maxVal == excitedValue)
            {
                currentState = State.Excited;
                if (Excited.value < 10)
                {
                    Excited.value += 1;
                }
                else if (Excited.value == 10)
                {
                    collectedEmotions[1] = AddMax(collectedEmotions[1]);
                    Excited.value = 0;
                }
            }
            else if (maxVal == stressValue)
            {
                currentState = State.Stress;
                if (Stress.value < 10)
                {
                    Stress.value += 1;
                }
                else if (Stress.value == 10)
                {
                    collectedEmotions[2] = AddMax(collectedEmotions[2]);
                    Stress.value = 0;
                }

            }
            else if (maxVal == relaxValue)
            {
                currentState = State.Calm;
                if (Relax.value < 10)
                {
                    Relax.value += 1;
                }
                else if (Relax.value == 10)
                {
                    collectedEmotions[3] = AddMax(collectedEmotions[3]);
                    Relax.value = 0;
                }
            }
        }


    }


    public int AddMax(int val)
    {
        int maxVal = 3;

        if (val < maxVal)
        {
            return val + 1;
        }
        else
        {
            return maxVal;
        }
    }

    void OnHeartRateEvent(object sender, HeartRatePlugin.EventArgs e)
    {
        SensorId = e.MacId;

        switch (e.Type)
        {
            case HeartRatePlugin.EventType.SYSTEM_SCANNING:
                Debug.Log("OnHeartRateEvent, SYSTEM_SCANNING");
                break;
            case HeartRatePlugin.EventType.SYSTEM_NOT_SCANNING:
                Debug.Log("OnHeartRateEvent, SYSTEM_NOT_SCANNING");
                break;
            case HeartRatePlugin.EventType.NEW_SENSOR:
                // a new sensor was found
                // connect to the first found sensor
                Debug.Log($"OnHeartRateEvent, NEW_SENSOR with MacId: {e.MacId}, Name: {HeartRateSensor.Sensors[e.MacId].Name}, Rssi: {HeartRateSensor.Sensors[e.MacId].Rssi}, connecting...");
                heartRatePlugin.Connect(e.MacId);
                break;
            case HeartRatePlugin.EventType.REMOVE_UNCONNECTED:
                // at scanstart all unconnected sensors are removed from MovesenseDevice-list
                Debug.Log("OnHeartRateEvent, REMOVE_UNCONNECTED");
                break;
            case HeartRatePlugin.EventType.CONNECTING:
                // a sensor is connecting
                Debug.Log($"OnHeartRateEvent, CONNECTING {e.MacId}");
                break;
            case HeartRatePlugin.EventType.CONNECTED:
                Debug.Log($"OnHeartRateEvent, CONNECTED {e.MacId}");
                break;
            case HeartRatePlugin.EventType.DISCONNECTED:
                Debug.Log($"OnHeartRateEvent, DISCONNECTED {e.MacId}");
                break;
            case HeartRatePlugin.EventType.NOTIFICATION_CONTROLPOINT:
                Debug.Log($"OnHeartRateEvent, NOTIFICATION_CONTROLPOINT for {e.MacId}, {HeartRateSensor.Sensors[e.MacId].HR_ControlPoint}");
                break;
            case HeartRatePlugin.EventType.NOTIFICATION_BODYSENSORLOCATION:
                Debug.Log($"OnHeartRateEvent, NOTIFICATION_BODYSENSORLOCATION for {e.MacId}, {HeartRateSensor.Sensors[e.MacId].SensorLocation}");
                break;
            case HeartRatePlugin.EventType.NOTIFICATION_MEASUREMENT:
                Debug.Log($"OnHeartRateEvent, NOTIFICATION_MEASUREMENT for {e.MacId} with value: {HeartRateSensor.Sensors[e.MacId].PulseRate}");
                break;
        }
    }

    public void forcedChange()
    {

    }
}

