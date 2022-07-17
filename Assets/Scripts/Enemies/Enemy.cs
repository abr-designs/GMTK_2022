using System;
using Unity.Mathematics;
using UnityEngine;
using Utilities;
using Utilities.Extension;

[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour, IDestructable, ICheckForCollision
{
    public static Action OnEnemyKilled;
    
    private const string WALL_TAG = "Wall";
    private const string DICE_TAG = "Dice";
    
    private const int ENEMY_DAMAGE = 1;
    //================================================================================================================//

    private static RoomEnemyManager _roomEnemyManager;

    [SerializeField]
    private LayerMask enemyAttackLayerMask;
    private Ray[] _rays;

    public bool HandlesDestruction => true;
    public int StartHealth { get; private set; }
    public int CurrentHealth { get; private set; }

    private float _moveDistance;

    private EnemyStatsScriptableObject _enemyStatsScriptableObject;

    private SpriteRenderer SpriteRenderer
    {
        get
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
            
            return _spriteRenderer;
        }
    }

    private SpriteRenderer _spriteRenderer;
    
    public Movement Movement
    {
        get
        {
            if (_movement == null)
                _movement = GetComponent<Movement>();

            return _movement;
        }
    }
    private Movement _movement;
    
    private Transform transform;
    
    //Unity Functions
    //================================================================================================================//

    private void OnEnable()
    {
        RoomGameTimer.ActionEvent += MoveTowardClosestDice;
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (_roomEnemyManager == null)
            _roomEnemyManager = FindObjectOfType<RoomEnemyManager>();
        
        transform = gameObject.transform;
        _rays = new Ray[3];
        Movement.Init(this);
    }

    private void OnDisable()
    {
        RoomGameTimer.ActionEvent -= MoveTowardClosestDice;
    }
    
    //Enemy Functions
    //================================================================================================================//

    public void Init(EnemyStatsScriptableObject enemyStatsScriptableObject)
    {
        _enemyStatsScriptableObject = enemyStatsScriptableObject;
        
        SpriteRenderer.sprite = _enemyStatsScriptableObject.enemySprite;
        _moveDistance = _enemyStatsScriptableObject.moveDistance;

        StartHealth = CurrentHealth = _enemyStatsScriptableObject.enemyHealth;
    }

    private void DropLoot()
    {
        var toDrop = _enemyStatsScriptableObject.loot.GetLootToDrop(_enemyStatsScriptableObject.lootDropCountRange);

        foreach (var lootData in toDrop)
        {
            var dropCount = lootData.dropCountRange.GetRandomRange(true);
            for (int i = 0; i < dropCount; i++)
            {
                var temp = Instantiate(lootData.prefab, transform.position, quaternion.identity);
                
                if(temp.TryGetComponent<CollectableBase>(out var collectableBase))
                    collectableBase.Launch();
            }

        }
    }

    private void MoveTowardClosestDice()
    {
        var currentPos = transform.position;
        var targetPosition = _roomEnemyManager.GetClosestDicePosition(currentPos);

        targetPosition.y = currentPos.y;

        //TODO This should be a smooth movement
        //currentPos += (targetPosition - currentPos).normalized * _moveDistance;
        //transform.position = currentPos;
        
        Movement.Move((targetPosition - currentPos).normalized, _moveDistance);

        
    }

    //IDestructable Functions
    //================================================================================================================//

    public void ChangeHealth(int changeAmount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + changeAmount, 0, StartHealth);

        if (CurrentHealth > 0)
        {
            EffectFactory.CreateFloatingText()
                .SetFloatingValues(transform.position + Vector3.up, changeAmount);
            return;
        }
        
        EffectFactory.CreateFloatingSprite()
            .SetFloatingValues(transform.position + Vector3.up, FloatingSprite.TYPE.SKULL);
        
        DropLoot();
        
        OnEnemyKilled?.Invoke();
        //TODO Do any VFX for death here
        Destroy(gameObject);
    }
    
    //ICheckForCollision
    //================================================================================================================//

    public bool CheckForCollision()
    {
        var dir = Movement.Direction;
        var offsetDir = Vector3.Cross(dir, Vector3.up);

        var currentPos = transform.position;
        currentPos -= Vector3.up * 0.25f;
        currentPos -= dir.normalized;
        
        _rays[0] = new Ray(currentPos, dir);
        _rays[1] = new Ray(currentPos + (offsetDir.normalized * 0.25f), dir);
        _rays[2] = new Ray(currentPos - (offsetDir.normalized * 0.25f), dir);

        for (int i = 0; i < _rays.Length; i++)
        {
            if (Physics.Raycast(_rays[i], out var raycastHit, 1.65f, enemyAttackLayerMask.value) == false)
            {
                Debug.DrawRay(_rays[i].origin, _rays[i].direction, Color.yellow);
                continue;
            }
            
            Debug.DrawRay(_rays[i].origin, _rays[i].direction, Color.green);

            var hitGameObject = raycastHit.transform.gameObject;

            if (hitGameObject.CompareTag(WALL_TAG))
            {
                Movement.Reflect(raycastHit.normal);

                transform.forward = Movement.Direction;
                break;
            }
            else if (hitGameObject.CompareTag(DICE_TAG))
            {
                if (hitGameObject.TryGetComponent<Dice_Prototype>(out var dice) == false)
                    continue;

                if (dice.IsMoving)
                    continue;
            
                dice.ReduceEffectiveness();
                dice.HitDice(Movement.Direction, 10);
                return false;
            }
        }

        return true;
    }
}
