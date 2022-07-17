using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SortingOrderSetter : MonoBehaviour
{
    private const float UPDATE_DELAY = 0.35f;
    
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

    private float _timer;
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateSortingOrder();
    }

    // Update is called once per frame
    void Update()
    {
        if (_timer < UPDATE_DELAY)
        {
            _timer += Time.deltaTime;
            return;
        }

        _timer = 0f;
        UpdateSortingOrder();
    }

    private void UpdateSortingOrder()
    {
        var order = -Mathf.RoundToInt(transform.position.z);
        SpriteRenderer.sortingOrder = order;
    }
}
