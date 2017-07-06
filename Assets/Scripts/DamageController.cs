using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageController : MonoBehaviour
{

    public Animator anim;
    public float life;
    public float currentLife;


    // Use this for initialization
    void Start()
    {
        currentLife = life;
        anim = GetComponent<Animator>();

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
        }

        if (currentLife == 0 && !anim.GetBool("isDead"))
        {
            onDeath();
        }
    }

    private void onDeath()
    {
        anim.SetBool("isDead", true);
        GetComponent<PrisonerMovement>().enabled = false;
        
        if (transform.GetChild(0).childCount > 0)
        {
            //Destroy(transform.GetChild(0).GetChild(0).gameObject);
            transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }
    }
}
