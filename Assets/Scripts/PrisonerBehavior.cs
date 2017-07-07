using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonerBehavior : MonoBehaviour
{

    public GameObject gameManager;
    public int teamNumber, teamElementNumber;
    public static bool[] Team1Prisoners, Team2Prisoners;

    private Transform weaponAnchorTransform;

    private int team1CurrentPlayer, team2CurrentPlayer, playersPerTeam;

    private void Awake()
    {
        playersPerTeam = GameObject.Find("PrisonerSpawner").GetComponent<SpawnerController>().playersPerTeam;

        Team1Prisoners = new bool[playersPerTeam];
        Team2Prisoners = new bool[playersPerTeam];
    }

    // Use this for initialization
    void Start()
    {
        weaponAnchorTransform = transform.Find("WeaponAnchor").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            gameManager.GetComponent<MenuManager>().ToggleWeaponMenu(this.gameObject);
        }

        if (Input.GetMouseButtonUp(0))
        {
            foreach (Transform child in weaponAnchorTransform)
            {
                if (child.tag == "Weapon")
                {
                    child.GetComponent<WeaponController>().ShotBullet();
                }
            }
        }
    }

    public void SetPrisonerInfos(int teamNumber, int teamElementNumber)
    {
        this.teamNumber = teamNumber;
        this.teamElementNumber = teamElementNumber;

        if (teamNumber == 1)
        {
            Team1Prisoners[teamElementNumber] = true;
        }
        else
        {
            Team2Prisoners[teamElementNumber] = true;
        }
    }

    int GetNext(int teamNumber)
    {
        bool[] currentArray;
        int prevCharacter;
        bool isTeamWithAliveElements = false;

        // Inizializzo le variabili generiche con i riferimenti della squadra corretta.
        if (teamNumber == 1)
        {
            currentArray = Team1Prisoners;
            prevCharacter = team1CurrentPlayer;
        }
        else
        {
            currentArray = Team2Prisoners;
            prevCharacter = team2CurrentPlayer;
        }

        // Verifico che per quella squadra esista almeno un giocatore in vita.
        foreach (bool item in currentArray)
        {
            if (item == true)
            {
                isTeamWithAliveElements = true;
                break;
            }
        }

        // Se sono tutti morti restituisco -1.
        if (!isTeamWithAliveElements) {
            return -1;
        }

        // Altrimenti cerco il primo vivo dopo l'ultimo utilizzato di questa squadra.

        prevCharacter++;
        if (prevCharacter > playersPerTeam - 1)
        {
            prevCharacter = 0;
        }

        // Looppo finché non ne trovo uno vivo.
        while (currentArray[prevCharacter] == false)
        {
            prevCharacter++;
        }

        // prevCharacter contains the correct next value.
        // Sovrascrivo il current player della squadra.
        if (teamNumber == 1)
        {
            team1CurrentPlayer = prevCharacter;
        }
        else
        {
            team2CurrentPlayer = prevCharacter;
        }

        // Restituisco il valore corretto al chiamante.
        return prevCharacter;
    }
}
