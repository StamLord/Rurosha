using UnityEngine;

public class KarmaPopup : UIPopup
{
    [SerializeField] private CharacterStats stats;

    protected override void Initialize()
    {
        stats.OnKarmaUpdate += StartPopup;
    }
}
