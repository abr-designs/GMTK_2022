using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public static Action OnLost;

    private static UIManager _uiManager;
    
    [SerializeField, Header("Room Data")]
    private string levelName;
    [SerializeField]
    private Vector3[] startLocations;

    [SerializeField, Header("Room Door")]
    private Door door;
    [SerializeField]
    private bool doorStartsOpen;

    [SerializeField, Header("Room Enemy Data")]
    private List<EnemyStatsScriptableObject> enemies;
    [SerializeField]
    private RoomEnemyManager roomEnemyManager;
    [SerializeField]
    private RoomGameTimer roomGameTimer;

    [SerializeField, Header("Prefabs")]
    private Dice_Prototype dicePrefab;

    
    //TODO Need to set up a condition

    //Unity Functions
    //================================================================================================================//
    private void OnEnable()
    {
        Dice_Prototype.OnDiceDied += OnDiceDied;
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (_uiManager == null)
            _uiManager = FindObjectOfType<UIManager>();
        
        SpawnDice();
        door.SetDoorOpen(doorStartsOpen);
        _uiManager.ShowTickTimer(enemies.Count > 0);

    }

    private void OnDisable()
    {
        Dice_Prototype.OnDiceDied -= OnDiceDied;
    }
    
    //Room Functions
    //================================================================================================================//

    public void StartRoom()
    {
        
        if (enemies.Count > 0)
        {
            roomEnemyManager.SpawnEnemies(enemies);
        }
    }

    private void SpawnDice()
    {
        foreach (var startLocation in startLocations)
        {
            var newDice = Instantiate(dicePrefab, startLocation, Quaternion.identity, transform);
        }
    }
    
    //Callback Functions
    //================================================================================================================//

    private void OnDiceDied()
    {
        IEnumerator WaitFrameCoroutine()
        {
            yield return null;
            
            var found = FindObjectOfType<Dice_Prototype>();

            if (found == null)
                OnLost?.Invoke();
        }

        StartCoroutine(WaitFrameCoroutine());
    }
    
    //================================================================================================================//

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        foreach (var startLocation in startLocations)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(startLocation, 0.5f);
        }
    }


#endif
}
