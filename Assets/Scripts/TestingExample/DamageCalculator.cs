using UnityEngine;

public class DamageCalculator : MonoBehaviour
{
    public IWeapon Weapon;
    [SerializeField]
    public float DamageMultiplier = 1;

    public float CalculateDamage()
    {
        return Weapon.BaseDamage * DamageMultiplier ;
    }
}
