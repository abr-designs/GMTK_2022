using System;
using UnityEngine;

[Serializable]
public struct LootData
{
    public string name;
    public bool alwaysDrop;
    public int weight;
    public Vector2Int dropCountRange;
    public GameObject prefab;
}
