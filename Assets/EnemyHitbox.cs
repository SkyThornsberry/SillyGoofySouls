using UnityEngine;




public class EnemyHitbox : MonoBehaviour
{
    public Player Player;
    float damage = 50f;
    string damageType = "physical";

    public float enemyHealth;
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
