using System;
using DefaultNamespace;
using UnityEngine;

public class RoomGameTimer : MonoBehaviour
{
    public static Action ActionEvent;
    public static Action<int> TickEvent;

    [SerializeField]
    private int ticksPerAction;
    
    [Min(0.1f), SerializeField]
    private float tickTime;

    /*[SerializeField]
    private TMP_Text TEMP_TickText;*/
    
    
    private float _tickTimer;
    private int _tickCount;
    
    //Unity Functions
    //================================================================================================================//

    // Update is called once per frame
    private void Update()
    {
        GameStateManager.TotalTime += Time.deltaTime;
        
        if (_tickTimer < tickTime)
        {
            _tickTimer += Time.deltaTime;
            return;
        }
        
        _tickTimer = 0f;


        if (_tickCount++ < ticksPerAction)
        {
            TickEvent?.Invoke(_tickCount);
            return;
        }
        
        ActionEvent?.Invoke();
        _tickCount = 0;
        TickEvent?.Invoke(_tickCount);

    }

    //================================================================================================================//

}
