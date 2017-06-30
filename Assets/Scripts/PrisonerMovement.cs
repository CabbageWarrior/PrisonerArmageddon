using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrisonerMovement : MonoBehaviour {

	public float speed = 12f, maxVelocity = 3f, jumpForce = 4f;
	public LayerMask whatIsGround;
    public Text textForceX, textForceY;

//	private int whatIsGround = 1;
	private Rigidbody2D myBody;
	[SerializeField]
	private bool isJumping = false;
	private Animator anim;
    private float planeParallelX, planeParallelY;

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
        float velocityX = 0f;
        float velocityY = 0f;
        // float vel = Mathf.Sqrt (Mathf.Pow(myBody.velocity.x, 2.0f) + Mathf.Pow(myBody.velocity.y, 2.0f));
        float vel = myBody.velocity.x;

        float horizontalInput = Input.GetAxisRaw ("Horizontal");

		if (!isJumping) {

			RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 1f, whatIsGround);

			if (horizontalInput > 0) {
                if (vel < maxVelocity)
                {
                    forceX = speed;
                }

                // vel = maxVelocity;

                if (transform.localScale.x < 0) {
					myBody.velocity = new Vector2 (0, 0);
					Vector3 temp = transform.localScale;
					temp.x *= -1;
					transform.localScale = temp;
				} 
					
				anim.SetBool ("isWalking", true);

			} else if (horizontalInput < 0) {

                if (vel < maxVelocity)
                {
                    forceX = -speed;
                //    velocityX = -maxVelocity;
                }

                // vel = maxVelocity;

                if (transform.localScale.x > 0) {
					myBody.velocity = new Vector2 (0, 0);
					Vector3 temp = transform.localScale;
					temp.x *= -1;
					transform.localScale = temp;
				} 

				anim.SetBool ("isWalking", true);

			} else {
                vel = 0f;

                if (anim.GetBool("isWalking")){
					myBody.velocity = new Vector2 (0, 0);
				}
				anim.SetBool ("isWalking", false);
				if (!isJumping) {
					myBody.velocity = new Vector2 (0, 0);
				}
			}

            /*
            // Checks the inclination of the terrain
			if(hit.collider != null && Mathf.Abs(hit.normal.x) > 0.1f){

                if (Mathf.Sign(horizontalInput) == Mathf.Sign(hit.normal.x))
                {
                    velocityX = Mathf.Sign(horizontalInput) * Mathf.Abs((hit.normal.x * vel));
                    velocityY = Mathf.Abs((hit.normal.y * vel));
                    myBody.velocity = new Vector2(velocityX, -velocityY);
                    textForceX.text = velocityX.ToString();
                    textForceY.text = velocityY.ToString();
                }
                else
                {
                    velocityX = Mathf.Sign(horizontalInput) * Mathf.Abs((hit.normal.x * vel));
                    velocityY = -Mathf.Abs((hit.normal.y * vel));
                    myBody.velocity = new Vector2(velocityX, velocityY);
                    textForceX.text = velocityX.ToString();
                    textForceY.text = velocityY.ToString();
                }
                */

            /*
            // Solution using forces, not really working
            if (hit.collider != null && Mathf.Abs(hit.normal.x) > 0.1f)
            {
                if (Mathf.Sign(forceX) == Mathf.Sign(hit.normal.x))
                {
                    planeParallelX = Mathf.Sign(forceX) * (hit.normal.x * forceX);
                    myBody.AddForce(new Vector2(planeParallelX, 0));
                    textForceX.text = planeParallelX.ToString();
                    textForceY.text = "0";        

                }
                else
                {
                    planeParallelX = Mathf.Sign(forceX) * Mathf.Abs((hit.normal.x * forceX));
                    planeParallelY = hit.normal.y * forceX;
                    myBody.AddForce(new Vector2(planeParallelX, planeParallelY));
                    textForceX.text = planeParallelX.ToString();
                    textForceY.text = planeParallelY.ToString();
                }

                }

            else {
                // myBody.AddForce (new Vector2(forceX, 0));
                myBody.velocity = new Vector2(Mathf.Sign(horizontalInput) * velocityX, 0);
            } */

            myBody.AddForce(new Vector2(forceX, 0));
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
