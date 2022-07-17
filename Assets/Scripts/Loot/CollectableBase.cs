using UnityEngine;
using Utilities;

public abstract class CollectableBase : MonoBehaviour, ICheckForCollision
{
    private const string WALL_TAG = "Wall";
    
    //Properties
    //================================================================================================================//
    
    [SerializeField]
    private LayerMask collisionMask;
    private Ray[] _rays;
    
    [Min(0f), SerializeField]
    private float launchForce;

    protected Movement Movement
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

    // Start is called before the first frame update
    private void Start()
    {
        transform = gameObject.transform;
        _rays = new Ray[3];
        
        Movement.Init(this);
    }
    
    //CollectableBase Functions
    //================================================================================================================//

    public void Launch()
    {
        var dir = Random.insideUnitSphere;
        dir.y = Mathf.Abs(dir.y);
        
        Movement.Move(dir, launchForce);
    }
    
    public abstract void Collect(in Dice_Prototype collectedBy);
    
    public bool CheckForCollision()
    {
        var dir = Movement.Direction;
        var offsetDir = Vector3.Cross(dir, Vector3.up);
        
        _rays[0] = new Ray(transform.position, dir);
        _rays[1] = new Ray(transform.position + (offsetDir.normalized * 0.25f), dir);
        _rays[2] = new Ray(transform.position - (offsetDir.normalized * 0.25f), dir);

        for (int i = 0; i < _rays.Length; i++)
        {
            if (Physics.Raycast(_rays[i], out var raycastHit, 0.65f, collisionMask.value) == false)
                continue;

            var hitGameObject = raycastHit.transform.gameObject;

            if (hitGameObject.CompareTag(WALL_TAG))
            {
                Movement.Reflect(raycastHit.normal);
                transform.forward = Movement.Direction;
                break;
            }
        }

        return true;
    }
    
    //================================================================================================================//

}
