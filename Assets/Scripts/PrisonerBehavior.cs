using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonerBehavior : MonoBehaviour {

	public GameObject gameManager;

	private Transform weaponAnchorTransform;

	[SerializeField] private int characterNumber;
	public int CharacterNumber { 
		get { 
			return characterNumber;
		} 
	}

	// Use this for initialization
	void Start () {
		weaponAnchorTransform = transform.Find ("WeaponAnchor").transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.KeypadEnter)) {
			gameManager.GetComponent<MenuManager>().ToggleWeaponMenu (this.gameObject);
		}

		if (Input.GetMouseButtonUp (0)) {
			foreach (Transform child in weaponAnchorTransform) {
				if (child.tag == "Weapon") {
					child.GetComponent<WeaponController>().ShotBullet();
				}	
			}
		}
	}
}
