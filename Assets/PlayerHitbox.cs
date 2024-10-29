using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class OnTriggerCollider : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Player Player;
    float damage = 50f;
    string damageType = "physical";
    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("triggered");
        if (other.tag == "enemy")
        {
            Debug.Log("hit");
            Player.playerHurt(damage, damageType);
        }

    }

}
