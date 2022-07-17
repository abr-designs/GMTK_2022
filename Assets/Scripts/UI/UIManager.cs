using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //Properties
    //================================================================================================================//

    [SerializeField, Header("Timer")]
    private Image timerImage;

    [SerializeField]
    private Sprite[] timerSprites;

    //Unity Functions
    //================================================================================================================//

    private void OnEnable()
    {
        RoomGameTimer.TickEvent += TickEvent;
    }

    // Start is called before the first frame update
    private void Start()
    {
        TickEvent(0);
    }

    private void OnDisable()
    {
        RoomGameTimer.TickEvent -= TickEvent;
    }

    //UIManager Functions
    //================================================================================================================//

    private void TickEvent(int tick)
    {
        timerImage.sprite = timerSprites[tick];
    }
    
    //================================================================================================================//

}
