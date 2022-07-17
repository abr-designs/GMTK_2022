using UnityEngine;

public class MaintainRotation : MonoBehaviour
{
    private Quaternion _startRotation;
    
    // Start is called before the first frame update
    private void Start()
    {
        _startRotation = transform.rotation;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        transform.rotation = _startRotation;
    }
}
