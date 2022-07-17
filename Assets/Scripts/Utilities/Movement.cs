using UnityEngine;
using Utilities;

public class Movement : MonoBehaviour
{
    private const float MovingThreshold = 0.25f;

    private static readonly Rect MOVE_RECT = new Rect(-5, -5, 10, 10);
    
    //Properties
    //================================================================================================================//
    private bool _isReady;

    public bool IsMoving { get; private set; }

    public Vector3 Direction => _currentVelocity.normalized;
    
    private Vector3 _currentVelocity;
    [SerializeField]
    private float speedDecay = 4;

    [SerializeField, Header("Gravity")]
    private bool useGravity;
    [SerializeField]
    private float groundHeight;
    [SerializeField, Min(1f)]
    private float bounceReduction = 3f;

    private ICheckForCollision _checkForCollision;

    //Unity Functions
    //================================================================================================================//

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (_isReady == false)
            return;
        
        if (IsMoving == false)
            return;
        
        if (_currentVelocity.magnitude <= MovingThreshold)
            IsMoving = false;
        
        //------------------------------------------------------------------------------------------------------------//

        if (_checkForCollision.CheckForCollision() == false)
            return;
        
        //------------------------------------------------------------------------------------------------------------//

        //transform.position += _currentVelocity * Time.deltaTime;
        TryMoveToNewPosition(_currentVelocity * Time.deltaTime);
        
        _currentVelocity -= _currentVelocity * (speedDecay * Time.deltaTime);

        if (useGravity == false)
            return;
        
        if (transform.position.y > groundHeight)
            _currentVelocity += Physics.gravity * (speedDecay * Time.deltaTime);
        else if (_currentVelocity.y > MovingThreshold)
            _currentVelocity.y = -_currentVelocity.y / bounceReduction;
        else
        {
            _currentVelocity.y = 0f;
        }

    }

    private void TryMoveToNewPosition(in Vector3 delta)
    {
        var currentPosition = transform.position;

        currentPosition += delta;

        if (currentPosition.x < MOVE_RECT.xMin)
            currentPosition.x = MOVE_RECT.xMin;
        else if (currentPosition.x > MOVE_RECT.xMax)
            currentPosition.x = MOVE_RECT.xMax;
        
        if (currentPosition.z < MOVE_RECT.yMin)
            currentPosition.z = MOVE_RECT.yMin;
        else if (currentPosition.z > MOVE_RECT.yMax)
            currentPosition.z = MOVE_RECT.yMax;

        transform.position = currentPosition;
    }
    
    //Movement Functions
    //================================================================================================================//

    public void Init(ICheckForCollision checkForCollision)
    {
        _checkForCollision = checkForCollision;
        _isReady = true;
    }

    public void Move(in Vector3 direction, in float speed)
    {
        _currentVelocity = direction * speed;
        IsMoving = true;
    }

    public void Reflect(in Vector3 normal)
    {
        _currentVelocity = Vector3.Reflect(_currentVelocity, normal);
    }

    public void Rotate(in Quaternion rotation)
    {
        _currentVelocity = rotation * _currentVelocity;
    }

    public void ApplyMovement(in Movement movement)
    {
        _currentVelocity = movement._currentVelocity;
    }
    
    //Editor Functions
    //================================================================================================================//

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color= Color.magenta;
        Gizmos.DrawRay(transform.position, _currentVelocity);
    }

#endif
    
    //================================================================================================================//

}
