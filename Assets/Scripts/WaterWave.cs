using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWave: MonoBehaviour {

    public float waveIntensity = 0.15f, waveFrequency = 1f;
    private Vector3 startPosition;
    
    // Use this for initialization
    void Start () {

        startPosition = transform.position;

    }
	
	// Update is called once per frame
	void Update () {

        transform.position = startPosition + (new Vector3(0.0f, Mathf.Sin(Time.time * waveFrequency), 0.0f) * waveIntensity);

    }

}
