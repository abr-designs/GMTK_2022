﻿using System;
using Unity.Mathematics;
using UnityEngine;
using Utilities.Extension;

namespace Destructibles
{
    public class DestructibleBase : MonoBehaviour, IDestructable
    {
        public static Action OnDestroyed;
        
        
        
        //Properties
        //================================================================================================================//
        private static Room _currentRoom;
        public bool HandlesDestruction => true;
        public int StartHealth => startHealth;
        [Min(0), SerializeField]
        private int startHealth = 4;
        public int CurrentHealth { get; private set; }

        [SerializeField, NonReorderable]
        private LootData[] loot;

        private bool _destroyed;

        //Unity Functions
        //================================================================================================================//

        protected virtual void Start()
        {
            if (_currentRoom == null)
                _currentRoom = FindObjectOfType<Room>();
            
            CurrentHealth = startHealth;
        }

        private void OnDestroy()
        {
            if (_destroyed == false)
                return;
            
            OnDestroyed?.Invoke();
        }

        //DestructibleBase Functions
        //================================================================================================================//
        private void DropLoot()
        {
            var toDrop = loot.GetLootToDrop(1);

            foreach (var lootData in toDrop)
            {
                if (lootData.prefab == null)
                    continue;
                
                var dropCount = lootData.dropCountRange.GetRandomRange(true);
                for (int i = 0; i < dropCount; i++)
                {
                    var temp = Instantiate(lootData.prefab, transform.position, quaternion.identity, _currentRoom.transform);
                
                    if(temp.TryGetComponent<CollectableBase>(out var collectableBase))
                        collectableBase.Launch();
                }

            }
        }

        //IDestructable Functions
        //================================================================================================================//

        public virtual void ChangeHealth(int changeAmount)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth + changeAmount, 0, StartHealth);

            EffectFactory.CreateFloatingText()
                .SetFloatingValues(transform.position + Vector3.up, changeAmount);
            
            if (CurrentHealth > 0)
                return;

            DropLoot();
            Destroy(gameObject);
            _destroyed = true;
        }
        
        
        
        //================================================================================================================//

    }
}