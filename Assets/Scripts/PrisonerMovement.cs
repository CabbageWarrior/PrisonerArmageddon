using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrisonerMovement : MonoBehaviour {

	public float speed = 12f, maxVelocity = 3f, jumpForce = 4f;
	public LayerMask whatIsGround;
    //public Text text1, text2;

    //	private int whatIsGround = 1;
    private Rigidbody2D myBody;
	[SerializeField]
	private bool isOnGround = false;
	private Animator anim;
	private float planeParallelX, planeParallelY, prisonerWidth, prisonerHeight;
    private enum raysHitCondition {TwoHit, FrontHit, BackHit, NoHit};
    private PrisonerBehavior behavior;

    void Awake()
	{
		myBody = GetComponent<Rigidbody2D> ();
        prisonerWidth = transform.localScale.x;
		prisonerHeight = transform.localScale.y;
        anim = GetComponent<Animator> ();
        behavior = GetComponent<PrisonerBehavior>();
	}

	void Start ()
	{
		
	}
	
	void Update () 
	{
        if (behavior.isActive == true)
        { 
		    PrisonerMoveKeyboard ();
        }
    }

	void PrisonerMoveKeyboard()
	{
        float velocityX = 0f, velocityY = 0f;
        float rayLength = 1.5f, rayShift = prisonerWidth / 2.5f;
        float slopeAngle;

        float distanceBack, distanceFront, distanceDifference;
        float vel = myBody.velocity.x;
        Vector2 backRayTransform = transform.position, frontRayTransform = transform.position;

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        int raycastsCollisionsState;
        
        if (isOnGround)
        {   
            if (horizontalInput > 0)
            {
                vel = maxVelocity;
                if (transform.localScale.x < 0)
                {
                    ChangeDirection();
                }
                anim.SetBool("isWalking", true);
                backRayTransform = new Vector2(transform.position.x - rayShift, transform.position.y);
                frontRayTransform = new Vector2(transform.position.x + rayShift, transform.position.y);
            }
            else if (horizontalInput < 0)
            {
                vel = -maxVelocity;
                if (transform.localScale.x > 0)
                {
                    ChangeDirection();
                }
                anim.SetBool("isWalking", true);
                backRayTransform = new Vector2(transform.position.x + rayShift, transform.position.y);
                frontRayTransform = new Vector2(transform.position.x - rayShift, transform.position.y);
            }
            else
            {
                vel = 0f;
                if (anim.GetBool("isWalking"))
                {
                    myBody.velocity = new Vector2(0, 0);
                }
                anim.SetBool("isWalking", false);
                if (isOnGround)
                {
                    myBody.velocity = new Vector2(0, 0);
                }
                backRayTransform = new Vector2(transform.position.x - rayShift, transform.position.y);
                frontRayTransform = new Vector2(transform.position.x + rayShift, transform.position.y);
            }

            if (vel != 0)
            {
                Debug.DrawRay(new Vector3(backRayTransform.x, backRayTransform.y, 0), Vector3.down * rayLength, Color.green);
                Debug.DrawRay(new Vector3(frontRayTransform.x, frontRayTransform.y, 0), Vector3.down * rayLength);
           
                RaycastHit2D hitDownBack = Physics2D.Raycast(backRayTransform, Vector2.down, rayLength, whatIsGround);
                RaycastHit2D hitDownFront = Physics2D.Raycast(frontRayTransform, Vector2.down, rayLength, whatIsGround);
                distanceBack = hitDownBack.distance;
                distanceFront = hitDownFront.distance;
                distanceDifference = distanceBack - distanceFront;

                // Checks the inclination of the terrain and assign the right velocity direction

                if (hitDownBack.collider != null && hitDownFront.collider != null)
                {
                    raycastsCollisionsState = (int)raysHitCondition.TwoHit;
                }
                else if (hitDownBack.collider != null)
                {
                    raycastsCollisionsState = (int)raysHitCondition.BackHit;
                }
                else if (hitDownFront.collider != null)
                {
                    raycastsCollisionsState = (int)raysHitCondition.FrontHit;
                }
                else
                {
                    raycastsCollisionsState = (int)raysHitCondition.NoHit;
                }


                switch (raycastsCollisionsState)
                {
                    case (int)raysHitCondition.TwoHit:

                        slopeAngle = Mathf.Rad2Deg * Mathf.Atan(distanceDifference / prisonerWidth);

                        if (horizontalInput < 0)
                        {
                            velocityX = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * vel;
                            velocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * -vel;
                        }
                        else
                        {
                            velocityX = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * vel;
                            velocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * vel;
                        }

                        if (Mathf.Min(distanceBack, distanceFront) > prisonerHeight / 2.0f)
                        {
                            velocityY -= 1;
                        }

                        break;

                    case (int)raysHitCondition.FrontHit:

                        
                        velocityX = vel;
                        velocityY = 0f;
                       
                        if (Mathf.Min(distanceBack, distanceFront) > prisonerHeight / 2.0f)
                        {
                            velocityY -= 1f;
                        }

                        break;

                    case (int)raysHitCondition.BackHit:

                        velocityX = vel;
                        velocityY = -5f;

                        break;

                    default:

                        velocityX = 0f;
                        velocityY = 0f;
                        isOnGround = false;
                        break;
                }

				
				myBody.velocity = new Vector2(velocityX, velocityY);

            }

            else
            {
                myBody.velocity = new Vector2(0, 0);
            }

            if (Input.GetButton("Jump") && isOnGround)
            {
                Jump();
            }
        }

	}

    void ChangeDirection()
    {
        myBody.velocity = new Vector2(0, 0);
        Vector3 temp = transform.localScale;
        temp.x *= -1;
        transform.localScale = temp;
    }

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Ground")
		{
			isOnGround = true;
			anim.SetBool ("isJumping", false);
			myBody.velocity = new Vector2 (0, 0);
		}

	}

	void Jump()
	{
		isOnGround = false;
		myBody.velocity = new Vector2(0, 0);
		anim.SetBool("isJumping", true);
		if (transform.localScale.x > 0f) 
		{
			myBody.AddForce (new Vector2 (jumpForce, jumpForce), ForceMode2D.Impulse);
		} 
		else 
		{
			myBody.AddForce (new Vector2 (-jumpForce, jumpForce), ForceMode2D.Impulse);
		}
	}
		
}
