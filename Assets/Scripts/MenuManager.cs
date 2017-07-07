using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

	public GameObject weaponMenu;

	public GameObject playerRef;

	//// Use this for initialization
	//void Start () {
		
	//}
	
	//// Update is called once per frame
	//void Update () {
		
	//}

	#region Weapon Menu
	/// <summary>
	/// Opens the weapon menu.
	/// </summary>
	/// <param name="currentPlayer">Current player.</param>
	public void OpenWeaponMenu (GameObject currentPlayer){
		playerRef = currentPlayer;
		weaponMenu.SetActive (true);
	
	}

	/// <summary>
	/// Closes the weapon menu.
	/// </summary>
	/// <param name="currentPlayer">Current player.</param>
	public void CloseWeaponMenu (GameObject currentPlayer){
		playerRef = null;
		weaponMenu.SetActive (false);

	}

	public void ToggleWeaponMenu (GameObject currentPlayer){

		if (weaponMenu.activeInHierarchy) {
			CloseWeaponMenu (currentPlayer);
		} else {
			OpenWeaponMenu (currentPlayer);
		}

	}

	/// <summary>
	/// Selects the weapon.
	/// </summary>
	/// <param name="weapon">Weapon.</param>
	public void SelectWeapon(GameObject weapon) {
		if (playerRef != null) {
			// Step 1: Destroy actual weapon.

			foreach (Transform child in playerRef.transform.Find("WeaponAnchor")) {
				if (child.tag == "Weapon") {
					Destroy(child.gameObject);
				}	
			}

			// Step 2: Instantiate selected weapon.
			Instantiate (weapon, playerRef.transform.Find("WeaponAnchor"));

			// Step 3: Close the menu.
			CloseWeaponMenu (playerRef);
		}
	}
	#endregion

}


