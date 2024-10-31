using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;
using UnityEngine.UI;
public class EnemyBase : MonoBehaviour
{
    public GameObject self;
    public float[] attackDamage;
    public string[] attackType;
    public float maxHealth;
    public float health;
    public EnemyType1 enemyType;
    public int curAttack =0;
    public GameObject healthBar;
    public GameObject healthLossBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackDamage = enemyType.DmgVals;
        attackType = enemyType.TypeVals;
        maxHealth =enemyType.health;
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnemyHurt(float damage, string Type)
    {
        //damage calcs
        if (health - damage <= 0)
        {
            health = 0;
            EnemyDeath();
        }
        else
            health -= damage;
        Debug.Log("Enemy Hurt: " + damage + ", " + Type);


        updateHealthBar();
        Invoke("updateHealthLossBar", 2);
    }

    void updateHealthBar()
    {
        //set health bar
        healthBar.transform.localScale = new Vector3(health * 1f / maxHealth, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }

    void updateHealthLossBar()
    {
        //set healthloss bar, the yellow bar behind the read health bar which remains after health is loss to display the amount lost
        healthLossBar.transform.localScale = new Vector3(health * 1f / maxHealth, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }

    public void EnemyDeath()
    {
        //calculate drops, play death animation, and delete enemy
        GameObject.Destroy(self);
    }
}
