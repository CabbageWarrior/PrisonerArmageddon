using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonerMovement : MonoBehaviour {

	public float speed = 8f, maxVelocity = 4f;

	private Rigidbody2D myBody;
	private Animator anim;

	void Awake()
	{
		myBody = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
	}

	void Start ()
	{
		
	}
	
	void Update () 
	{
		PrisonerMoveKeyboard ();
	}

	void PrisonerMoveKeyboard()
	{
		float forceX = 0f;
		float vel = Mathf.Abs (myBody.velocity.x);

		float h = Input.GetAxisRaw ("Horizontal");
		if (h > 0) {
			if (vel < maxVelocity)
				forceX = speed;

			if (transform.localScale.x < 0) 
			{
				Vector3 temp = transform.localScale;
				temp.x *= -1;
				transform.localScale = temp;
			} 


			anim.SetBool ("isWalking", true);


		} else if (h < 0) {

			if (vel < maxVelocity)
				forceX = -speed;

			if (transform.localScale.x > 0) 
			{
				Vector3 temp = transform.localScale;
				temp.x *= -1;
				transform.localScale = temp;
			} 

			anim.SetBool ("isWalking", true);

		} else {
			anim.SetBool ("isWalking", false);
		}

		myBody.AddForce (new Vector2(forceX, 0));
	}
}
