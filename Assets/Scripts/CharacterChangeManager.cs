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

    //Boundaries relative to the average
    public int calmLowerBoundary;
    public int calmRefValue;
    public int calmUpperBoundary;
    public int focusRefValue;
    public int focusUpperBoundary;
    public int avgUpperBoundary = 4;
    public int excRefValue = 8;
    public int excUpperBoundary = 12;
    public int stressRefValue = 16;
    public int stressUpperBoundary = 20;

    public float focusValue;
    public float excitedValue;
    public float relaxValue;
    public float stressValue;
    public float baselineValue;

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
    public AnimatorOverrideController FocusAnim;
    public AnimatorOverrideController StressAnim;

    private System.Random rand;

    // Couroutines

    private IEnumerator measure;

    void Start(){
        HeartRatePlugin.Event += OnHeartRateEvent; // HeartRate

        StartCoroutine(StartScanning());

        anim = Player.GetComponent<Animator>();
        rand = new System.Random();
        collectedEmotions = new int[5]; // 3~4 emotions + wildcard : Excitement, Stress, Relaxation, Focus, Bonus

        measure = MeasureEmotion();
        StartCoroutine(measure);
        StartCoroutine(CalculateEmotion());
        calmLowerBoundary = -stressUpperBoundary;
        calmRefValue = -stressRefValue;
        calmUpperBoundary = -excUpperBoundary;
        focusRefValue = -excRefValue;
        focusUpperBoundary = -avgUpperBoundary;
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

        if (!(ForceChange.GetComponent<ForceChange>().forceActive)) // Check if not Forced
        {
            if (currentState == State.Calm) {
                anim.runtimeAnimatorController = CalmAnim as RuntimeAnimatorController;
            } else if (currentState == State.Excited) {
                anim.runtimeAnimatorController = ExcitedAnim as RuntimeAnimatorController;
            } else if (currentState == State.Stress){
                anim.runtimeAnimatorController = StressAnim as RuntimeAnimatorController;
            } else if (currentState == State.Focus){ 
                anim.runtimeAnimatorController = FocusAnim as RuntimeAnimatorController;
            } else {
                anim.runtimeAnimatorController = BaselineAnim as RuntimeAnimatorController;
            }
        } else //Start Timer
        {
            if (ForceChange.GetComponent<ForceChange>().timerIsRunning)
            {
                if (ForceChange.GetComponent<ForceChange>().timeRemaining > 0)
                {
                    ForceChange.GetComponent<ForceChange>().timeRemaining -= Time.deltaTime;
                    if (ForceChange.GetComponent<ForceChange>().forcedCalm)
                    {
                        Relax.gameObject.transform.Find("Relax Timer").Find("Image").GetComponent<Image>().fillAmount = (ForceChange.GetComponent<ForceChange>().timeRemaining / 30);

                    } else if (ForceChange.GetComponent<ForceChange>().forcedExcited)
                    {
                        Excited.gameObject.transform.Find("Excited Timer").Find("Image").GetComponent<Image>().fillAmount = (ForceChange.GetComponent<ForceChange>().timeRemaining / 30);

                    } else if (ForceChange.GetComponent<ForceChange>().forcedFocus)
                    {
                        Focus.gameObject.transform.Find("Focus Timer").Find("Image").GetComponent<Image>().fillAmount = (ForceChange.GetComponent<ForceChange>().timeRemaining / 30);

                    } else if (ForceChange.GetComponent<ForceChange>().forcedStress)
                    {
                        Stress.gameObject.transform.Find("Stress Timer").Find("Image").GetComponent<Image>().fillAmount = (ForceChange.GetComponent<ForceChange>().timeRemaining / 30);

                    }
                    //ForceChange.GetComponent<ForceChange>().fillTimer.fillAmont = ForceChange.GetComponent<ForceChange>().timeRemaining / ForceChange.GetComponent<ForceChange>().timeRemaining;
                    //Debug.Log("Time: " + ForceChange.GetComponent<ForceChange>().timeRemaining);
                }
                else
                {
                    Debug.Log("Time has run out!");
                    ForceChange.GetComponent<ForceChange>().timeRemaining = 30;
                    ForceChange.GetComponent<ForceChange>().timerIsRunning = false;
                    ForceChange.GetComponent<ForceChange>().forceActive = false;
                    ForceChange.GetComponent<ForceChange>().forcedCalm = false;
                    ForceChange.GetComponent<ForceChange>().forcedExcited = false;
                    ForceChange.GetComponent<ForceChange>().forcedStress = false;
                    ForceChange.GetComponent<ForceChange>().forcedFocus = false;

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


            //heartBeat = (float)(averageHeartBeat - stressUpperBoundary + rand.NextDouble() * (2*stressUpperBoundary)); // For testing without the device
            //heartBeat = (float)90;
            //Reading data from the device
             if (SensorId != null)
            {
                if (HeartRateSensor.Sensors[SensorId] != null)
                {
                    Debug.Log("Error: " + HeartRateSensor.Sensors[SensorId]);

                    heartBeat = (float)HeartRateSensor.Sensors[SensorId].PulseRate;
                }
            }
            else
            {
                heartBeat = 0;
                Debug.Log("NO SIGNAL");
            }


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
                    if (heartBeat > averageHeartBeat - stressUpperBoundary)
                    {
                        boostHeartBeat = heartBeat - modifier;
                    }
                    break;
                case State.Excited:
                    //currentMaxEmotion.text = "Excited";
                    //currentMaxEmotion.text.color = new Color32(255, 245, 102, 255);
                    if (heartBeat > averageHeartBeat + excUpperBoundary)
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
                    if (heartBeat > averageHeartBeat - avgUpperBoundary)
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
                    if (heartBeat < averageHeartBeat + stressUpperBoundary)
                    {
                        boostHeartBeat = heartBeat + modifier;
                    }
                    break;
                default:
                    break;
            }



            //Debug.Log("Boost:" + boostHeartBeat);
           // Debug.Log("Normal:" + heartBeat);
            int focusRef = averageHeartBeat - avgUpperBoundary;
            int calmRef = averageHeartBeat - stressUpperBoundary;
            int excitedRef = averageHeartBeat + avgUpperBoundary;
            int stressRef = averageHeartBeat + stressUpperBoundary;
            focusValue = (float)(100 - Mathf.Abs(boostHeartBeat - focusRef));
            excitedValue = (float)(100 - Mathf.Abs(boostHeartBeat - excitedRef));
            relaxValue = (float)(100 - Mathf.Abs(boostHeartBeat - calmRef));
            stressValue = (float)(100 - Mathf.Abs(boostHeartBeat - stressRef));
            baselineValue = (float)(100 -  Mathf.Abs(boostHeartBeat - averageHeartBeat));

            HeartRate.value = (float)(boostHeartBeat);

            HeartRate.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = ColorChange(boostHeartBeat);
            HeartRate.gameObject.transform.GetComponent<Slider>().minValue = averageHeartBeat - stressUpperBoundary;
            HeartRate.gameObject.transform.GetComponent<Slider>().maxValue = averageHeartBeat + stressUpperBoundary;


            //GameObject.Find("Wildcard (new)").transform.Find("Counter").Find("Text").GetComponent<Text>().text = (collectedEmotions[4]).ToString();

            //HeartRate.gameObject.transform.Find("Handle Slide Area").value = boostHeartBeat;
            //HeartRate.gameObject.transform.Find("Handle Slide Area").Find("Handle").position = boostHeartBeat;

        }
    }

    // For the HeartRate bar
    private Color ColorChange(float heartBeat)
    {
        Color color;
        if (heartBeat <= averageHeartBeat - excUpperBoundary)
        {
            color = Color.blue;
        }
        else if ((heartBeat > averageHeartBeat - excUpperBoundary) && (heartBeat <= averageHeartBeat - avgUpperBoundary))
        {
            color = Color.grey;
        }
        else if ((heartBeat > averageHeartBeat - avgUpperBoundary) && (heartBeat <= averageHeartBeat + avgUpperBoundary))
        {
            color = Color.white;
        }
        else if ((heartBeat > averageHeartBeat + avgUpperBoundary) && (heartBeat <= averageHeartBeat + excUpperBoundary))
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


            double maxVal = Math.Max(relaxValue, Math.Max(focusValue, Math.Max(baselineValue, Math.Max(stressValue, excitedValue))));

            if (maxVal == 0)
            {
                yield return 0;
            }
            if (ForceChange.GetComponent<ForceChange>().forceActive)
            {
                if ((ForceChange.GetComponent<ForceChange>().forcedExcited)){
                    currentState = State.Excited;
                }
                else if (ForceChange.GetComponent<ForceChange>().forcedCalm){
                    currentState = State.Calm;
                } else if (ForceChange.GetComponent<ForceChange>().forcedStress){
                    currentState = State.Stress;
                } else if (ForceChange.GetComponent<ForceChange>().forcedFocus){
                    currentState = State.Focus;
                }
                if (maxVal == focusValue)
                {
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
            else { 
                if (maxVal == focusValue)
                {
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
                    Debug.Log("Im excited: " + excitedValue);
                    Debug.Log("Baseline: " + baselineValue);


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
                else if (maxVal == baselineValue)
                {
                    Debug.Log("Im here: " + baselineValue);
                    currentState = State.Baseline;
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

