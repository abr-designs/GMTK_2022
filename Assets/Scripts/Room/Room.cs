using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Destructibles;
using UnityEngine;

public class Room : MonoBehaviour
{
    //Enums
    //================================================================================================================//

    public enum CONIDITION
    {
        NONE,
        ENEMIES,
        DESTRUCTIBLES,
        BOTH
    }
    
    //Action Events
    //================================================================================================================//
    public static Action OnLost;

    //Properties
    //================================================================================================================//
    
    private static UIManager _uiManager;
    
    [SerializeField, Header("Room Data")]
    private string levelName;
    [SerializeField]
    private Vector3[] startLocations;

    [SerializeField, Header("Room Door")]
    private Door door;
    [SerializeField]
    private bool doorStartsOpen;
    [SerializeField]
    private CONIDITION openCondition;

    [SerializeField, Header("Room Enemy Data")]
    private List<EnemyStatsScriptableObject> enemies;
    [SerializeField]
    private RoomEnemyManager roomEnemyManager;
    [SerializeField]
    private RoomGameTimer roomGameTimer;

    [SerializeField, Header("Prefabs")]
    private Dice_Prototype dicePrefab;

    private Dice_Prototype[] _dice;

    private bool _started;
    private bool _completed;

    private DestructibleBase[] _roomDestructables;
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
        Enemy.OnEnemyKilled -= CheckConditions;
        DestructibleBase.OnDestroyed -= CheckConditions;
        
    }
    
    //Room Functions
    //================================================================================================================//

    public void StartRoom()
    {
        _roomDestructables = FindObjectsOfType<DestructibleBase>();
        
        if (enemies.Count > 0)
        {
            roomEnemyManager.SpawnEnemies(enemies, _dice);
        }

        switch (openCondition)
        {
            case CONIDITION.NONE:
                break;
            case CONIDITION.ENEMIES:
                Enemy.OnEnemyKilled += CheckConditions;
                break;
            case CONIDITION.DESTRUCTIBLES:
                DestructibleBase.OnDestroyed += CheckConditions;
                break;
            case CONIDITION.BOTH:
                Enemy.OnEnemyKilled += CheckConditions;
                DestructibleBase.OnDestroyed += CheckConditions;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        _started = true;
    }

    private void SpawnDice()
    {
        _dice = new Dice_Prototype[startLocations.Length];
        for (var i = 0; i < startLocations.Length; i++)
        {
            var newDice = Instantiate(dicePrefab, startLocations[i], Quaternion.identity, transform);
            newDice.SetNewModifier(GameStateManager.CarryOverModifier);
            GameStateManager.CarryOverModifier = 0;
            _dice[i] = newDice;
            
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

    private void CheckConditions()
    {
        IEnumerator WaitFrameCoroutine()
        {
            yield return null;
            
            switch (openCondition)
            {
                case CONIDITION.ENEMIES:
                    if (roomEnemyManager.ActiveEnemies.All(a => a == null) == false)
                        break;
                    _uiManager.ShowTickTimer(false);
                    door.SetDoorOpen(true);
                    AudioController.PlaySound(SOUND.DOOR);
                    _completed = true;
                    break;
                case CONIDITION.DESTRUCTIBLES:
                    if (_roomDestructables.All(a => a == null) == false)
                        break;
                    door.SetDoorOpen(true);
                    AudioController.PlaySound(SOUND.DOOR);
                    _completed = true;
                    break;
                case CONIDITION.BOTH:
                    if (roomEnemyManager.ActiveEnemies.All(a => a == null) == false)
                        break;
                    _uiManager.ShowTickTimer(false);
                    
                    if (_roomDestructables.All(a => a == null) == false)
                        break;
                    door.SetDoorOpen(true);
                    AudioController.PlaySound(SOUND.DOOR);
                    _completed = true;
                    break;
            }
        }

        if (_completed)
            return;
        if (openCondition == CONIDITION.NONE)
            return;

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
