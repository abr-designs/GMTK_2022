using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDestructable
{
    int StartHealth { get; }
    int CurrentHealth { get; }

    void ChangeHealth(int changeAmount);
}
