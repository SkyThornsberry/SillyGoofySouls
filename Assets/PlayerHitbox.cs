using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerHitbox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Player Player;
    float tempDmg;
    string tempType;
    private void OnTriggerEnter(Collider other)
    {
        
        //Debug.Log("triggered");
        if (other.tag == "EnemyHurtbox")
        {

            tempDmg = other.GetComponent<EnemyHurtbox>().dmg;
            tempType = other.GetComponent<EnemyHurtbox>().type;

            Debug.Log("Player hurt, " + tempDmg + ", " + tempType);
            Player.playerHurt(tempDmg, tempType);
            
        }

    }

}
