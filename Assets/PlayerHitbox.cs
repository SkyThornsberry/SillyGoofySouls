using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerHitbox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Player Player;
    float tempDmg;
    string tempType;
    float iFrames = .5f;
    bool invincible = false;
    private void OnTriggerEnter(Collider other)
    {
        
        //Debug.Log("triggered");
        if (other.tag == "EnemyHurtbox" & !invincible)
        {

            tempDmg = other.GetComponent<EnemyHurtbox>().dmg;
            tempType = other.GetComponent<EnemyHurtbox>().type;
            iFrames = other.GetComponent<EnemyHurtbox>().iFrames;

            Debug.Log("Player hurt, " + tempDmg + ", " + tempType);
            Player.playerHurt(tempDmg, tempType);
            invincible = true;
            Invoke("setNotInvincible", iFrames);
        }

    }

    void setNotInvincible()
    {
        invincible = false;
    }

}
