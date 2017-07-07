using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{

    private int teamNumber;
    private CircleCollider2D circleCollider2D;
    RuntimeAnimatorController team1Animator, team2Animator;

    public GameObject gameManager, playerPrefab, ground;
    public LayerMask whatIsGround;

    //// Use this for initialization
    void Start()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        team1Animator = (RuntimeAnimatorController)Resources.Load("Animators/Team1Prisoner");
        team2Animator = (RuntimeAnimatorController)Resources.Load("Animators/Team2Prisoner");
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void SetSpawnerPosition(Vector3 spawnPosition)
    {
        transform.position = spawnPosition;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        SetSpawnerPosition(transform.position + new Vector3(0, 10, 0));
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        circleCollider2D.enabled = false;

        GameObject currentPlayer = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        currentPlayer.GetComponent<PrisonerBehavior>().gameManager = gameManager;
        if (teamNumber == 1)
        {
            currentPlayer.GetComponent<Animator>().runtimeAnimatorController = team1Animator;
        }
        else
        {
            currentPlayer.GetComponent<Animator>().runtimeAnimatorController = team2Animator;
        }
    }

    //public void CreateNewPrisoner(Vector3 spawnPosition, int teamNumber)
    public void CreateNewPrisoner(float xMin, float xMax, float yMin, float yMax, int teamNumber)
    {
        Vector3 spawnPosition;
        RaycastHit2D availablePositionFloor;
        Collider2D[] circleCollisions;

        do
        {
            spawnPosition = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), 0);

            //// Floor
            //Debug.DrawRay(new Vector3(spawnPosition.x, spawnPosition.y, 0), Vector3.down * 50f, Color.red);
            //// Circle
            //Debug.DrawRay(new Vector3(spawnPosition.x, spawnPosition.y, 0), Vector3.up * (playerPrefab.transform.localScale.y / 2 + 0.5f), Color.yellow);
            //Debug.DrawRay(new Vector3(spawnPosition.x, spawnPosition.y, 0), Vector3.down * (playerPrefab.transform.localScale.y / 2 + 0.5f), Color.yellow);
            //Debug.DrawRay(new Vector3(spawnPosition.x, spawnPosition.y, 0), Vector3.left * (playerPrefab.transform.localScale.y / 2 + 0.5f), Color.yellow);
            //Debug.DrawRay(new Vector3(spawnPosition.x, spawnPosition.y, 0), Vector3.right * (playerPrefab.transform.localScale.y / 2 + 0.5f), Color.yellow);

            availablePositionFloor = Physics2D.Raycast(spawnPosition, Vector2.down, 50f, whatIsGround);
            circleCollisions = Physics2D.OverlapCircleAll(spawnPosition, playerPrefab.transform.localScale.y / 2 + 0.5f, whatIsGround);
        } while (availablePositionFloor.collider == null || circleCollisions.Length > 0);

        this.teamNumber = teamNumber;
        SetSpawnerPosition(spawnPosition);
        circleCollider2D.enabled = true;

        if (!circleCollider2D.IsTouching(ground.GetComponent<PolygonCollider2D>()))
        {
            SpawnPlayer();
        }
    }

}
