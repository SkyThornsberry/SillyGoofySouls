using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;
using UnityEngine.UI;

public class EnemyHurtbox : MonoBehaviour
{

    public float dmg;
    public EnemyBase Enemy;
    public string type;
    public float iFrames = .25f;
    void Update()
    {
        dmg = Enemy.attackDamage[Enemy.curAttack];
        type = Enemy.attackType[Enemy.curAttack];
    }


    private void OnTriggerEnter(Collider other)
    {


    }
}
