using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {

	Camera cam;

	void Awake() {
		cam = Camera.main;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		// Getting mouse coordinates referred to the scene.
		float camDis = cam.transform.position.y - transform.position.y;
		Vector3 mouse = cam.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, camDis));

		// Check if weapon must flip.
		bool flipSprites = ((mouse.x - transform.position.x) < 0);

		// Getting the absolute Rotation angle based on mouse coords and pivot object coords.
		float AngleRad = Mathf.Atan2 (mouse.y - transform.position.y, Mathf.Abs(mouse.x - transform.position.x));
		float AngleDeg = (180 / Mathf.PI) * AngleRad;

		bool isPlayerFlipped = (transform.parent.parent.transform.localScale.x < 0);

		// Rotate the weapon dinamically.
		if (flipSprites) {
			//Debug.Log ("FLIPPA!");
			transform.localScale = new Vector3 (-Mathf.Abs (transform.localScale.x) * (isPlayerFlipped ? -1 : 1), transform.localScale.y, transform.localScale.z);
			transform.rotation = Quaternion.Euler (0, 0, -AngleDeg);
		} else {
			//Debug.Log ("NON FLIPPARE!");
			transform.localScale = new Vector3 (Mathf.Abs (transform.localScale.x) * (isPlayerFlipped ? -1 : 1), transform.localScale.y, transform.localScale.z);
			transform.rotation = Quaternion.Euler (0, 0, AngleDeg);
		}


	}
}
