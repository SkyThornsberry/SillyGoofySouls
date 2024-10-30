using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;
using UnityEngine.UI;


public class EnemyHitbox : MonoBehaviour
{
    public EnemyBase Enemy;
    public float enemyHealth;
    float tempDmg;
    string tempType;
    bool invincible;
    float iFrames = .5f;

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("triggered");
        if (other.tag == "playerHurtbox" & !invincible)
        {
            tempDmg = other.GetComponent<PlayerHurtbox>().dmg;
            tempType = other.GetComponent<PlayerHurtbox>().type;
            iFrames = other.GetComponent<PlayerHurtbox>().iFrames;
            Debug.Log("hit");
            Enemy.EnemyHurt(tempDmg, tempType);
            invincible = true;
            Invoke("setNotInvincible", iFrames);
        }

    }

    void setNotInvincible()
    {
        invincible = false;
    }

}


