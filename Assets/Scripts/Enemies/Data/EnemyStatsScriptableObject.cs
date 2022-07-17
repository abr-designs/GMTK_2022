using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_Stats", menuName = "ScriptableObjects/Enemy Stats Template", order = 1)]
public class EnemyStatsScriptableObject : ScriptableObject
{
    [Header("Enemy Data")]
    public Sprite enemySprite;
    public int enemyHealth;
    public float moveDistance;

    [Header("Spawn Data")] 
    public float spawnTime;
    public Vector2 spawnDelay;
    public AnimationCurve spawnCurve;
    public float shakeAmount;

    [Header("Loot")]
    public Vector2Int lootDropCountRange;
    [NonReorderable]
    public LootData[] loot;
}
