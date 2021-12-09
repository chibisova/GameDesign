using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class HeartRateSensor {
	private const string	TAG = "HeartRateSensor; ";
	private const bool		isLogging = false;

	public enum HRA_ControlPointStatus {
		NOT_SUPPORTED,
		SUPPORTED
	}
	public enum HRM_SensorContactStatus {
		NOT_SUPPORTED,
		NOT_SUPPORTED_1,
		NO_CONTACT,
		CONTACT
	}

	public enum HRM_BodySensorLocation : uint {
		OTHER,
		CHEST,
		WRIST,
		FINGER,
		HAND,
		EARLOBE,
		FOOT
	}

	public enum HRM_EnergyExpendedStatus {
		NOT_PRESENT,
		PRESENT
	}

	public enum HRM_ValueFormat {
		UINT8,
		UINT16
	}

	/// <summary>MacAdress or identifier</summary>
	public string MacId;
	/// <summary>AdvertismentName</summary>
	public string Name;
	/// <summary>Distance from Sensor to MobileDevice</summary>
	public int Rssi;
	public bool IsConnecting = false;
	public bool IsConnected = false;
	public uint? PulseRate;
	public HRM_SensorContactStatus SCStatus;
    /// <summary>null: not present</summary>
    public uint? EnergyExpended = null;
    /// <summary>null: not present</summary>
    public List<uint> RrInterval;
    /// <summary>mandatory</summary>
    public HRM_BodySensorLocation SensorLocation;
	/// <summary>0 or null: not supported</summary>
    public HRA_ControlPointStatus? HR_ControlPoint;


	public HeartRateSensor(string macId, string name, int rssi, bool isConnecting, bool isConnected,
							uint? _PulseRate, HRM_SensorContactStatus _SCStatus = 0, uint? _EnergyExpended = null, List<uint> _RrInterval = null, HRM_BodySensorLocation _SensorLocation = 0, HRA_ControlPointStatus? _HR_ControlPoint = null) {
		MacId = macId;
		Name = name;
		Rssi = rssi;
		IsConnecting = isConnecting;
		IsConnected = isConnected;

		PulseRate = _PulseRate;
		EnergyExpended = _EnergyExpended;
		RrInterval = _RrInterval;
		SCStatus = _SCStatus;
		SensorLocation	= _SensorLocation;
		HR_ControlPoint = _HR_ControlPoint;
	}
	public class HeartRateSensorDictionary : KeyedCollection<string, HeartRateSensor> {
		protected override string GetKeyForItem(HeartRateSensor item) {
        return item.MacId;
		}
	}
	public static HeartRateSensorDictionary Sensors = new HeartRateSensorDictionary();

	public static void RemoveUnconnected() {
		#pragma warning disable 0162
		if (isLogging) Debug.Log(TAG + "RemoveUnconnected");
		#pragma warning restore 0162

		// removes devices, which are not connected AND not connecting
		// keeps devices, which are connected OR connecting
		for (int i = Sensors.Count - 1; i >= 0 ; i--) {
			if (!Sensors[i].IsConnected && !Sensors[i].IsConnecting) {
				#pragma warning disable 0162
				if (isLogging) Debug.Log(TAG + "RemoveUnconnected: " + Sensors[i].MacId + " (" + Sensors[i].Name + ")");
				#pragma warning restore 0162
				Sensors.RemoveAt(i);
			}
		}
	}

	public static void SetAllDisconnected() {
		#pragma warning disable 0162
		if (isLogging) Debug.Log(TAG + "SetAllDisconnected");
		#pragma warning restore 0162

		for (int i = Sensors.Count - 1; i >= 0 ; i--) {
			Debug.Log(TAG + "SetDisconnected: " + Sensors[i].Name);
			Sensors[i].IsConnecting = false;
			Sensors[i].IsConnected = false;
		}
	}


	public static void HandleHeartRateNotification(string MacId, string Prefix, string Value) {
		#pragma warning disable 0162
		if (isLogging) Debug.Log(TAG + "HandleHeartRateNotification; MacId:  " + MacId + ", Prefix: " + Prefix + ", Value: " + Value + "|");
		#pragma warning restore 0162

		int listIndex = Sensors.IndexOf(Sensors[MacId]);
		HeartRateSensor sensor = HeartRateSensor.Sensors[listIndex];

		switch (Prefix) {
			case "2A39":
				#pragma warning disable 0162
				if (isLogging) Debug.Log(TAG + "HandleHeartRateNotification: handle Heart Rate Control Point");
				#pragma warning restore 0162

				sensor.HR_ControlPoint = (HRA_ControlPointStatus)int.Parse(Value);
			break;
			case "2A38":
				#pragma warning disable 0162
				if (isLogging) Debug.Log(TAG + "HandleHeartRateNotification: handle Body Sensor Location");
				#pragma warning restore 0162

				sensor.SensorLocation = (HRM_BodySensorLocation)int.Parse(Value);
			break;
			case "2A37":
				#pragma warning disable 0162
				if (isLogging) Debug.Log(TAG + "HandleHeartRateNotification: handle Heart Rate Measurement");
				#pragma warning restore 0162

				string[] valueSplit = Value.Split(' ');
				int valueSplitCount = valueSplit.Length - 1; // last string is always empty
				#pragma warning disable 0162
				if (isLogging) Debug.Log(TAG + "HandleHeartRateNotification: valueSplitCount: " + valueSplitCount);
				#pragma warning restore 0162

				/* *** Flags (mandatory) *** */
				int flag = int.Parse(valueSplit[0], System.Globalization.NumberStyles.HexNumber);
				#pragma warning disable 0162
				if (isLogging) Debug.Log(TAG + "HandleHeartRateNotification: Flags: " + flag);
				#pragma warning restore 0162

				// Heart Rate Value Format
				HRM_ValueFormat format  = (HRM_ValueFormat)(flag & 0x01);
				#pragma warning disable 0162
				if (isLogging) Debug.Log(TAG + "HandleHeartRateNotification: Heart Rate Value Format: " + (HRM_ValueFormat)format);
				#pragma warning restore 0162
				
				// Sensor Contact Status
				HRM_SensorContactStatus contactStatus = (HRM_SensorContactStatus)((flag & 0x06) >> 1);
				#pragma warning disable 0162
				if (isLogging) Debug.Log(TAG + "HandleHeartRateNotification: Sensor Contact Status: " + (HRM_SensorContactStatus)contactStatus);
				#pragma warning restore 0162

				if (contactStatus == HRM_SensorContactStatus.CONTACT || contactStatus == HRM_SensorContactStatus.NO_CONTACT) {
					sensor.SCStatus = contactStatus;
				}

				// Energy Expended Status
				HRM_EnergyExpendedStatus expendedStatus = (HRM_EnergyExpendedStatus)((flag & 0x08) >> 3);
				#pragma warning disable 0162
				if (isLogging) Debug.Log(TAG + "HandleHeartRateNotification: Energy Expended Status: " + (HRM_EnergyExpendedStatus)expendedStatus);
				#pragma warning restore 0162

				// RR-Interval
				HRM_EnergyExpendedStatus rrIntervalStatus = (HRM_EnergyExpendedStatus)((flag & 0x10) >> 4);
				#pragma warning disable 0162
				if (isLogging) Debug.Log(TAG + "HandleHeartRateNotification: RR-Interval: " + (HRM_EnergyExpendedStatus)rrIntervalStatus);
				#pragma warning restore 0162

				if (rrIntervalStatus == HRM_EnergyExpendedStatus.PRESENT) {
					sensor.RrInterval = new List<uint>();
				}

				/* *** Values *** */
				int offset = 1;
				// PulseRate
				if (format == HRM_ValueFormat.UINT8) {
					sensor.PulseRate = uint.Parse(valueSplit[offset], System.Globalization.NumberStyles.HexNumber);
					offset++;
				} else {
					sensor.PulseRate = uint.Parse(valueSplit[offset+1]+valueSplit[offset], System.Globalization.NumberStyles.HexNumber);
					offset += 2;
				}
				
				#pragma warning disable 0162
				if (isLogging) Debug.Log(TAG + "HandleHeartRateNotification: Puls: " + sensor.PulseRate);
				#pragma warning restore 0162

				// Energy Expended
				if (expendedStatus == HRM_EnergyExpendedStatus.PRESENT) {
					sensor.EnergyExpended = uint.Parse(valueSplit[offset+1]+valueSplit[offset], System.Globalization.NumberStyles.HexNumber);
					#pragma warning disable 0162
					if (isLogging) Debug.Log(TAG + "HandleHeartRateNotification: Energy Expended: " + sensor.EnergyExpended);
					#pragma warning restore 0162
					offset += 2;
				}

				// RR-Interval
				if (rrIntervalStatus == HRM_EnergyExpendedStatus.PRESENT) {
					for (int i = offset+1; i < valueSplitCount; i += 2) {
						sensor.RrInterval.Add(uint.Parse(valueSplit[offset+1]+valueSplit[offset], System.Globalization.NumberStyles.HexNumber));
					}
					#pragma warning disable 0162
					if (isLogging) Debug.Log(TAG + "HandleHeartRateNotification: (last)RR-Interval: " + sensor.RrInterval[sensor.RrInterval.Count-1]);
					#pragma warning restore 0162
				}
			break;
		}
	}
}