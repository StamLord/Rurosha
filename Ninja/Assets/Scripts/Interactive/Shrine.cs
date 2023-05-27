using UnityEngine;

public class Shrine : Usable
{
    [SerializeField] int karmaAmount = 5;
    [SerializeField] int donationAmount = 0;

    public override void Use(Interactor interactor)
    {
        base.Use(interactor);

        // Free prayer
        if(donationAmount < 1)
        {
            interactor.AddKarma(karmaAmount);
            return;
        }
        
        // Donation based prayer
        if(interactor.DepleteMoney(donationAmount))
            interactor.AddKarma(karmaAmount);
    }
}
