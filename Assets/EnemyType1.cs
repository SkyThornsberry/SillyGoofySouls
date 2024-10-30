using UnityEngine;

public class EnemyType1 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float[] DmgVals =  {50f, 100f, 150f};
    public string[] TypeVals = { "physical", "magic", "fire" };
    public float health = 400;
}
