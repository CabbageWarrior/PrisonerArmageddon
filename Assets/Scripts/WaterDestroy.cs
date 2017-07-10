using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDestroy : MonoBehaviour {

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;
        if (other.tag == "Player")
        {
            // Pasare il turno 
            // Togliere dalla rotazione della telecamera

            Destroy(other);
        }
        else
        {
            Destroy(other);
        }
        
    }
}
