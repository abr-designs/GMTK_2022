using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_Stats", menuName = "ScriptableObjects/Enemy Stats Template", order = 1)]
public class EnemyStatsScriptableObject : ScriptableObject
{
    public Sprite enemySprite;
    public int enemyHealth;
    public float moveDistance;

    public Vector2Int lootDropCountRange;
    [NonReorderable]
    public LootData[] loot;
}
