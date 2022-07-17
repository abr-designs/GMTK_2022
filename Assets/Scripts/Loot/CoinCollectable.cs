using System;
using UnityEngine;

public class CoinCollectable : CollectableBase
{
    public static Action OnCoinCollected;
    
    public override void Collect(in Dice_Prototype collectedBy)
    {
        if (Movement.IsMoving)
            return;
        
        OnCoinCollected?.Invoke();

        EffectFactory.CreateFloatingSprite()
            .SetFloatingValues(transform.position + Vector3.up, FloatingSprite.TYPE.TOKEN);
        
        Destroy(gameObject);
        AudioController.PlaySound(SOUND.COLLECT);

    }
}
