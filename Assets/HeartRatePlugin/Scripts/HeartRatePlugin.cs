using System;
using UnityEngine;
#if (UNITY_STANDALONE_OSX || UNITY_EDITOR) || (UNITY_IOS && !UNITY_EDITOR)
using System.Runtime.InteropServices;
#endif
#if PLATFORM_ANDROID && UNITY_2018_3_OR_NEWER
using UnityEngine.Android;
#endif
using System.Collections;

public enum BleHardwareState {
	UNKNOWN			= 0,
	RESETTING		= 1,
	UNSUPPORTED		= 2,
	UNAUTHORIZED	= 3,
	POWERED_OFF		= 4,
	POWERED_ON		= 5,
	LOCATION_OFF	= 6,
	LOCATION_ON		= 7
}

public class HeartRatePlugin : MonoBehaviour {
	private const string TAG = "HeartRatePlugin; ";
	private const bool isLogging = false;
	public enum EventType {
		SYSTEM_NOT_SCANNING,
		SYSTEM_SCANNING,
        NEW_SENSOR,
        REMOVE_UNCONNECTED,
		CONNECTING,
        CONNECTED,
        DISCONNECTED,
        NOTIFICATION_CONTROLPOINT, // 2A39
        NOTIFICATION_BODYSENSORLOCATION, // 2A38
		NOTIFICATION_MEASUREMENT // 2A37
    }

	#region Plugin import
		#if UNITY_ANDROID && !UNITY_EDITOR
			private static AndroidJavaObject AndroidHeartRatePlugin;
		#elif UNITY_IOS && !UNITY_EDITOR	
			[DllImport ("__Internal")]
			private static extern void Initplugin_iOS(bool shouldLog);

			[DllImport ("__Internal")]
			private static extern void Dispose_iOS();
			
			[DllImport ("__Internal")]
			private static extern int GetBLEStatus_iOS(); // see iOS-Ble-Hardware-States

			[DllImport ("__Internal")]
			private static extern void EnableBluetooth_iOS();

			[DllImport ("__Internal")]
			private static extern void StopScan_iOS();

			[DllImport ("__Internal")]
			private static extern void Scan_iOS();
			
			[DllImport ("__Internal")]
			private static extern void Disconnect_iOS(string Identifier);

			[DllImport ("__Internal")]
			private static extern void DisconnectAll_iOS();

			[DllImport ("__Internal")]
			private static extern void Connect_iOS(string Identifier);

			[DllImport ("__Internal")]
			private static extern void HeartRateCommand_iOS(string Identifier, byte[] bytes);
		#elif UNITY_STANDALONE_OSX || UNITY_EDITOR
			[UnmanagedFunctionPointer(CallingConvention.StdCall)]
			public delegate void AsyncCallback(string callbackString);

			[DllImport ("MacOSHeartRatePlugin")]
			private static extern void Initplugin_MacOS(bool Log,
			AsyncCallback ReportActualBleState,
			AsyncCallback ReportNewSensor,
			AsyncCallback ReportConnection,
			AsyncCallback NotificationCallback);

			[DllImport ("MacOSHeartRatePlugin")]
			private static extern int Dispose_MacOS();
			
			[DllImport ("MacOSHeartRatePlugin")]
			private static extern int GetBLEStatus_MacOS();
			[DllImport ("MacOSHeartRatePlugin")]
			private static extern void StopScan_MacOS();
			[DllImport ("MacOSHeartRatePlugin")]
			private static extern void Scan_MacOS();
			[DllImport ("MacOSHeartRatePlugin")]
			private static extern void Disconnect_MacOS(string Identifier);

			[DllImport ("MacOSHeartRatePlugin")]
			private static extern void DisconnectAll_MacOS();
			[DllImport ("MacOSHeartRatePlugin")]
			private static extern void Connect_MacOS(string Identifier);
			[DllImport ("MacOSHeartRatePlugin")]
			private static extern void HeartRateCommand_MacOS(string Identifier, byte[] bytes);
#endif
    #endregion

