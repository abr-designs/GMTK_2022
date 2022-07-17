using System;
using DefaultNamespace;
using UnityEngine;

public class RoomGameTimer : MonoBehaviour
{
    private const int TICKS_PER_ACTION = 4;

    public static Action ActionEvent;
    public static Action<int> TickEvent;
    
    [Min(0.1f), SerializeField]
    private float tickTime;

    /*[SerializeField]
    private TMP_Text TEMP_TickText;*/

    private bool _incrementTime;
    private float _tickTimer;
    private int _tickCount;
    
    //Unity Functions
    //================================================================================================================//

    private void OnEnable()
    {
        Room.OnLost += OnLost;
        LevelManager.OnWon += OnWon;

        _incrementTime = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_incrementTime == false)
            return;
        
        GameStateManager.TotalTime += Time.deltaTime;
        
        if (_tickTimer < tickTime)
        {
            _tickTimer += Time.deltaTime;
            return;
        }
        
        _tickTimer = 0f;


        if (_tickCount++ < TICKS_PER_ACTION)
        {
            TickEvent?.Invoke(_tickCount);
            return;
        }
        
        ActionEvent?.Invoke();
        _tickCount = 0;
        TickEvent?.Invoke(_tickCount);

    }
    
    private void OnDisable()
    {
        Room.OnLost -= OnLost;
        LevelManager.OnWon -= OnWon;
    }

    //================================================================================================================//

    
    private void OnWon()
    {
        _incrementTime = false;
    }

    private void OnLost()
    {
        _incrementTime = false;
    }
    
    //================================================================================================================//

}
