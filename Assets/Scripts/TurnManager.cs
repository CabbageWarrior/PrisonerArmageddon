using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public GameObject mainCamera;

    
    Transform currentActivePlayerTransform;
    
    //// Use this for initialization
    //void Start()
    //{
    //
    //}

    // Update is called once per frame
    void Update()
    {
        if (currentActivePlayerTransform != null)
        {
            Vector3 newCameraPosition = new Vector3(currentActivePlayerTransform.position.x, currentActivePlayerTransform.position.y, mainCamera.transform.position.z);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, newCameraPosition, 5f * Time.deltaTime);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            SetActivePlayer(Random.Range(1, 2), Random.Range(0, 3));
        }
    }

    public void SetActivePlayer(int teamNumber, int teamElementNumber)
    {
        bool[] currentArray;

        if (teamNumber == 1)
        {
            currentArray = PrisonerBehavior.Team1Prisoners;
        }
        else
        {
            currentArray = PrisonerBehavior.Team2Prisoners;
        }

        PrisonerBehavior loopPrisonerBehavior;
        foreach (GameObject prisoner in GameObject.FindGameObjectsWithTag("Player"))
        {
            loopPrisonerBehavior = prisoner.GetComponent<PrisonerBehavior>();
            if (loopPrisonerBehavior.teamNumber == teamNumber && loopPrisonerBehavior.teamElementNumber == teamElementNumber)
            {
                currentActivePlayerTransform = prisoner.transform;
            }
        }
    }
}
