using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using Utilities;
using Utilities.Extension;
using Random = UnityEngine.Random;

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

    [SerializeField]
    private SpriteRenderer whiteHurtRenderer;
    private SpriteRenderer SpriteRenderer
    {
        get
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
            return _spriteRenderer;
        }
    }

    private SpriteRenderer _spriteRenderer;
    
    private SpriteMask SpriteMask
    {
        get
        {
            if (_spriteMask == null)
                _spriteMask = GetComponentInChildren<SpriteMask>();
            
            return _spriteMask;
        }
    }

    private SpriteMask _spriteMask;
    
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
    
    
    //Spawning Properties
    //================================================================================================================//

    private bool _isSpawning;
    private bool _killed;
    
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
        
        _rays = new Ray[3];
        Movement.Init(this);
    }

    private void OnDisable()
    {
        RoomGameTimer.ActionEvent -= MoveTowardClosestDice;
    }

    private void OnDestroy()
    {
        if (_killed == false)
            return;
        
        OnEnemyKilled?.Invoke();
    }

    //Enemy Functions
    //================================================================================================================//

    public void Init(EnemyStatsScriptableObject enemyStatsScriptableObject)
    {
        _enemyStatsScriptableObject = enemyStatsScriptableObject;
        
        SpriteRenderer.sprite = _enemyStatsScriptableObject.enemySprite;
        SpriteMask.sprite = _enemyStatsScriptableObject.enemySprite;
        _moveDistance = _enemyStatsScriptableObject.moveDistance;

        StartHealth = CurrentHealth = _enemyStatsScriptableObject.enemyHealth;

        SpriteRenderer.transform.localScale = Vector3.one * _enemyStatsScriptableObject.scale;

        PlaySpawnAnimation();
    }

    private void DropLoot()
    {
        var toDrop = _enemyStatsScriptableObject.loot.GetLootToDrop(_enemyStatsScriptableObject.lootDropCountRange);

        foreach (var lootData in toDrop)
        {
            var dropCount = lootData.dropCountRange.GetRandomRange(true);
            for (int i = 0; i < dropCount; i++)
            {
                var temp = Instantiate(lootData.prefab, transform.position, quaternion.identity, _roomEnemyManager.transform);
                
                if(temp.TryGetComponent<CollectableBase>(out var collectableBase))
                    collectableBase.Launch();
            }

        }
    }

    private void PlaySpawnAnimation()
    {
        IEnumerator SpawnCoroutine()
        {
            _isSpawning = true;

            
            
            var startPos = transform.position - Vector3.up * 3f;
            var endPos = transform.position;

            var spawnTime = _enemyStatsScriptableObject.spawnTime;
            var spawnCurve = _enemyStatsScriptableObject.spawnCurve;
            var shakeAmount = _enemyStatsScriptableObject.shakeAmount;

            var VFX = EffectFactory.CreateSpawningVFX();
            VFX.transform.position = endPos - Vector3.up;
            transform.position = startPos;

            //TODO Create Spawning VFX
            yield return new WaitForSeconds(_enemyStatsScriptableObject.spawnDelay.GetRandomRange());

            for (var t = 0f; t < spawnTime; t += Time.deltaTime )
            {
                var td = spawnCurve.Evaluate(t / spawnTime);
                transform.position = Vector3.Lerp(startPos, endPos, td) + Random.insideUnitSphere * shakeAmount;
            
                yield return null;
            }
        
            transform.position = endPos;
            var emissionModule = VFX.emission;
            emissionModule.rateOverTime = 0f;
            

            _isSpawning = false;
            Destroy(VFX.gameObject);
        }
        
        if (_isSpawning)
            return;
        
        StartCoroutine(SpawnCoroutine());
    }

    private void MoveTowardClosestDice()
    {
        if (_isSpawning)
            return;
        
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
        if (_isSpawning)
            return;
        
        CurrentHealth = Mathf.Clamp(CurrentHealth + changeAmount, 0, StartHealth);

        if (CurrentHealth > 0)
        {
            EffectFactory.CreateFloatingText()
                .SetFloatingValues(transform.position + Vector3.up, changeAmount);

            StartCoroutine(HitEffectCoroutine());
            return;
        }
        
        EffectFactory.CreateFloatingSprite()
            .SetFloatingValues(transform.position + Vector3.up, FloatingSprite.TYPE.SKULL);
        
        DropLoot();

        _killed = true;
        //TODO Do any VFX for death here
        Destroy(gameObject);
    }

    private IEnumerator HitEffectCoroutine()
    {
        whiteHurtRenderer.enabled = true;

        yield return new WaitForSeconds(0.2f);

        whiteHurtRenderer.enabled = false;
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
                
                var smallHitVFX = EffectFactory.CreateSmallHitVFX();
                smallHitVFX.transform.position = raycastHit.point;
                Destroy(smallHitVFX.gameObject, 2f);
            
                dice.ReduceEffectiveness();
                dice.HitDice(Movement.Direction, 10);
                return false;
            }
        }

        return true;
    }
}
