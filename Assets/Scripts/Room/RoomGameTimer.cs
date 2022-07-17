using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomGameTimer : MonoBehaviour
{
    public static Action ActionEvent;
    
    [SerializeField]
    private int ticksPerAction;
    
    [Min(0.1f), SerializeField]
    private float tickTime;

    [SerializeField]
    private TMP_Text TEMP_TickText;
    
    
    private float _tickTimer;
    private int _tickCount;
    
    //Unity Functions
    //================================================================================================================//

    // Start is called before the first frame update
    void Start()
    {
        TEMP_TickText.text = $"{_tickCount}/{ticksPerAction}";
    }

    // Update is called once per frame
    private void Update()
    {
        if (_tickTimer < tickTime)
        {
            _tickTimer += Time.deltaTime;
            return;
        }
        
        _tickCount++;
        _tickTimer = 0f;
        TEMP_TickText.text = $"{_tickCount}/{ticksPerAction}";

        if (_tickCount < ticksPerAction)
            return;
        
        ActionEvent?.Invoke();
        _tickCount = 0;
        TEMP_TickText.text = $"{_tickCount}/{ticksPerAction}";
    }
    
    //================================================================================================================//

}
