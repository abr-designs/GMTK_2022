using System;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;
using Random = UnityEngine.Random;

public class Dice_Prototype : MonoBehaviour, ICheckForCollision
{
    public static Action OnDiceDied;
    
    private const string WALL_TAG = "Wall";
    private const string DOOR_TAG = "Door";
    private const string DESTRUCTABLE_TAG = "Destructable";
    
    //Properties
    //================================================================================================================//

    private int _modifier;
    
    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private TMP_Text modifierText;

    [SerializeField]
    private Sprite[] diceSprites;
    [SerializeField]
    private SpriteRenderer diceValueSpriteRenderer;

    private int _currentValue;

    [FormerlySerializedAs("maxDistance")] [SerializeField, Min(0.1f)]
    private float maxDragDistance;
    [SerializeField]
    private float speedMult = 1f;
    

    private Vector2 _currentDirection;

    private static Camera CurrentCamera
    {
        get
        {
            if (_currentCamera == null)
                _currentCamera = FindObjectOfType<Camera>();

            return _currentCamera;
        }
    }
    private static Camera _currentCamera;

    private Vector3 _dragWorldPosition;

    private bool _currentlyDragging;
    private Vector3 _currentPosition;
    
    private Plane _plane;
    private Transform transform;

    
    [SerializeField, Header("Prototype")]
    private bool usePull;
    
    //================================================================================================================//

    private Vector3 _launchDir;
    private float _dragDistance;
    
    //================================================================================================================//

    public bool IsMoving => Movement.IsMoving;
    private Movement Movement
    {
        get
        {
            if (_movement == null)
                _movement = GetComponent<Movement>();

            return _movement;
        }
    }
    private Movement _movement;

    private bool _applyNoDamage;
    
    //================================================================================================================//

    [SerializeField]
    private LayerMask collisionMask;
    private Ray[] _rays;

    //Unity Functions
    //================================================================================================================//

    private void OnEnable()
    {
        LevelManager.OnWon += OnWon;
    }



    // Start is called before the first frame update
    private void Start()
    {
        transform = gameObject.transform;

        _plane = new Plane(Vector3.up, transform.position);

        lineRenderer.enabled = false;

        _rays = new Ray[3];
        RollNewRandomNumber();
        //SetNewModifier(0);
        
        Movement.Init(this);
    }

    private void Update()
    {
        if (Movement.IsMoving == false && _applyNoDamage)
            _applyNoDamage = false;
        
        if (Movement.IsMoving == false)
            return;
        
        //tmpText.transform.eulerAngles = Vector3.right * 90;
        modifierText.transform.eulerAngles = Vector3.right * 45;
    }

    private void OnMouseDrag()
    {
        if (Movement.IsMoving)
            return;
        
        
        if (_currentlyDragging == false)
        {
            _currentPosition = transform.position;

            lineRenderer.SetPosition(0, _currentPosition);
            lineRenderer.SetPosition(1, _currentPosition);
            lineRenderer.enabled = true;

            _currentlyDragging = true;
        }
        
        var ray = CurrentCamera.ScreenPointToRay(Input.mousePosition);
        
        Debug.DrawLine(ray.origin, ray.origin + (ray.direction * CurrentCamera.farClipPlane), Color.yellow);

        if (_plane.Raycast(ray, out var enter) == false)
            return;
        
        var hitpoint = ray.GetPoint(enter);
        Debug.DrawLine(_currentPosition, hitpoint, Color.green);

        var vector = hitpoint - _currentPosition;
        _launchDir = vector.normalized * (usePull ? -1f : 1f);
        _dragDistance = vector.magnitude;

        if (_dragDistance > maxDragDistance)
        {
            _dragWorldPosition = _currentPosition + (_launchDir * maxDragDistance);
            _dragDistance = maxDragDistance;
        }
        else
            _dragWorldPosition = _currentPosition + (_launchDir * _dragDistance);
        
        lineRenderer.SetPosition(1, _dragWorldPosition);
    }

    private void OnMouseUp()
    {
        if (_currentlyDragging == false)
            return;
        
        LaunchDice(_launchDir, _dragDistance * speedMult);
        lineRenderer.enabled = false;
        _currentlyDragging = false;
    }
    
    private void OnDisable()
    {
        LevelManager.OnWon -= OnWon;
    }


    //Movement Functions
    //================================================================================================================//

    private void LaunchDice(in Vector3 direction, in float speed)
    {
        
        Movement.Move(Vector3.ProjectOnPlane(direction, Vector3.up), speed);
        
        transform.forward = direction;
    }

    public void HitDice(in Vector3 direction, in float speed)
    {
        _applyNoDamage = true;
        Movement.Move(direction, speed);
        transform.forward = direction;
    }
    
    private void OnWon()
    {
        Destroy(gameObject);
    }
    
    //Dice Functions
    //================================================================================================================//


