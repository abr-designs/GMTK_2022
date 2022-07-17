using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCollector : MonoBehaviour
{
    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private float collectionRadius;

    private Dice_Prototype _dicePrototype;
    private Collider[] _overlapResults;
    
    //================================================================================================================//

    // Start is called before the first frame update
    private void Start()
    {
        _dicePrototype = GetComponent<Dice_Prototype>();
        _overlapResults = new Collider[10];

    }

    // Update is called once per frame
    private void Update()
    {
        if (_dicePrototype.IsMoving == false)
            return;


        var size = Physics.OverlapSphereNonAlloc(transform.position, collectionRadius, _overlapResults, layerMask.value);
        if (size == 0)
            return;

        for (int i = 0; i < size; i++)
        {
            var collectable = _overlapResults[i].GetComponent<CollectableBase>();
            if (collectable == null)
                continue;
            
            collectable.Collect(_dicePrototype);
        }
    }
    
    //================================================================================================================//

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying == false)
            return;
        
        
    }

#endif
}
