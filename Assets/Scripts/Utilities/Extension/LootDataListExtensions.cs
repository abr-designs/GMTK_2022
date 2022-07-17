using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LootDataListExtensions
{
    public static LootData[] GetLootToDrop(this IEnumerable<LootData> lootDatas, int dropCount)
    {
        var dropList = new LootData[dropCount];
        GetLootToDrop(lootDatas, ref dropList);

        return dropList;
    }

    public static LootData[] GetLootToDrop(this IEnumerable<LootData> lootDatas, Vector2Int dropCountRange)
    {
        var dropList = new LootData[Random.Range(dropCountRange.x, dropCountRange.y + 1)];
        GetLootToDrop(lootDatas, ref dropList);

         return dropList;
    }
    private static void GetLootToDrop(this IEnumerable<LootData> lootDatas, ref LootData[] dropsNonAlloc)
    {
        var dropables = lootDatas
            .Where(a => a.alwaysDrop == false)
            .ToArray();
        

        var allWeights = lootDatas
            .Where(a => a.alwaysDrop == false)
            .Sum(a => a.weight);
        
        var probabilities = new float[dropables.Length];
        for (int i = 0; i < dropables.Length; i++)
        {
            probabilities[i] = dropables[i].weight / (float)allWeights;
        }

        var dropCount = dropsNonAlloc.Length;

        for (int i = 0; i < dropCount; i++)
        {
            var hitValue = Random.value;
            var runningValue = 0f;

            for (var j = 0; j < dropables.Length; j++)
            {
                var lootData = dropables[j];
                runningValue += probabilities[j];

                if (hitValue >= runningValue)
                    continue;
                
                dropsNonAlloc[i] = lootData;
                break;
            }
        }
    }
}
