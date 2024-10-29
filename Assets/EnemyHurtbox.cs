using UnityEngine;

public class Hurtbox : MonoBehaviour
{



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
