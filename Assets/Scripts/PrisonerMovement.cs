using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonerMovement : MonoBehaviour {

	public float speed = 8f, maxVelocity = 4f, jumpForce = 4f;

	private Rigidbody2D myBody;
	private Animator anim;
	[SerializeField]
	private bool isJumping = false;

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
		float forceY = 0f;
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

		if (Input.GetButton ("Jump") && !isJumping) 
		{
			isJumping = true;
			forceY = jumpForce;
			anim.SetBool ("isJumping", true);
			myBody.AddForce (new Vector2(0, forceY), ForceMode2D.Impulse);
		}

		myBody.AddForce (new Vector2(forceX, 0));
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.name == "TmpGround")
		{
			isJumping = false;
			anim.SetBool ("isJumping", false);
		}
	}
}
