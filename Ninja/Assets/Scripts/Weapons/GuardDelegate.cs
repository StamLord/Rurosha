using UnityEngine;

public class GuardDelegate : MonoBehaviour
{
    [SerializeField] private WeaponObject weapon;

    public void PerfectGuard(Rigidbody target)
    {
        weapon.PerfectGuard(target);
    }

    public void Guard(Rigidbody target)
    {
        weapon.Guard(target);
    }


}
