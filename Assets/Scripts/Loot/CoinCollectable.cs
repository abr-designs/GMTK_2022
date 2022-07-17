using System;

public class CoinCollectable : CollectableBase
{
    public static Action OnCoinCollected;
    
    public override void Collect(in Dice_Prototype collectedBy)
    {
        if (Movement.IsMoving)
            return;
        
        OnCoinCollected?.Invoke();
        
        Destroy(gameObject);
    }
}
