using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Visualize : MonoBehaviour {
	private const string	TAG = "Visualize; ";

	public enum VisualizationStyle{Heart,ECG,Impulse,Sine,Triangle};
	private VisualizationStyle visualizationStyle = VisualizationStyle.Heart;

	private bool isBeating = false;
	public bool IsBeating {
		get {
			return isBeating;
		}
		set {
			isBeating = value;
			// Debug.Log("IsBeating set: " + value);
	
			if (!value && lineRenderer != null) {
				tickrate = 0;
				
				ClearLineRenderer();
			}
		}
	}
	public uint				BPM = 0;
	private float			tickrate = 0;
	private int				drawDotPosition = 0;
	private float			scaleFactor = 1.3F;

	#pragma warning disable 0649
    [SerializeField]
	private Image			imageHeart;

    [SerializeField]
	private LineRenderer	lineRenderer;
	#pragma warning restore 0649
    private Vector3			originScale = new Vector3();
    private int				lineRendererPositionCount = 125;
	private List<Vector3>	currentVectorLineArray = new List<Vector3>();
	private Vector3			currentVectorDot = new Vector3();
	private float[]			ECG_Dots = {0,2F,4F,5F,4F,2F,0,0,0,-6F,48F,-10F,0,0,0,0,0,2F+2F/3F,5F+1F/3F,8F,9F,8F,5F+1F/3F,2F+2F/3F,0};


	void Start () {
		originScale = imageHeart.transform.localScale;
	}
	
	void Update () {
		if (isBeating) {
			if (visualizationStyle == VisualizationStyle.Heart) {
				VisualizeHeart();
			} else if (visualizationStyle == VisualizationStyle.ECG) {
				VisualizeECG();
			} else if (visualizationStyle == VisualizationStyle.Impulse) {
				VisualizeImpulse();
			} else if (visualizationStyle == VisualizationStyle.Sine) {
				VisualizationSine();
			} else if (visualizationStyle == VisualizationStyle.Triangle) {
				VisualizeTriangle();
			}
		} 
	}

	public void onDropDownChanged(TMP_Dropdown dropdown) {
		if ((VisualizationStyle)dropdown.value == VisualizationStyle.Heart) {
			lineRenderer.gameObject.SetActive(false);

			imageHeart.gameObject.SetActive(true);
		} else {
			imageHeart.gameObject.SetActive(false);

			lineRenderer.gameObject.SetActive(true);

			lineRenderer.positionCount = lineRendererPositionCount;
			
			ClearLineRenderer();

			tickrate = 0;
		}

		visualizationStyle = (VisualizationStyle)dropdown.value;
	}

	private void ClearLineRenderer() {
		currentVectorLineArray.Clear();
		for (int i = 0; i < lineRendererPositionCount; i++) {
			currentVectorLineArray.Add(new Vector3((float)i,0,0));

			lineRenderer.SetPosition(i,new Vector3(0,0,0));	
		}
	}

	void VisualizeHeart() {
		if (tickrate <= 0) {
			if (BPM > 20) {
				tickrate = 60000 / BPM;
				imageHeart.transform.localScale = new Vector3(originScale.x*scaleFactor,originScale.y*scaleFactor,originScale.z*scaleFactor);
			}
		} else {
			imageHeart.transform.localScale =	new Vector3(Mathf.Lerp(imageHeart.transform.localScale.x, originScale.x, 3*Time.deltaTime),
															Mathf.Lerp(imageHeart.transform.localScale.y, originScale.y, 3*Time.deltaTime),
															Mathf.Lerp(imageHeart.transform.localScale.z, originScale.z, 3*Time.deltaTime));	
		}
		tickrate -= 1000*Time.deltaTime;
	}

	void VisualizeECG() {
		if (tickrate <= 0) {
			drawDotPosition = 0;
			tickrate = 60000F / (float)(BPM == 0 ? 1 : BPM);

			currentVectorDot = new Vector3(0, ECG_Dots[ECG_Dots.Length-1], 0);
		} else {
			if (drawDotPosition < ECG_Dots.Length) {
				currentVectorDot = new Vector3(0, ECG_Dots[ECG_Dots.Length-1-drawDotPosition], 0);
			} else {
				currentVectorDot = new Vector3(0, 0, 0);
			}
		}
		drawDotPosition++;

		List<Vector3> shiftedVectorLineArray = new List<Vector3>();
		shiftedVectorLineArray.Add(currentVectorDot);
		
		// copy from 2nd item
		for (int i = 1; i < currentVectorLineArray.Count; i++) {
			shiftedVectorLineArray.Add(new Vector3(i, currentVectorLineArray[i-1].y, 0));
		}

		currentVectorLineArray = shiftedVectorLineArray;

		for (int j = 0; j < currentVectorLineArray.Count; j++) {
			lineRenderer.SetPosition(j,currentVectorLineArray[j]);	
		}

		tickrate -= 1000*Time.deltaTime;
	}

	void VisualizeImpulse() {
		if (tickrate <= 0) {
			drawDotPosition = 0;
			tickrate = 60000F / (float)(BPM == 0 ? 1 : BPM);

			currentVectorDot = new Vector3(0, 48, 0);
		} else {

			if (drawDotPosition < 25) {
				currentVectorDot = new Vector3(0, 48-2*drawDotPosition, 0);
			} else {
				currentVectorDot = new Vector3(0, 0, 0);
			}
		}
		drawDotPosition++;

		List<Vector3> shiftedVectorLineArray = new List<Vector3>();
		shiftedVectorLineArray.Add(currentVectorDot);
		
		// copy from 2nd item
		for (int i = 1; i < currentVectorLineArray.Count; i++) {
			shiftedVectorLineArray.Add(new Vector3(i, currentVectorLineArray[i-1].y, 0));
		}

		currentVectorLineArray = shiftedVectorLineArray;

		for (int j = 0; j < currentVectorLineArray.Count; j++) {
			lineRenderer.SetPosition(j,currentVectorLineArray[j]);	
		}

		tickrate -= 1000*Time.deltaTime;
	}

	void VisualizationSine() {
		if (tickrate <= 0) {
			tickrate = 359;
		}
		
		List<Vector3> shiftedVectorLineArray = new List<Vector3>();
		shiftedVectorLineArray.Add(new Vector3(0, Mathf.Sin (tickrate*Mathf.PI/180)*40.0F, 0));
		
		// copy from 2nd item
		for (int i = 1; i < currentVectorLineArray.Count; i++) {
			shiftedVectorLineArray.Add(new Vector3(i, currentVectorLineArray[i-1].y, 0));
		}

		currentVectorLineArray = shiftedVectorLineArray;

		for (int j = 0; j < currentVectorLineArray.Count; j++) {
			lineRenderer.SetPosition(j,currentVectorLineArray[j]);	
		}

		tickrate -= 1000*Time.deltaTime*BPM/120;
	}

	void VisualizeTriangle() {
		if (tickrate < 0){
			tickrate = 359;
		}

		if (tickrate < 180) {
			currentVectorDot = new Vector3(0, tickrate*.22F, 0);
		} else {
			currentVectorDot = new Vector3(0, (359-tickrate)*.22F, 0);
		}

		List<Vector3> shiftedVectorLineArray = new List<Vector3>();
		shiftedVectorLineArray.Add(currentVectorDot);
		
		// copy from 2nd item
		for (int i = 1; i < currentVectorLineArray.Count; i++) {
			shiftedVectorLineArray.Add(new Vector3(i, currentVectorLineArray[i-1].y, 0));
		}

		currentVectorLineArray = shiftedVectorLineArray;

		for (int j = 0; j < currentVectorLineArray.Count; j++) {
			lineRenderer.SetPosition(j,currentVectorLineArray[j]);	
		}

		tickrate -= 1000*Time.deltaTime*BPM/120;
	}
}
