using System;
using UnityEngine;

namespace Destructibles
{
    public class DestructibleBase : MonoBehaviour, IDestructable
    {
        public bool HandlesDestruction => false;
        public int StartHealth => startHealth;
        [Min(0), SerializeField]
        private int startHealth = 4;
        public int CurrentHealth { get; private set; }

        protected virtual void Start()
        {
            CurrentHealth = startHealth;
        }

        public virtual void ChangeHealth(int changeAmount)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth + changeAmount, 0, StartHealth);
        }
    }
}