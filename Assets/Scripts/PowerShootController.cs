using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerShootController : MonoBehaviour {

	public Text text1;

	private bool isShooting = false;
	private float barWidth;
	private Vector3 initialMousePosition;
	private Vector3 OriginalBarScale;
	private SpriteRenderer myRenderer;
	private Sprite mySprite;
	private Texture2D baseTexture;


	// Use this for initialization
	void Start () {
		baseTexture = (Texture2D)Resources.Load("progress-bar");
		mySprite = Sprite.Create (baseTexture, new Rect(0f,0f, 0f, baseTexture.height), new Vector2(0.5f, 0.5f), 250f);
		text1.text = baseTexture.width.ToString();
		//transform.localScale = new Vector3 (0, 1, 1);
		OriginalBarScale = this.transform.localScale;
		myRenderer = GetComponent<SpriteRenderer> ();
		myRenderer.sprite = mySprite;

	}
	
	// Update is called once per frame
	void Update () {
		

		if (Input.GetMouseButtonDown (0)) {
			initialMousePosition = Input.mousePosition;
		}

		if (Input.GetMouseButton (0)) {
			float barLength;
			barLength = Mathf.Clamp (Input.mousePosition.x - initialMousePosition.x, 0f, baseTexture.width);
			mySprite = Sprite.Create (baseTexture, new Rect(0f,0f, barLength, baseTexture.height), new Vector2(0f, 0f), 250f);
			myRenderer.sprite = mySprite;

			/*
			transform.localScale = new Vector3 (
				(transform.localScale.x + (Input.mousePosition.x - initialMousePosition.x)) / 100,
				transform.localScale.y,
				transform.localScale.z
			);*/
		}
	}


}
