using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {

	GameObject[] Players;
	Transform currentActivePlayerTransform;
	Transform cameraTransform;

	// Use this for initialization
	void Start () {
		Players = GameObject.FindGameObjectsWithTag ("Player");
		SetActivePlayer (Random.Range (0, Players.Length));

		cameraTransform = transform.GetChild (0);
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 newCameraPosition = new Vector3 (currentActivePlayerTransform.position.x, currentActivePlayerTransform.position.y, cameraTransform.position.z);
		cameraTransform.position = Vector3.Lerp (cameraTransform.position, newCameraPosition, 5f * Time.deltaTime);

		if (Input.GetKeyUp (KeyCode.Space)) {
			SetActivePlayer (Random.Range (0, Players.Length));
		}
	}

	void SetActivePlayer(int playerNumber) {
		if (Players [playerNumber]) {
			currentActivePlayerTransform = Players [playerNumber].transform;
		}
	}
}
