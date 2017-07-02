using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrisonerMovement : MonoBehaviour {

	public float speed = 12f, maxVelocity = 3f, jumpForce = 4f, maxClimbAngle=60.0f;
	public LayerMask whatIsGround;
    public Text text1, text2;

    //	private int whatIsGround = 1;
    private Rigidbody2D myBody;
	[SerializeField]
	private bool isJumping = false;
	private Animator anim;
    private float planeParallelX, planeParallelY, prisonerWidth;

	void Awake()
	{
		myBody = GetComponent<Rigidbody2D> ();
        prisonerWidth = transform.localScale.x;
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
		//float forceX = 0f;
		float forceY = 0f;
        float velocityX = 0f;
        float velocityY = 0f;
        float slopeAngle;

        float distanceBack, distanceFront, distanceDifference;
        // float vel = Mathf.Sqrt (Mathf.Pow(myBody.velocity.x, 2.0f) + Mathf.Pow(myBody.velocity.y, 2.0f));
        float vel = myBody.velocity.x;
        Vector2 backRayTransform, frontRayTransform;

        float horizontalInput = Input.GetAxisRaw ("Horizontal");

        if (!isJumping)
        {   
            if (horizontalInput > 0)
            {
                vel = maxVelocity;
                if (transform.localScale.x < 0)
                {
                    ChangeDirection();
                }
                anim.SetBool("isWalking", true);
                backRayTransform = new Vector2(transform.position.x - prisonerWidth/2.0f, transform.position.y);
                frontRayTransform = new Vector2(transform.position.x + prisonerWidth / 2.0f, transform.position.y);
            }
            else if (horizontalInput < 0)
            {
                vel = -maxVelocity;
                if (transform.localScale.x > 0)
                {
                    ChangeDirection();
                }
                anim.SetBool("isWalking", true);
                backRayTransform = new Vector2(transform.position.x + prisonerWidth / 2.0f, transform.position.y);
                frontRayTransform = new Vector2(transform.position.x - prisonerWidth / 2.0f, transform.position.y);
            }
            else
            {
                vel = 0f;
                if (anim.GetBool("isWalking"))
                {
                    myBody.velocity = new Vector2(0, 0);
                }
                anim.SetBool("isWalking", false);
                if (!isJumping)
                {
                    myBody.velocity = new Vector2(0, 0);
                }
                backRayTransform = new Vector2(transform.position.x - prisonerWidth / 2.0f, transform.position.y);
                frontRayTransform = new Vector2(transform.position.x + prisonerWidth / 2.0f, transform.position.y);
            }

            if (vel != 0)
            {
                Debug.DrawRay(new Vector3(transform.position.x, transform.position.y, 0), new Vector3(2f * Mathf.Sign(horizontalInput), 0, 0));
                Debug.DrawRay(new Vector3(backRayTransform.x, backRayTransform.y, 0), Vector3.down * 3f, Color.green);
                Debug.DrawRay(new Vector3(frontRayTransform.x, frontRayTransform.y, 0), Vector3.down * 3f);

                RaycastHit2D hitFront = Physics2D.Raycast(transform.position, new Vector2((int)Mathf.Sign(horizontalInput), 0), 2f, whatIsGround);
                RaycastHit2D hitDownBack = Physics2D.Raycast(backRayTransform, Vector2.down, 3f, whatIsGround);
                RaycastHit2D hitDownFront = Physics2D.Raycast(frontRayTransform, Vector2.down, 3f, whatIsGround);
                distanceBack = hitDownBack.distance;
                distanceFront = hitDownFront.distance;
                distanceDifference = distanceBack - distanceFront;
                slopeAngle = Mathf.Rad2Deg * Mathf.Atan(distanceDifference/prisonerWidth);

                // Checks the inclination of the terrain
                if (hitFront.collider != null && (hitDownBack.collider != null || hitDownFront.collider != null))
                {   

                    if (slopeAngle <= maxClimbAngle)
                    {
                        if ((horizontalInput < 0 && distanceBack < distanceFront) || (horizontalInput > 0 && distanceBack > distanceFront))
                        {
                            velocityX = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * vel;
                            velocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * vel;
                            myBody.velocity = new Vector2(velocityX, velocityY);
                        }
                        else
                        {
                            velocityX = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * vel;
                            velocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * -vel;
                            myBody.velocity = new Vector2(velocityX, velocityY);
                        }
                    }
                    else
                    {
                        myBody.velocity = new Vector2(vel, 0);
                    }
                }

                else if (hitFront.collider == null && (hitDownBack.collider != null || hitDownFront.collider != null))
                {
                    
                    if (horizontalInput < 0 && distanceBack < distanceFront)
                    {
                        velocityX = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * vel;
                        velocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * -vel;
                        myBody.velocity = new Vector2(velocityX, velocityY);
                    }
                    else
                    {
                        velocityX = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * vel;
                        velocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * vel;
                        myBody.velocity = new Vector2(velocityX, velocityY);
                    }
                }
                else
                {
                    myBody.velocity = new Vector2(vel, 0);
                }


            }

            else
            {
                myBody.velocity = new Vector2(0, 0);
            }

            if (Input.GetButton("Jump") && !isJumping)
            {
                isJumping = true;
                forceY = jumpForce;
                anim.SetBool("isJumping", true);
                myBody.AddForce(new Vector2(0, forceY), ForceMode2D.Impulse);
            }

            //text1.text = velocityX.ToString();
            //text2.text = velocityY.ToString();

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
			isJumping = false;
			anim.SetBool ("isJumping", false);
			myBody.velocity = new Vector2 (0, 0);
		}

	}
		
}
