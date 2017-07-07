using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public GameObject mainCamera;
    public static float endTurnTimeoutSeconds = 5;


    public static float turnFinishTimeout = -1;
    public static bool isTurnFinishing = false;
    
    
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

        if (turnFinishTimeout > 0)
        {
            isTurnFinishing = true;
            turnFinishTimeout -= Time.deltaTime;
        }
        else if (isTurnFinishing == true)
        {
            int nextTeam = (PrisonerBehavior.currentTeam == 1 ? 2 : 1);
            SetActivePlayer(nextTeam, PrisonerBehavior.GetNext(nextTeam));
            isTurnFinishing = false;
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
                PrisonerBehavior.currentTeam = teamNumber;
                prisoner.GetComponent<PrisonerBehavior>().isAlreadyShooted = false;
                if (teamNumber == 1)
                {
                    PrisonerBehavior.team1CurrentPlayer = prisoner.GetComponent<PrisonerBehavior>().teamElementNumber;
                }
                else
                {
                    PrisonerBehavior.team2CurrentPlayer = prisoner.GetComponent<PrisonerBehavior>().teamElementNumber;
                }
            }
        }
    }
}
