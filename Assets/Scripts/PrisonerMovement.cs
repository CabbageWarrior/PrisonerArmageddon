using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonerMovement : MonoBehaviour {

	public float speed = 12f, maxVelocity = 3f, jumpForce = 4f;
	public LayerMask whatIsGround;

//	private int whatIsGround = 1;
	private Rigidbody2D myBody;
	[SerializeField]
	private bool isJumping = false;
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
		float forceY = 0f;
		float vel = Mathf.Abs (myBody.velocity.x);

		float horizontalInput = Input.GetAxisRaw ("Horizontal");

		if (!isJumping) {

			RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 1f, whatIsGround);
			Debug.Log (Mathf.Abs(hit.normal.x));

			if (horizontalInput > 0) {
				if (vel < maxVelocity)
					forceX = speed;
				if (transform.localScale.x < 0) {
					myBody.velocity = new Vector2 (0, 0);
					Vector3 temp = transform.localScale;
					temp.x *= -1;
					transform.localScale = temp;
				} 
					
				anim.SetBool ("isWalking", true);

			} else if (horizontalInput < 0) {

				if (vel < maxVelocity)
					forceX = -speed;
				if (transform.localScale.x > 0) {
					myBody.velocity = new Vector2 (0, 0);
					Vector3 temp = transform.localScale;
					temp.x *= -1;
					transform.localScale = temp;
				} 

				anim.SetBool ("isWalking", true);

			} else {
				if (anim.GetBool("isWalking")){
					myBody.velocity = new Vector2 (0, 0);
				}
				anim.SetBool ("isWalking", false);
				if (!isJumping) {
					myBody.velocity = new Vector2 (0, 0);
				}
			}

			if(hit.collider != null && Mathf.Abs(hit.normal.x) > 0.1f){

				Debug.Log ("Siamo inclinati");

				if (Mathf.Abs (hit.normal.y) < 0.8f) {
					Debug.Log ("Siamo molto inclinati");
					myBody.AddForce (new Vector2 (hit.normal.x * forceX / 4.0f, hit.normal.y * forceX));
				} else {
					myBody.AddForce (new Vector2 (hit.normal.x * forceX, hit.normal.y * forceX));
				}
	//			myBody.velocity =  new Vector2(horizontalInput * maxVelocity, myBody.velocity.y);
			} else{
				Debug.Log ("Siamo poco inclinati");
				myBody.AddForce (new Vector2(forceX, 0));
			}
		}

		if (Input.GetButton ("Jump") && !isJumping) 
		{
			isJumping = true;
			forceY = jumpForce;
			anim.SetBool ("isJumping", true);
			myBody.AddForce (new Vector2(0, forceY), ForceMode2D.Impulse);
		}


	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.name == "Ground")
		{
			isJumping = false;
			anim.SetBool ("isJumping", false);
			myBody.velocity = new Vector2 (0, 0);
		}

	}
		
}
