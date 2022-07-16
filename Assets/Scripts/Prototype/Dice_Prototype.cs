using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Dice_Prototype : MonoBehaviour
{
    private const string WALL_TAG = "Wall";
    private const string DESTRUCTABLE_TAG = "Destructable";
    
    //Properties
    //================================================================================================================//

    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private TMP_Text tmpText;

    private int _currentValue;

    [FormerlySerializedAs("maxDistance")] [SerializeField, Min(0.1f)]
    private float maxDragDistance;
    [SerializeField]
    private float speedMult = 1f;
    

    private Vector2 _currentDirection;

    [SerializeField]
    private Camera _currentCamera;

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

    private bool _isMoving;

    private Vector3 _currentVelocity;
    [SerializeField]
    private float speedDecay;

    private const float MovingThreshold = 0.25f;
    
    //================================================================================================================//

    [SerializeField]
    private LayerMask collisionMask;
    private Ray[] _rays;

    //Unity Functions
    //================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        transform = gameObject.transform;

        _plane = new Plane(Vector3.up, transform.position);

        lineRenderer.enabled = false;

        _rays = new Ray[3];
        RollNewRandomNumber();
    }

    private void Update()
    {
        if (_isMoving == false)
            return;
        
        tmpText.transform.eulerAngles = Vector3.right * 90;
        
        if (_currentVelocity.magnitude <= MovingThreshold)
            _isMoving = false;

        //------------------------------------------------------------------------------------------------------------//

        if (CanContinueMoving() == false)
            return;
        
        //------------------------------------------------------------------------------------------------------------//

        transform.position += _currentVelocity * Time.deltaTime;
        _currentVelocity -= _currentVelocity * (speedDecay * Time.deltaTime);
    }

    private void OnMouseDrag()
    {
        if (_isMoving)
            return;
        
        
        if (_currentlyDragging == false)
        {
            _currentPosition = transform.position;

            lineRenderer.SetPosition(0, _currentPosition);
            lineRenderer.SetPosition(1, _currentPosition);
            lineRenderer.enabled = true;

            _currentlyDragging = true;
        }
        
        var ray = _currentCamera.ScreenPointToRay(Input.mousePosition);
        
        Debug.DrawLine(ray.origin, ray.origin + (ray.direction * _currentCamera.farClipPlane), Color.yellow);

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
        
        LaunchDice(_launchDir, _dragDistance);
        lineRenderer.enabled = false;
        _currentlyDragging = false;
    }

    //Movement Functions
    //================================================================================================================//


    private void LaunchDice(in Vector3 direction, in float distance)
    {
        _isMoving = true;
        _currentVelocity = direction * distance * speedMult;
        transform.forward = direction;
    }

    private void RollNewRandomNumber()
    {
        var newValue = Random.Range(1, 7);

        //Ensure that we don't get the same number back to back (Feel like that's boring)
        while (newValue == _currentValue)
        {
            newValue = Random.Range(1, 7);
        }

        _currentValue = newValue; 

        tmpText.text = _currentValue.ToString();
    }
    
    //================================================================================================================//

    private bool CanContinueMoving()
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
                _currentVelocity = Vector3.Reflect(_currentVelocity, raycastHit.normal);
                transform.forward = _currentVelocity.normalized;
                break;
            }
            else if (hitGameObject.CompareTag(DESTRUCTABLE_TAG))
            {

                var shouldDestroy = false;
                var destructable = hitGameObject.GetComponent<IDestructable>();
                if (destructable != null)
                {
                    destructable.ChangeHealth(-_currentValue);

                    if (destructable.CurrentHealth <= 0)
                        shouldDestroy = true;
                }
                else
                    shouldDestroy = true;

                RollNewRandomNumber();

                if (shouldDestroy == false)
                {
                    //TODO Should Reflect
                    _currentVelocity = Vector3.Reflect(_currentVelocity,
                        Vector3.ProjectOnPlane((raycastHit.transform.position - transform.position).normalized, Vector3.up));
                    transform.forward = _currentVelocity.normalized;
                    return true;
                }
                
                Destroy(hitGameObject);

                //TODO Might want to do a frame pause here
                _currentVelocity = Quaternion.Euler(0, Random.Range(-90, 90), 0) * _currentVelocity;
                transform.forward = _currentVelocity.normalized;
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