    #region Variables
    	private bool _isInitialized = false;
		public bool IsInitialized {
			get
			{
				return _isInitialized;
			}
			private set
			{
            	_isInitialized = value;
			}
		}
		private bool _isScanning = false;
		public bool IsScanning {
			get {
				return _isScanning;
			}
			private set {
				_isScanning = value;
			}
		}

		#if UNITY_STANDALONE_OSX || UNITY_EDITOR
		private static HeartRatePlugin instance = null;
		#endif

	#endregion
	

	#region Event
		[Serializable]
		public sealed class EventArgs : System.EventArgs {
			public EventType Type { get; private set; }
			public string MacId { get; private set; }
			public string Info { get; private set; }
			public EventArgs (EventType type, string macId, string info) {
				Type = type;
				MacId = macId;
				Info = info;
			}
		}
		//provide Events
		public static event	EventHandler<EventArgs> Event;
	#endregion


	private void OnDestroy() {
		#pragma warning disable 0162
		if (isLogging) Debug.Log(TAG + "OnDestroy");
		#pragma warning restore 0162
		// Garbage native plugin
		#if UNITY_ANDROID && !UNITY_EDITOR
			if (AndroidHeartRatePlugin != null) AndroidHeartRatePlugin.Dispose();
		#elif UNITY_IOS && !UNITY_EDITOR
			Dispose_iOS();
		#elif UNITY_STANDALONE_OSX || UNITY_EDITOR
			Dispose_MacOS();
		#endif
	}

	void Awake() {
		#pragma warning disable 0162
		if (isLogging) Debug.Log(TAG + "Awake");
		#pragma warning restore 0162
		
		if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
			enabled = false;
        } else {
			DontDestroyOnLoad(gameObject);
		}

