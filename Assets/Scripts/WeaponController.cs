using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{

    Camera cam;

    public float shotPower;
    public GameObject weaponBullet;

    private Transform bulletSpawnerTransform;
    private Transform pointerTransform;
    private Transform progressBarTransform;

    private bool isShooting = false;
    private Vector3 initialMousePosition;
    private SpriteRenderer progressBarRenderer;
    private Texture2D progressBarBaseTexture;
    private float barSpeed = 400f, barLength = 0f;

    void Awake()
    {
        cam = Camera.main;
    }

    // Use this for initialization
    void Start()
    {
        Transform spriteTransform = transform.GetChild(0);
        pointerTransform = spriteTransform.GetChild(0);
        bulletSpawnerTransform = spriteTransform.GetChild(1);
        progressBarTransform = bulletSpawnerTransform.GetChild(0);

        progressBarRenderer = progressBarTransform.GetComponent<SpriteRenderer>();
        progressBarBaseTexture = (Texture2D)Resources.Load("progress-bar");

        progressBarRenderer.sprite = UpdatedProgressSprite(0f);
    }

    // Update is called once per frame
    void Update()
    {
        // Getting mouse coordinates referred to the scene.
        float camDis = cam.transform.position.y - transform.position.y;
        Vector3 mouse = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camDis));

        // Check if weapon must flip.
        bool flipSprites = ((mouse.x - transform.position.x) < 0);

        // Getting the absolute Rotation angle based on mouse coords and pivot object coords.
        float AngleRad = Mathf.Atan2(mouse.y - transform.position.y, Mathf.Abs(mouse.x - transform.position.x));
        float AngleDeg = (180 / Mathf.PI) * AngleRad;

        bool isPlayerFlipped = (transform.parent.parent.transform.localScale.x < 0);

        float minAimAngle, maxAimAngle;

        if (isPlayerFlipped)
        {
            minAimAngle = -90f;
            maxAimAngle = 90f;
        }
        else
        {
            minAimAngle = 90f;
            maxAimAngle = 270f;
        }

        // Rotate the weapon dinamically.
        if (flipSprites)
        {
            if (AngleDeg >= minAimAngle && AngleDeg <= maxAimAngle)
            {
                //Flip the sprite.
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x) * (isPlayerFlipped ? -1 : 1), transform.localScale.y, transform.localScale.z);
                transform.rotation = Quaternion.Euler(0, 0, -AngleDeg);
                progressBarTransform.localScale = new Vector3(-Mathf.Abs(progressBarTransform.localScale.x), -Mathf.Abs(progressBarTransform.localScale.y), progressBarTransform.localScale.z);
            }

        }
        else
        {
            if ((AngleDeg + 180) >= minAimAngle && (AngleDeg + 180) <= maxAimAngle)
            {
                //Don't flip the sprite.
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * (isPlayerFlipped ? -1 : 1), transform.localScale.y, transform.localScale.z);
                transform.rotation = Quaternion.Euler(0, 0, AngleDeg);
                progressBarTransform.localScale = new Vector3(-Mathf.Abs(progressBarTransform.localScale.x), Mathf.Abs(progressBarTransform.localScale.y), progressBarTransform.localScale.z);
            }

        }

        if (!TurnManager.isTurnFinishing && transform.parent.parent.GetComponent<PrisonerBehavior>().isAlreadyShooted == false)
        {
            // Progress Bar changes on input.
            if (Input.GetMouseButtonDown(0))
            {
                initialMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(0))
            {

                //float barLength;
                //barLength = Mathf.Clamp (Input.mousePosition.x - initialMousePosition.x, 0f, baseTexture.width);
                UpdateBarLength();
                progressBarRenderer.sprite = UpdatedProgressSprite(barLength);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            ResetBarLenght();
        }
    }

    /// <summary>
    /// Shots the bullet.
    /// </summary>
    public void ShotBullet()
    {
        if (pointerTransform != null && bulletSpawnerTransform != null && transform.parent.parent.GetComponent<PrisonerBehavior>().isAlreadyShooted == false)
        {
            transform.parent.parent.GetComponent<PrisonerBehavior>().isAlreadyShooted = true;

            float AngleDeg = Mathf.Atan2(pointerTransform.position.y - bulletSpawnerTransform.position.y, pointerTransform.position.x - bulletSpawnerTransform.position.x);
            Vector2 newImpulse = new Vector2(Mathf.Cos(AngleDeg), Mathf.Sin(AngleDeg)) * shotPower * barLength / progressBarBaseTexture.width;

            // Spawn the bullet.
            GameObject projectile = Instantiate(weaponBullet, bulletSpawnerTransform.position, Quaternion.identity);

            // Add impulse to new bullet.
            projectile.GetComponent<Rigidbody2D>().AddForce(newImpulse, ForceMode2D.Impulse);

            // Set fine turno
            TurnManager.turnFinishTimeout = TurnManager.endTurnTimeoutSeconds;
        }
    }


    // PROGRESS BAR METHODS
    private Sprite UpdatedProgressSprite(float barLength)
    {
        return Sprite.Create(progressBarBaseTexture, new Rect(0f, 0f, barLength, progressBarBaseTexture.height), new Vector2(0f, 0.5f), 250f);
    }

    public void UpdateBarLength()
    {
        barLength += barSpeed * Time.deltaTime;
        barLength = Mathf.Clamp(barLength, 0f, progressBarBaseTexture.width);
    }

    public void ResetBarLenght()
    {
        barLength = 0;
        progressBarRenderer.sprite = UpdatedProgressSprite(barLength);
    }
}
