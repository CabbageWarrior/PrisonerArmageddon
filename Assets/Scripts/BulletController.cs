using UnityEngine;

public class BulletController : MonoBehaviour
{
    /// <summary>
    /// Value of bullet damage.
    /// </summary>
    public float bulletDamage;

    /// <summary>
    /// Global reference to the Rigidbody2D.
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// Transform component of the Sprite into the Bullet object.
    /// </summary>
    public Transform bulletSpriteTransform;

    /// <summary>
    /// Bool that says whether or not to update the rotation of the GameObject Sprite based on the trajectory of the bullet. This bool is to say that after the bullet collides with some other body, the rotation of the bullet should no longer be updated based on the trajectory.
    /// </summary>
    private bool updateAngle = true;

    /// <summary>
    /// Global reference to the BulletSmoke object, that is the particle tail of the bullet.
    /// </summary>
    public GameObject bulletSmoke;

    /// <summary>
    /// Global reference to the circle collider of destruction.
    /// </summary>
    public CircleCollider2D destructionCircle;

    /// <summary>
    /// Global reference to the ground.
    /// </summary>
    public static GroundController groundController;

    /// <summary>
    /// Start method.
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    //rb.velocity = new Vector2(5f, 10f);

    /// <summary>
    /// Update method.
    /// </summary>
    void Update()
    {

        if (updateAngle)
        {
            // Direction angle given by velocity.
            Vector2 dir = new Vector2(rb.velocity.x, rb.velocity.y);

            dir.Normalize();
            float angle = Mathf.Asin(dir.y) * Mathf.Rad2Deg;
            if (dir.x < 0f)
            {
                angle = 180 - angle;
            }

            //Debug.Log("angle = " + angle);

            // Update of the Euler angles of the Sprite contained into the Bullet object.
            bulletSpriteTransform.localEulerAngles = new Vector3(0f, 0f, angle); // + 45f);
        }
    }

    /// <summary>
    /// Catches every collision 2D.
    /// </summary>
    /// <param name="coll">Collision Event arguments.</param>
    private void OnCollisionEnter2D(Collision2D coll)
    {

        // When the bullet collides with another object, the bullet will explode,
        // but only if the collided object has some specific attributes.
        switch (coll.collider.tag)
        {
            case "Ground":
                groundController.DestroyGround(destructionCircle);

                updateAngle = false;
                bulletSmoke.SetActive(false);
                Destroy(gameObject);
                break;
            case "Player":
                groundController.DestroyGround(destructionCircle);

                updateAngle = false;
                bulletSmoke.SetActive(false);
                Destroy(gameObject);

                coll.gameObject.GetComponent<DamageController>().SubtractLifePoints(bulletDamage);
                break;
        }

    }
}