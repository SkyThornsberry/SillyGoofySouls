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
    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("triggered");
        if (other.tag == "playerHurtbox")
        {
            tempDmg = other.GetComponent<PlayerHurtbox>().dmg;
            tempType = other.GetComponent<PlayerHurtbox>().type;
            Debug.Log("hit");
            Enemy.EnemyHurt(tempDmg, tempType);
        }

    }
}
