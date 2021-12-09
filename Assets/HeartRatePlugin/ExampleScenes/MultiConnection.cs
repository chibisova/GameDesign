using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MultiConnection : MonoBehaviour {
	private const string TAG = "MultiConnection; ";
	private const bool isLogging = false;

	#pragma warning disable 0649
    [SerializeField]
	private HeartRatePlugin heartRatePlugin;

	[SerializeField]
	private GameObject SensorElementPrefab;
    
	[SerializeField]
	private TMP_Text buttonText;

	[SerializeField]
	private RectTransform scrollViewContent;

	[SerializeField]
	private TMP_Text statusText;
	#pragma warning restore 0649

    private class SensorElement {
		public GameObject Element;
		public Visualize Visualize;

		public SensorElement(GameObject element, Visualize visualize) {
			Element = element;
			Visualize = visualize;
		}
	}
	private List<SensorElement> sensorElements = new List<SensorElement>();
	private float ElementHeight = 0;

	
	void Start () {
		#pragma warning disable 0162
		if (isLogging) Debug.Log(TAG + "Start");
		#pragma warning restore 0162

		// attach events
		HeartRatePlugin.Event += OnHeartRateEvent;

		buttonText.text = "start scan";
	}

	void OnDestroy() {
		#pragma warning disable 0162
		if (isLogging) Debug.Log(TAG + "OnDestroy");
		#pragma warning restore 0162

		// detach events
		HeartRatePlugin.Event -= OnHeartRateEvent;
	}


	public void OnClickButtonControl() {
		#pragma warning disable 0162
		if (isLogging) Debug.Log(TAG + "OnClickButtonControl");
		#pragma warning restore 0162

		if (heartRatePlugin.IsScanning) {
			heartRatePlugin.StopScan();
		} else {
			heartRatePlugin.StartScan();
		}
	}

	public void OnClickButtonConnect(string macId) {
		bool isConnecting = HeartRateSensor.Sensors[macId].IsConnecting;
		bool isConnected = HeartRateSensor.Sensors[macId].IsConnected;
		
		// attached method for ElementClone
		#pragma warning disable 0162
		if (isLogging) Debug.Log(TAG + "OnClickButtonConnect, " + ((isConnecting || isConnected) ? "disconnecting: " : "connecting: ") + macId);
		#pragma warning restore 0162

		if (heartRatePlugin.IsInitialized) {
			if (isConnecting || isConnected) {
				heartRatePlugin.Disconnect(macId, false);
			} else {
				heartRatePlugin.Connect(macId);
			}
		} else {
			Debug.LogError(TAG + "OnClickButtonConnect, MovesenseController is NOT initialized. Did you forget to add MovesenseController object in the scene?");
		}
	}

	void OnHeartRateEvent(object sender, HeartRatePlugin.EventArgs e) {
		switch (e.Type) {
			case HeartRatePlugin.EventType.SYSTEM_SCANNING:
				buttonText.text = "Stop scanning";
				RefreshScrollViewContent(e.MacId);
				statusText.text = e.Info;
			break;
			case HeartRatePlugin.EventType.SYSTEM_NOT_SCANNING:
				buttonText.text = "Start scanning";
				RefreshScrollViewContent(e.MacId);
				statusText.text = e.Info;
			break;
			case HeartRatePlugin.EventType.NEW_SENSOR:			// a new sensor was found
			case HeartRatePlugin.EventType.REMOVE_UNCONNECTED:	// at scanstart all unconnected sensors are removed from MovesenseDevice-list
			case HeartRatePlugin.EventType.CONNECTING:
			case HeartRatePlugin.EventType.CONNECTED:
			case HeartRatePlugin.EventType.DISCONNECTED:
            case HeartRatePlugin.EventType.NOTIFICATION_CONTROLPOINT:
            case HeartRatePlugin.EventType.NOTIFICATION_BODYSENSORLOCATION:
			case HeartRatePlugin.EventType.NOTIFICATION_MEASUREMENT:
				RefreshScrollViewContent(e.MacId);
				statusText.text = e.Info;
			break;
		}
	}

	void RefreshScrollViewContent(string MacId) {
		#pragma warning disable 0162
		if (isLogging) Debug.Log(TAG + "RefreshScrollViewContent");
		#pragma warning restore 0162

		int scannedDevices = HeartRateSensor.Sensors.Count;
			
		#pragma warning disable 0162
		if (isLogging) Debug.Log(TAG + "scannedDevices: " + scannedDevices);
		#pragma warning restore 0162
		int sensorElementsCount = sensorElements.Count;
		#pragma warning disable 0162
		if (isLogging) Debug.Log(TAG + "scanElementsCount: " + sensorElementsCount);
		#pragma warning restore 0162

		if (sensorElementsCount < scannedDevices) {
			for (int i = sensorElementsCount; i < scannedDevices; i++) {
				#pragma warning disable 0162
				if (isLogging) Debug.Log(TAG + "RefreshScrollViewContent, add clone " + i);
				#pragma warning restore 0162

				GameObject sensorElementClone = Instantiate(SensorElementPrefab, scrollViewContent) as GameObject;
				Visualize visualizeScript = sensorElementClone.GetComponentInChildren<Visualize>();
				
				// Positioning
				RectTransform sensorElementRect = sensorElementClone.GetComponent<RectTransform>();
				if (ElementHeight == 0) ElementHeight = sensorElementRect.sizeDelta.y;
				sensorElementRect.anchoredPosition = new Vector2(0, -ElementHeight/2 - (i * ElementHeight));
				
				// Set texts
				TMP_Text[] sensorElementSensorTexts = sensorElementClone.GetComponentsInChildren<TMP_Text>();
				foreach (var text in sensorElementSensorTexts) {
					if (text.name == "Text Status") {
						if (HeartRateSensor.Sensors[i].IsConnecting) {
							text.text = "connecting";	
						} else {
							text.text = HeartRateSensor.Sensors[i].IsConnected ? "Sensor connected, press to disconnect" : "not connected, press to connected";
						}
					} else if (text.name == "Text Name") {
						text.text = HeartRateSensor.Sensors[i].Name;
					} else if (text.name == "Text MacId") {
						text.text = HeartRateSensor.Sensors[i].MacId;
					} else if (text.name == "Text Rssi") {
						text.text = HeartRateSensor.Sensors[i].Rssi.ToString() + "db";
					}
				}

				// change OnClickButtonConnect-methodparameters
				Button sensorElementButton = sensorElementClone.GetComponentInChildren<Button>();

				sensorElementButton.onClick.RemoveAllListeners();

				System.Func<string, UnityEngine.Events.UnityAction> actionBuilder = (macId) => () => OnClickButtonConnect(macId);
				UnityEngine.Events.UnityAction action1 = actionBuilder(HeartRateSensor.Sensors[i].MacId);
				sensorElementButton.onClick.AddListener(action1);

				// add Clone to scanElements-list
				SensorElement sensorElement = new SensorElement(sensorElementClone, visualizeScript);
				sensorElements.Add(sensorElement);
			}

			// set contentsize
			scrollViewContent.sizeDelta = new Vector2(0, ElementHeight * scannedDevices);
		} else if (sensorElementsCount > scannedDevices) {
			for (int i = sensorElementsCount-1; i > scannedDevices-1; i--) {
				#pragma warning disable 0162
				if (isLogging) Debug.Log(TAG + "RefreshScrollViewContent, destroy clone " + i);
				#pragma warning restore 0162
				Destroy(sensorElements[i].Element);
				sensorElements.RemoveAt(i);
			}
			scrollViewContent.sizeDelta = new Vector2(0, ElementHeight * scannedDevices);
		}

		if (MacId == null) { // remove unconnected
			return;
		}

		// get index of key MacId
		int listIndex;
		try {
			listIndex = HeartRateSensor.Sensors.IndexOf(HeartRateSensor.Sensors[MacId]);
		} catch {
			Debug.LogWarning(TAG + "Sensor has been removed");
			return;
		}
		
		#pragma warning disable 0162
		if (isLogging) Debug.Log(TAG + "RefreshScrollViewContent, updating data for device " + listIndex);
		#pragma warning restore 0162

		HeartRateSensor sensor = HeartRateSensor.Sensors[listIndex];

		// update texts with new measurement data
		TMP_Text[] sensorElementDataTexts = sensorElements[listIndex].Element.GetComponentsInChildren<TMP_Text>();
		foreach (var text in sensorElementDataTexts) {
			if (text.name == "Text Status") {
				if (sensor.IsConnecting) {
					text.text = "connecting";	
				} else {
					text.text = sensor.IsConnected ? "Sensor connected, press to disconnect" : "not connected, press to connect";
				}
			} else if (text.name == "Text PulseRate") {
				if (!sensor.IsConnected) {
					text.text = "";	
				} else {
					text.text = sensor.PulseRate.ToString() + (sensor.PulseRate != null ? " bpm" : "");
				}
			} else if (text.name == "Text SensorLocation") {
				if (!sensor.IsConnected) {
					text.text = "";
				} else {
					text.text = ((HeartRateSensor.HRM_BodySensorLocation)sensor.SensorLocation).ToString();
				}
			} else if (text.name == "Text SensorContact") {
				if (!sensor.IsConnected) {
					text.text = "";
				} else {
					text.text = ((HeartRateSensor.HRM_SensorContactStatus)sensor.SCStatus).ToString();
				}
			} else if (text.name == "Text EnergyExpended") {
				if (!sensor.IsConnected) {
					text.text = "";
				} else {
					if (sensor.EnergyExpended == null) {
						text.text = ((HeartRateSensor.HRM_EnergyExpendedStatus)0).ToString();
					} else {
						text.text = (sensor.EnergyExpended).ToString() + " kj";
					}
				}
			} else if (text.name == "Text RR-Interval") {
				if (!sensor.IsConnected) {
					text.text = "";
				} else {
					if (sensor.RrInterval == null) {
						text.text = ((HeartRateSensor.HRM_EnergyExpendedStatus)0).ToString();
					} else {
						text.text = (sensor.RrInterval[sensor.RrInterval.Count-1]).ToString() + " ms";
					}
				}
			} else if (text.name == "Text ControlPoint") {
				if (!sensor.IsConnected || sensor.HR_ControlPoint == null) {
					text.text = "";
				} else {
					text.text = ((HeartRateSensor.HRA_ControlPointStatus)sensor.HR_ControlPoint).ToString();	
				}
			}
		}
		
		// change visualization parameter
		if (sensor.IsConnected) {
			if (sensor.PulseRate != null) {
				sensorElements[listIndex].Visualize.BPM = (uint)sensor.PulseRate;
				sensorElements[listIndex].Visualize.IsBeating = true;
			} else {
				sensorElements[listIndex].Visualize.IsBeating = false;
			}
		} else {
			sensorElements[listIndex].Visualize.IsBeating = false;
		}
	}
}
