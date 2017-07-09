using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageController : MonoBehaviour
{

    public Animator anim;
    public float life;
    public float currentLife;

    private PrisonerMovement prisonerMovement;
    private PrisonerBehavior prisonerBehavior;
    private TextMesh displayedLife;

    private Color team1TextColor = new Color(255, 138, 0);
    private Color team2TextColor = new Color(0, 255, 12);
    private int teamNbr = 0;

    // Use this for initialization
    void Start()
    {
        displayedLife = this.transform.Find("HealthScore").GetComponent<TextMesh>();
        currentLife = life;
        displayedLife.text = currentLife.ToString();

        teamNbr = this.GetComponent<PrisonerBehavior>().teamNumber;

        if (teamNbr == 1)
            displayedLife.color = team1TextColor;
        else
            displayedLife.color = team2TextColor;


        anim = GetComponent<Animator>();
        prisonerMovement = GetComponent<PrisonerMovement>();
        prisonerBehavior = GetComponent<PrisonerBehavior>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "Bullet")
    //    {
    //        Destroy(collision.gameObject);
    //        currentLife -= collision.gameObject.GetComponentInParent<BulletController>().bulletDamage;
    //    }
    //}

    public void SubtractLifePoints(float lpNumber)
    {
        currentLife -= lpNumber;

        if (currentLife < 0)
        {
            currentLife = 0;
            displayedLife.text = currentLife.ToString();
        }
        else
        {
            displayedLife.text = currentLife.ToString();
        }

        if (currentLife == 0 && !anim.GetBool("isDead"))
        {
            onDeath();
        }
    }

    private void onDeath()
    {
        anim.SetBool("isDead", true);
        prisonerMovement.enabled = false;

        if (prisonerBehavior.teamNumber == 1)
        {
            PrisonerBehavior.Team1Prisoners[prisonerBehavior.teamElementNumber] = false;
        }
        else
        {
            PrisonerBehavior.Team2Prisoners[prisonerBehavior.teamElementNumber] = false;
        }

        if (transform.GetChild(0).childCount > 0)
        {
            Destroy(transform.GetChild(0).GetChild(0).gameObject);
            transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }
    }
}
