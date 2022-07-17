using UnityEngine;

public class PotionCollectable : CollectableBase
{
    [Min(1), SerializeField]
    private int healAmount;
    
    public override void Collect(in Dice_Prototype collectedBy)
    {
        if (Movement.IsMoving)
            return;
        
        collectedBy.RaiseEffectiveness(healAmount);
        
        Destroy(gameObject);
        AudioController.PlaySound(SOUND.COLLECT);
    }
}
