using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;
using UnityEngine.UI;
public class EnemyBase : MonoBehaviour
{
    public float[] attackDamage;
    public string[] attackType;
    public float health;
    public EnemyType1 enemyType;
    public int curAttack =0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackDamage = enemyType.DmgVals;
        attackType = enemyType.TypeVals;
        health =enemyType.health;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnemyHurt(float damage, string Type)
    {
        //health -= damage;
        Debug.Log("Enemy Hurt: " + damage + ", " + Type); 
    }
}
