using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperSimpleExample : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField]
    private HeartRatePlugin heartRatePlugin;
    #pragma warning restore 0649

    // Start is called before the first frame update
    void Start()
    {
        // attach event
        HeartRatePlugin.Event += OnHeartRateEvent;
        
        StartCoroutine(StartScanning());
    }

    IEnumerator StartScanning()
    {
        // wait for HeartRatePlugin to be initialized
        yield return new WaitUntil(() => heartRatePlugin.IsInitialized);
        heartRatePlugin.StartScan();
        // Don't forget to stop scanning, if you don't need it anymore
    }

    void OnHeartRateEvent(object sender, HeartRatePlugin.EventArgs e)
    {
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