    private void RollNewRandomNumber()
    {
        var newValue = Random.Range(1, 7);

        //Ensure that we don't get the same number back to back (Feel like that's boring)
        while (newValue == _currentValue)
        {
            newValue = Random.Range(1, 7);
        }

        SetNewValue(newValue);
    }

    public void ReduceEffectiveness()
    {
        if (Movement.IsMoving)
            return;
        
        //If we've reach the limit, then the dice is dead
        if (_modifier - 1 <= -6)
        {
            var bigHitVFX = EffectFactory.CreateBigHitVFX();
            bigHitVFX.transform.position = transform.position;
            Destroy(bigHitVFX.gameObject, 2f);
                    
            AudioController.PlaySound(SOUND.DEATH);
            
            Destroy(gameObject);
            OnDiceDied?.Invoke();
            return;
        }
        
        SetNewModifier(_modifier - 1);
    }
    
    public void RaiseEffectiveness(in int amount)
    {
        SetNewModifier(_modifier + amount);
        
        EffectFactory.CreateFloatingText()
            .SetFloatingValues(transform.position + Vector3.up, amount);
    }

    private void SetNewValue(in int value)
    {
        _currentValue = value; 

        //tmpText.text = _currentValue.ToString();
        diceValueSpriteRenderer.sprite = diceSprites[_currentValue - 1];
    }
    public void SetNewModifier(in int value)
    {
        _modifier = value;

        if (_modifier == 0)
        {
            modifierText.text = string.Empty;
            return;
        }

        modifierText.text = value < 0 ? $"<color=red>{value}</color>" : $"<color=green>+{value}</color>";
    }
    
    //================================================================================================================//

    public bool CheckForCollision()
    {
        _rays[0] = new Ray(transform.position, transform.forward.normalized);
        _rays[1] = new Ray(transform.position + (transform.right.normalized * 0.25f), transform.forward.normalized);
        _rays[2] = new Ray(transform.position - (transform.right.normalized * 0.25f), transform.forward.normalized);

        for (int i = 0; i < _rays.Length; i++)
        {
            if (Physics.Raycast(_rays[i], out var raycastHit, 0.65f, collisionMask.value) == false)
                continue;

            var hitGameObject = raycastHit.transform.gameObject;

            if (hitGameObject.CompareTag(WALL_TAG))
            {
                var bumpVFX = EffectFactory.CreateBumpVFX();
                bumpVFX.transform.position = raycastHit.point;
                Destroy(bumpVFX.gameObject, 2f);
                
                AudioController.PlaySound(SOUND.BUMP);
                
                Movement.Reflect(raycastHit.normal);
                transform.forward = Movement.Direction;
                break;
            }
            else if (hitGameObject.CompareTag(DOOR_TAG))
            {
                
                GameStateManager.CarryOverModifier = _modifier;
                LevelManager.LoadNextLevel();
                
                return false;
            }
            else if (_applyNoDamage == false && hitGameObject.CompareTag(DESTRUCTABLE_TAG))
            {

                var shouldDestroy = false;
                var destructable = hitGameObject.GetComponent<IDestructable>();
                if (destructable != null)
                {
                    var damage = Mathf.Clamp(Mathf.Abs(_currentValue + _modifier), 1, 20);
                    destructable.ChangeHealth(-damage);

                    if (destructable.CurrentHealth <= 0)
                        shouldDestroy = true;
                }
                else
                    shouldDestroy = true;

                RollNewRandomNumber();
                
                if (destructable.CurrentHealth == 0)
                {
                    var bigHitVFX = EffectFactory.CreateBigHitVFX();
                    bigHitVFX.transform.position = raycastHit.point;
                    Destroy(bigHitVFX.gameObject, 2f);
                    
                    AudioController.PlaySound(SOUND.DEATH);
                }
                else
                {
                    var smallHitVFX = EffectFactory.CreateSmallHitVFX();
                    smallHitVFX.transform.position = raycastHit.point;
                    Destroy(smallHitVFX.gameObject, 2f);
                    
                    AudioController.PlaySound(SOUND.HIT);
                }

                if (shouldDestroy == false)
                {
                    if (destructable is Enemy enemy)
                    {
                        //enemy.Push(_currentVelocity.normalized, _currentVelocity.magnitude);
                        enemy.Movement.ApplyMovement(Movement);
                    }
                    
                    Movement.Reflect(Vector3.ProjectOnPlane((raycastHit.transform.position - transform.position).normalized, Vector3.up));
                    transform.forward = Movement.Direction;
                    
                    return true;
                }

                
                if(destructable.HandlesDestruction == false)
                    Destroy(hitGameObject);

                Movement.Rotate(Quaternion.Euler(0, Random.Range(-90, 90), 0));

                //TODO Might want to do a frame pause here
                transform.forward = Movement.Direction;
                return false;
            }
        }

        return true;
    }
    
    //================================================================================================================//
    
#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying == false)
            return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_dragWorldPosition, 0.2f);
    }

#endif
    //================================================================================================================//

}
