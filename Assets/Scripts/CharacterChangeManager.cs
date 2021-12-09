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
    private List<string> listStreams = new List<string>() {};
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
    public enum State {Baseline, Calm, Stress, Excited, Focus}
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

    public Slider Focus;
    public Slider Excited;
    public Slider Relax;
    public Slider Stress;

    // Sprites
    private Animator anim;
    public AnimatorOverrideController CalmAnim;
    public AnimatorOverrideController ExcitedAnim;
    public AnimatorOverrideController BaselineAnim;

    private System.Random rand;

    void Start(){
        HeartRatePlugin.Event += OnHeartRateEvent; // HeartRate

        StartCoroutine(StartScanning());

        anim = Player.GetComponent<Animator>();
        rand = new System.Random();
        collectedEmotions = new int[5]; // 3~4 emotions + wildcard : Excitement, Stress, Relaxation, Focus

        StartCoroutine(MeasureEmotion(emotions));
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

        if (currentState == State.Calm){
            anim.runtimeAnimatorController = CalmAnim as RuntimeAnimatorController;
        } else if (currentState == State.Excited){
            anim.runtimeAnimatorController = ExcitedAnim as RuntimeAnimatorController;
        } else {
            anim.runtimeAnimatorController = BaselineAnim as RuntimeAnimatorController;
        }
    }

    // Edit value multiplier here
    private double ModifyValue(double val){
   
        // val = val + 0.2;
        return val;
    }


    IEnumerator MeasureEmotion(double[] emotionStream){
        while (true){
            yield return new WaitForSeconds(5.0f);


            //heartBeat = (float) (50 + rand.NextDouble()* 50);
            if (SensorId != null)
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
            }
            float boostHeartBeat = heartBeat;
            Debug.Log(heartBeat);
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

            switch (boostedState){
                case State.Calm:
                    //currentMaxEmotion.text = "Relaxed";
                    //currentMaxEmotion.text.color = new Color32(52, 161, 207, 255);
                    if(heartBeat > averageHeartBeat - 25){
                        boostHeartBeat = heartBeat - modifier;
                    }
                    break;
                case State.Excited:
                    //currentMaxEmotion.text = "Excited";
                    //currentMaxEmotion.text.color = new Color32(255, 245, 102, 255);
                    if(heartBeat > averageHeartBeat + 15){
                        boostHeartBeat = heartBeat - modifier;
                    }else if(heartBeat < averageHeartBeat + 5){
                        boostHeartBeat = heartBeat + modifier;
                    }
                    break;
                case State.Focus:
                    //currentMaxEmotion.text = "Focused";
                    //currentMaxEmotion.text.color = new Color32(255, 255, 255, 150); 
                    if(heartBeat > averageHeartBeat - 5){
                        boostHeartBeat = heartBeat - modifier;
                    }else if(heartBeat < averageHeartBeat - 15){
                        boostHeartBeat = heartBeat + modifier;
                    }
                    break;
                case State.Stress:
                    //currentMaxEmotion.text = "Stressed";
                    //currentMaxEmotion.text.color = new Color32(255, 0, 0, 255);
                    if(heartBeat < averageHeartBeat + 25){
                        boostHeartBeat = heartBeat + modifier;
                    }
                    break;
                default:
                    break;
            }

            Debug.Log("Boost:" + boostHeartBeat);
            Debug.Log("Normal:"+ heartBeat);
            int focusRef = averageHeartBeat - 10;
            int calmRef = averageHeartBeat - 25;
            int excitedRef = averageHeartBeat + 10;
            int stressRef = averageHeartBeat + 25;
            Focus.value = (float) (100 - Mathf.Abs(boostHeartBeat - focusRef)/50 * 100); 
            Excited.value = (float) (100 - Mathf.Abs(boostHeartBeat - excitedRef)/50 * 100);
            Relax.value = (float) (100 - Mathf.Abs(boostHeartBeat - calmRef)/50 * 100);
            Stress.value = (float) (100 - Mathf.Abs(boostHeartBeat - stressRef)/50 * 100);
        }
    }


    // New Game Design

    public IEnumerator CalculateEmotion(){
        // Todo: Logic for deciding emotion
        // currently just mathematical max of all values
        while (true){ 
            yield return new WaitForSeconds(5.0f); // do this every 10 seconds
            
            
            double maxVal = Math.Max(Relax.value, Math.Max(Focus.value, Math.Max(Stress.value, Excited.value)));

            if (maxVal == 0){
                yield return 0;
            }

            if (maxVal == Focus.value){
                currentState = State.Focus;
                collectedEmotions[0] = AddMax(collectedEmotions[0]);
            } else if (maxVal == Excited.value){
                currentState = State.Excited;
                collectedEmotions[1] = AddMax(collectedEmotions[1]);
            } else if (maxVal == Stress.value){
                currentState = State.Stress;
                collectedEmotions[2] = AddMax(collectedEmotions[2]);
            } else if (maxVal == Relax.value){
                currentState = State.Calm;
                collectedEmotions[3] = AddMax(collectedEmotions[3]);
            }
        }
        
    }

    public int AddMax(int val){
        int maxVal = 3;

        if (val < maxVal){
            return val+1;
        } else{
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
}

