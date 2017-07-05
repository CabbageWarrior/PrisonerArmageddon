using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageController : MonoBehaviour {

    public Animator anim;
    public float life = 10;
    public float currentLife;
    

    // Use this for initialization
    void Start() {
        currentLife = life;
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (currentLife <= 0 && !anim.GetBool("isDead"))
        {         
            onDeath();
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            Debug.Log("ho colliso");
            Destroy(other.gameObject);
            currentLife -= other.gameObject.GetComponent<BulletController>().bulletDamage;
        }
    }

    private void onDeath()
    {
        anim.SetBool("isDead", true);
    }

}
