using System;
using UnityEngine;

namespace Destructibles
{
    public class DestructibleBase : MonoBehaviour, IDestructable
    {
        public int StartHealth => startHealth;
        [Min(0), SerializeField]
        private int startHealth = 4;
        public int CurrentHealth { get; private set; }

        private void Start()
        {
            CurrentHealth = startHealth;
        }

        public void ChangeHealth(int changeAmount)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth + changeAmount, 0, StartHealth);
        }
    }
}