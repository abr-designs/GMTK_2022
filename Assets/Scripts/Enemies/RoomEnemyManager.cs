using System.Collections.Generic;
using UnityEngine;
using Utilities.Extension;

public class RoomEnemyManager : MonoBehaviour
{
    private Dice_Prototype[] _dice;

    public List<Enemy> ActiveEnemies { get; private set; }

    //================================================================================================================//

    [SerializeField]
    private Rect enemySpawnRect;
    [SerializeField]
    private float spawnHeight;

    [Min(0f), SerializeField]
    private float diceSpaceRadius;

    [SerializeField]
    private Enemy enemyPrefab;

    public int EnemySpawnedCount { get; private set; }
    /*[SerializeField]
    private EnemyStatsScriptableObject[] spawnEnemies;*/
    
    //Unity Functions
    //================================================================================================================//

    // Start is called before the first frame update
    private void Start()
    {
        EnemySpawnedCount = 0;
    }
    

    //================================================================================================================//

    public void SpawnEnemies(in IEnumerable<EnemyStatsScriptableObject> spawnEnemies, in Dice_Prototype[] dice)
    {
        _dice = dice;
        ActiveEnemies = new List<Enemy>();
        foreach (var enemyData in spawnEnemies)
        {
            Vector3 checkPoint = Vector3.zero;
            bool passed = false;
            while (passed == false)
            {
                checkPoint = enemySpawnRect.GetPointInRect(spawnHeight, 1f);
                foreach (var die in _dice)
                {
                    if (Vector3.Distance(die.transform.position, checkPoint) <= diceSpaceRadius)
                        break;

                    passed = true;
                }
            }
            
            //TODO Might want to make sure we dont spawn close to the dice!
            var newEnemy = Instantiate(enemyPrefab, 
                checkPoint,
                enemyPrefab.transform.rotation,
                transform);
            
            newEnemy.Init(enemyData);
            EnemySpawnedCount++;
            ActiveEnemies.Add(newEnemy);
        }
    }
    
    public Vector3 GetClosestDicePosition(in Vector3 worldPosition)
    {
        var dist = 999f;
        var index = -1;
        for (int i = 0; i < _dice.Length; i++)
        {
            var dice = _dice[i];
            if (dice == null)
                continue;

            var foundDistance = Vector3.Distance(worldPosition, dice.transform.position);

            if (foundDistance >= dist)
                continue;

            index = i;
            dist = foundDistance;
        }

        return index < 0 ? Vector3.zero : _dice[index].transform.position;
    }
    
    //================================================================================================================//

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        var min = new Vector3(enemySpawnRect.xMin, spawnHeight, enemySpawnRect.yMin);
        var max = new Vector3(enemySpawnRect.xMax, spawnHeight, enemySpawnRect.yMax);

        var TL = new Vector3(min.x, spawnHeight, max.z);
        var BR = new Vector3(max.x, spawnHeight, min.z);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(TL, max);
        Gizmos.DrawLine(max, BR);
        Gizmos.DrawLine(BR, min);
        Gizmos.DrawLine(min, TL);
    }

#endif
    
    //================================================================================================================//

}
