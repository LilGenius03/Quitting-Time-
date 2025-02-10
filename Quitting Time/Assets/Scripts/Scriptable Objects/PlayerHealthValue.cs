using UnityEngine;
[CreateAssetMenu(menuName = "Player Health Value")]
public class PlayerHealthValue : ScriptableObject
{
   public float healthValue;
    public float damageValue;

    public void ResetHealth()
    {
        healthValue = 3f;
        damageValue = 1f;
    }
}