		#if UNITY_STANDALONE_OSX || UNITY_EDITOR
		instance = this;
		#endif
	}
	
	void Start() {
		#pragma warning disable 0162
		if (isLogging) Debug.Log(TAG + "Start: Initializing HeartRatePlugin");
		#pragma warning restore 0162

		Initialize(isLogging);
	}

	// ~HeartRatePlugin() { // Finalizer
	// }

	/* This can be used, if you want to Disconnect all or only some Devices, if the app enters Background
	private void OnApplicationPause(bool isPause) {	
		Debug.Log(TAG + "OnApplicationPause; isPause: " + isPause);
		if (isPause) {
			// if you want to do a reconnect if the app enters foreground, store all MacId in PlayerPrefs
			Disconnect(null, true);
		} else {
			// if you want to do a reconnect if the app enters foreground, get All MacId from PLayerPrefs
			// and do connect for all MacId: Connect(MacId);
		}
	} //*/

	void OnApplicationFocus(bool focusStatus)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
			Debug.Log($"{TAG}OnApplicationFocus: focusStatus:{focusStatus}");
			if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
			{
				Debug.Log($"{TAG}OnApplicationFocus: Permission.FineLocation granted");
				fineLocationPermissionGranted = true;
			}
			else
			{
				Debug.Log($"{TAG}OnApplicationFocus: Permission.FineLocation denied");
				fineLocationPermissionGranted = false;
			}

			if (!fineLocationPermissionAsked && focusStatus)
			{
				fineLocationPermissionAsked = true;
			}
		#endif
	}

	void Initialize(bool shouldSanPluginLog) {
		if (!IsInitialized) {
			#pragma warning disable 0162
			if (isLogging) Debug.Log (TAG + "Initialize " + (shouldSanPluginLog ? "with logging" : "without logging"));
			#pragma warning restore 0162
			#if UNITY_ANDROID && !UNITY_EDITOR
				using (AndroidJavaClass jc = new AndroidJavaClass("com.kaasa.androidheartratepluginmulti.HeartRatePlugin")) { // name of the class not the plugin-file
					AndroidHeartRatePlugin = jc.CallStatic<AndroidJavaObject>("instance");
					AndroidHeartRatePlugin.Call("InitPluginAndroid", shouldSanPluginLog);	
				}

				IsInitialized = true;
			#elif UNITY_IOS && !UNITY_EDITOR
				Initplugin_iOS(shouldSanPluginLog);	
			#elif UNITY_STANDALONE_OSX || UNITY_EDITOR
				Initplugin_MacOS(shouldSanPluginLog, ReportActualBleState, ReportNewSensor, ReportConnection, NotificationCallback);
			#endif
		}
	}
	
	#region Bluetooth Low Energy systemstatus
		#if UNITY_STANDALONE_OSX || UNITY_EDITOR
		public static void ReportActualBleState(string s_actualState) {
		#else
		public void ReportActualBleState(string s_actualState) {
		#endif
		
			#pragma warning disable 0162
			if (isLogging) Debug.Log(TAG + "ReportActualBleState: state: " + s_actualState);
			#pragma warning restore 0162

			#if UNITY_IOS && !UNITY_EDITOR
				IsInitialized = true;
			#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
				instance.IsInitialized = true;
			#endif

			BleHardwareState bleState = (BleHardwareState)int.Parse(s_actualState);
	
			#if UNITY_STANDALONE_OSX || UNITY_EDITOR
				if (instance.IsScanning == true) {
					instance.IsScanning = false;
				}
			#else
				if (IsScanning == true) {
					IsScanning = false;
				}
			#endif

			string logErrorString;

			switch (bleState) {
				case BleHardwareState.POWERED_OFF:
					logErrorString = "Bluetooth POWERED_OFF";
					#if UNITY_IOS && !UNITY_EDITOR
						logErrorString += "\nmaybe check Bluetooth in control center";
					#endif
					Debug.LogError(TAG + logErrorString);

					HeartRateSensor.SetAllDisconnected();
					HeartRateSensor.RemoveUnconnected();
					
					if (Event != null) {
						Event(null, new EventArgs(EventType.SYSTEM_NOT_SCANNING, null, logErrorString));
					}
				break;
				case BleHardwareState.POWERED_ON:
					#if UNITY_ANDROID && !UNITY_EDITOR
						Debug.Log(TAG + "Bluetooth-Hardware is turned on, checking Location");
						if (!AndroidHeartRatePlugin.Call<bool>("IsLocationTurnedOn")) {
							CheckBleStatus();
						} else {
							Scan();
						}
					#elif UNITY_IOS && !UNITY_EDITOR
						Debug.Log(TAG + "Bluetooth-Hardware is turned on");
						Scan();
					#elif UNITY_STANDALONE_OSX || UNITY_EDITOR
						Debug.Log(TAG + "Bluetooth-Hardware is turned on");
						instance.Scan();
					#endif
				break;
				case BleHardwareState.LOCATION_OFF:
					#if UNITY_ANDROID && !UNITY_EDITOR
					logErrorString = "LOCATION_OFF";
					Debug.LogError(TAG + logErrorString);

					AndroidHeartRatePlugin.Call("StopScan");
					
					if (Event != null) {
						Event(null, new EventArgs(EventType.SYSTEM_NOT_SCANNING, null, logErrorString));
					}
					#endif
				break;
				case BleHardwareState.LOCATION_ON:
					#if UNITY_ANDROID && !UNITY_EDITOR
						Debug.Log(TAG + "Location is on");
						if (!AndroidHeartRatePlugin.Call<bool>("IsBluetoothTurnedOn")) {
							CheckBleStatus();
						} else {
							Scan();
						}
					#endif
				break;
				case BleHardwareState.UNKNOWN:
				case BleHardwareState.RESETTING:
				case BleHardwareState.UNSUPPORTED:
				case BleHardwareState.UNAUTHORIZED:
					logErrorString = "Bluetooth is " + (BleHardwareState)bleState;
					Debug.LogError(TAG + logErrorString);
					
					if (Event != null) {
						Event(null, new EventArgs(EventType.SYSTEM_NOT_SCANNING, null, logErrorString));
					}
				break;
			}
		}

		private void CheckBleStatus() {
			#pragma warning disable 0162
			if (isLogging) Debug.Log (TAG + "checking Ble status");
			#pragma warning restore 0162

			string logErrorString = null;

			#if UNITY_ANDROID && !UNITY_EDITOR
				if (!AndroidHeartRatePlugin.Call<bool>("IsBleFeatured")) {
					logErrorString = "Bluetooth is not featured";
					return;
				} 
				if (!AndroidHeartRatePlugin.Call<bool>("IsBluetoothAvailable")) {
					logErrorString = "Bluetooth is not available";
					return;
				} 
				if (!AndroidHeartRatePlugin.Call<bool>("IsBluetoothTurnedOn")) {
					logErrorString = "Bluetooth is powered_off, try to turn on";
					AndroidHeartRatePlugin.Call("EnableBluetooth");
					return;
				}
				if (!AndroidHeartRatePlugin.Call<bool>("IsLocationTurnedOn")) {
					logErrorString = "Location is off, try to turn on";
					AndroidHeartRatePlugin.Call("EnableLocation");
				}
			#elif UNITY_IOS && !UNITY_EDITOR
				BleHardwareState state = (BleHardwareState)GetBLEStatus_iOS();
				switch (state) {
					case BleHardwareState.UNKNOWN:
						logErrorString = "Bluetooth is UNKNOWN";
					break;
					case BleHardwareState.RESETTING:
						logErrorString = "Bluetooth is RESETTING";
					break;
					case BleHardwareState.UNSUPPORTED:
						logErrorString = "Bluetooth is UNSUPPORTED";
					break;
					case BleHardwareState.UNAUTHORIZED:
						logErrorString = "Bluetooth is UNAUTHORIZED";
					break;
					case BleHardwareState.POWERED_OFF:
						logErrorString = "Bluetooth is POWERED_OFF, try to turn on";
						EnableBluetooth_iOS();
					break;
				}
			#endif

			if (logErrorString != null) {
				Debug.LogError(TAG + logErrorString);

				if (Event != null) {
					Event(null, new EventArgs(EventType.SYSTEM_NOT_SCANNING, null, logErrorString));
				}
			}
		}
	#endregion

	#region Scan
		public void StartScan() {
			#pragma warning disable 0162
			if (isLogging) Debug.Log (TAG + "StartScan, checking Ble-status");
			#pragma warning restore 0162

			if (!IsInitialized) {
				Debug.LogError(TAG + "StartScan: ScanController is not initialized. Did you forget to add ScanController object in the scene?");
				return;
			}

			#if UNITY_ANDROID && !UNITY_EDITOR
				if (!AndroidHeartRatePlugin.Call<bool>("IsBleFeatured") || !AndroidHeartRatePlugin.Call<bool>("IsBluetoothAvailable") || !AndroidHeartRatePlugin.Call<bool>("IsBluetoothTurnedOn") || !AndroidHeartRatePlugin.Call<bool>("IsLocationTurnedOn")) {
					Debug.Log(TAG + "Scan not possible");
					CheckBleStatus();
					return;
				}

				Scan();
			#elif UNITY_IOS && !UNITY_EDITOR
				if (GetBLEStatus_iOS() != 5) {
					Debug.Log(TAG + "Scan not possible");
					CheckBleStatus();
					return;
				}
				
				Scan();
			#elif UNITY_STANDALONE_OSX || UNITY_EDITOR
				if (GetBLEStatus_MacOS() != 5) {
					Debug.Log(TAG + "Scan not possible");
					return;
				}

				Scan();
			#endif
		}
		private void Scan() {
			if (IsScanning) {
				return;
			}
			#pragma warning disable 0162
			if (isLogging) Debug.Log (TAG + "Scan");
			#pragma warning restore 0162
			
			IsScanning = true;
			
			HeartRateSensor.RemoveUnconnected();
			
			if (Event != null) {
				Event(null, new EventArgs(EventType.REMOVE_UNCONNECTED, null, "unconnected sensors removed from list"));
			}
			
			#if UNITY_ANDROID && !UNITY_EDITOR
				#if UNITY_2018_3_OR_NEWER
					StartCoroutine(RequestPermissionRoutine());
					return;
				#else
					AndroidHeartRatePlugin.Call("Scan");
				#endif
			#elif UNITY_IOS && !UNITY_EDITOR
				Scan_iOS();
			#elif UNITY_STANDALONE_OSX || UNITY_EDITOR
				Scan_MacOS();
			#endif

			if (Event != null) {
				Event(null, new EventArgs(EventType.SYSTEM_SCANNING, null, "scanning"));
			}
		}

		bool fineLocationPermissionAsked;
		bool fineLocationPermissionGranted = true;
		private IEnumerator RequestPermissionRoutine()
		{
			Debug.Log($"{TAG}RequestPermissionRoutine");
#if UNITY_ANDROID && !UNITY_EDITOR
			if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
				Debug.Log($"{TAG}RequestPermissionRoutine: already granted");
			else
			{
				fineLocationPermissionAsked = false;
				Permission.RequestUserPermission(Permission.FineLocation);
				yield return new WaitUntil(() => fineLocationPermissionAsked == true);
				Debug.Log($"{TAG}RequestPermissionRoutine: next");
			}

			if (Event != null) {
				if (fineLocationPermissionGranted)
				{
					AndroidHeartRatePlugin.Call("Scan");
					Event(null, new EventArgs(EventType.SYSTEM_SCANNING, null, "scanning"));
				}
				else
					Event(null, new EventArgs(EventType.SYSTEM_NOT_SCANNING, null, "scanning"));
			}
#else
 			yield break;
#endif
    }

		public void StopScan() {
			if (!IsScanning) {
				return;
			}
			#pragma warning disable 0162
			if (isLogging) Debug.Log (TAG + "StopScan");
			#pragma warning restore 0162
			
			IsScanning = false;
			
			#if UNITY_ANDROID && !UNITY_EDITOR
				AndroidHeartRatePlugin.Call("StopScan");
			#elif UNITY_IOS && !UNITY_EDITOR
				StopScan_iOS();
			#elif UNITY_STANDALONE_OSX || UNITY_EDITOR
				StopScan_MacOS();
			#endif
			
			if (Event != null) {
				Event(null, new EventArgs(EventType.SYSTEM_NOT_SCANNING, null, "not scanning"));
			}
		}

		#if UNITY_STANDALONE_OSX || UNITY_EDITOR
		public static void ReportNewSensor(string Device) {
		#else
		public void ReportNewSensor(string Device) {
		#endif
			#pragma warning disable 0162
			if (isLogging) Debug.Log(TAG + "ReportNewSensor: " + Device);
			#pragma warning restore 0162

			string[] splitString = Device.Split('|');
			string macId = splitString[0];
			string name = splitString[1];
			string s_rssi = splitString[2];
			int i_rssi = int.Parse(s_rssi);

			if (!HeartRateSensor.Sensors.Contains(macId)) {
				#pragma warning disable 0162
				if (isLogging) Debug.Log(TAG + macId + " (" + name + ") is new");
				#pragma warning restore 0162
				
				HeartRateSensor heartRateSensor = new HeartRateSensor(macId, name, i_rssi, false, false, null);
				HeartRateSensor.Sensors.Add(heartRateSensor);
				if (Event != null) {
					Event(null, new EventArgs(EventType.NEW_SENSOR, macId, "new sensor found"));
				}
			}
		}
	#endregion

	#region Connection
		public void Connect(string MacId) {
		if (!IsInitialized) {
			Debug.LogError(TAG + "Connect: HeartRatePlugin is not initialized. Did you forget to add HeartRatePlugin object in the scene?");
			return;
		}

		string name = HeartRateSensor.Sensors[MacId].Name;

		#pragma warning disable 0162
		if (isLogging) Debug.Log (TAG + "Connect: " + MacId + " (" + name + ")");
		#pragma warning restore 0162

		HeartRateSensor.Sensors[MacId].IsConnecting = true;

		// connecting event is send from native bridge
		
		#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidHeartRatePlugin.Call("Connect", MacId);
		#elif UNITY_IOS && !UNITY_EDITOR
			Connect_iOS(MacId);
		#elif UNITY_STANDALONE_OSX || UNITY_EDITOR
			Connect_MacOS(MacId);
		#endif
		}

		public void Disconnect(string MacId, bool All) {
			#pragma warning disable 0162
			if (isLogging) Debug.Log (TAG + "Disconnect: " + (All ? "All" : MacId + " (" + HeartRateSensor.Sensors[MacId].Name + ")"));
			#pragma warning restore 0162
			
			if (All) {
				#if UNITY_ANDROID && !UNITY_EDITOR
					AndroidHeartRatePlugin.Call("DisconnectAll");
				#elif UNITY_IOS && !UNITY_EDITOR
					DisconnectAll_iOS();
				#elif UNITY_STANDALONE_OSX || UNITY_EDITOR
					DisconnectAll_MacOS();
				#endif
			} else {
				#if UNITY_ANDROID && !UNITY_EDITOR
					AndroidHeartRatePlugin.Call("Disconnect", MacId);
				#elif UNITY_IOS && !UNITY_EDITOR
					Disconnect_iOS(MacId);
				#elif UNITY_STANDALONE_OSX || UNITY_EDITOR
					Disconnect_MacOS(MacId);
				#endif
			}
		}

		#if UNITY_STANDALONE_OSX || UNITY_EDITOR
		public static void ReportConnection(string Device) {
		#else
		public void ReportConnection(string Device) {
		#endif
			#pragma warning disable 0162
			if (isLogging) Debug.Log(TAG + "ReportConnection: " + Device);
			#pragma warning restore 0162

			string[] splitString = Device.Split('|');
			string macId = splitString[0];
			
			EventType connectionState = (EventType)int.Parse(splitString[1]);
			
			string reason = splitString[2];

			switch (connectionState) {
				case EventType.CONNECTING:
					HeartRateSensor.Sensors[macId].IsConnecting = true;
				break;
				case EventType.CONNECTED:
					HeartRateSensor.Sensors[macId].IsConnecting = false;
					HeartRateSensor.Sensors[macId].IsConnected = true;
				break;
				case EventType.DISCONNECTED:
					HeartRateSensor.Sensors[macId].IsConnecting = false;
					HeartRateSensor.Sensors[macId].IsConnected = false;
					if (reason == "DEVICE_NOT_AVAILABLE") {
						HeartRateSensor.Sensors.RemoveAt(HeartRateSensor.Sensors.IndexOf(HeartRateSensor.Sensors[macId]));
					}
				break;
			}

			if (Event != null) {
				Event(null, new EventArgs(connectionState, macId, macId + " " + (EventType)connectionState + (reason == "" ? "" : (": " + reason))));
			}
		}
	#endregion

	#region Notificatiopn
		#if UNITY_STANDALONE_OSX || UNITY_EDITOR
		public static void NotificationCallback(string Notification) {
		#else
		public void NotificationCallback(string Notification) {
		#endif
			#pragma warning disable 0162
			if (isLogging) Debug.Log(TAG + "NotificationCallback: " + Notification);
			#pragma warning restore 0162

			string[] splitString = Notification.ToUpper().Split('|');
			string macId = splitString[0];
			string notification = splitString[1];

			string[] notificationSplit = notification.Split(';');
			int notificationSplitCount = notificationSplit.Length;

			string prefix;
			string value;
			EventType type;
			if (notificationSplitCount > 1) {
				prefix = notificationSplit[0];
				value = notificationSplit[1];
				#pragma warning disable 0162
				if (isLogging) {
					Debug.Log(TAG + "NotificationCallback; prefix: " + prefix);
					Debug.Log(TAG + "NotificationCallback; value : " + value);
				}
				#pragma warning restore 0162

				if (prefix == "2A39") {
                	type = EventType.NOTIFICATION_CONTROLPOINT;
				} else if (prefix == "2A38") {
					type = EventType.NOTIFICATION_BODYSENSORLOCATION;
				} else if (prefix == "2A37") {
                	type = EventType.NOTIFICATION_MEASUREMENT;
				} else {
					Debug.LogError(TAG + "NotificationCallback; no matching prefix found");
                	return;
            	}

            	HeartRateSensor.HandleHeartRateNotification(macId, prefix, value);
			
				if (Event != null) {
					#pragma warning disable 0162
					if (isLogging) Debug.Log($"{TAG}NotificationCallback; raising {type}-event, MacId: " + macId + ", Info: got notification for " + macId);
					#pragma warning restore 0162
					Event(null, new EventArgs(type, macId, "got notification for " + macId));
				}
			}
		}
	#endregion
}