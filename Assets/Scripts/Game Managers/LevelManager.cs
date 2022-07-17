using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static Action OnWon;
    //Properties
    //================================================================================================================//

    private static LevelManager _instance;

    private UIManager _uiManager;

#if UNITY_EDITOR
    [SerializeField, Min(0), Header("Debug Values")] private int debugStartScene;
#endif

    [SerializeField, Min(0f), Header("Scene Fading")]
    private float fadeTime;
    [SerializeField, Header("Levels")]
    private List<Room> roomPrefabs;

    private Scene _activeScene;
    private int _currentRoomIndex;
    private Room _currentRoom;
    
    //Unity Functions
    //================================================================================================================//

    private void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _uiManager = FindObjectOfType<UIManager>();

        _activeScene = SceneManager.CreateScene("Rooms Scene");

#if UNITY_EDITOR
        _currentRoomIndex = debugStartScene;
        TryLoadRoom(true);
#else
        TryLoadFirst(true);

#endif
        
    }
    
    //================================================================================================================//

    private bool _loading;
    private void TryLoadFirst(bool skipIntro = false)
    {
        if (_loading)
            return;
        
        _currentRoomIndex = 0;

        TryLoadRoom(skipIntro);
    }
    
    private void TryLoadNext()
    {
        if (_loading)
            return;
        
        if (_currentRoomIndex + 1 >= roomPrefabs.Count)
        {
            //TODO Show game complete
            OnWon?.Invoke();
            return;
        }
        
        _currentRoomIndex++;

        TryLoadRoom(false);
    }
    
    private void TryLoadRoom(bool skipIntro)
    {
        _loading = true;
        _uiManager.FadeScreen(fadeTime, 
            () =>
            {
                if(_currentRoom)
                    Destroy(_currentRoom.gameObject);

                _currentRoom = Instantiate(roomPrefabs[_currentRoomIndex]);
                SceneManager.MoveGameObjectToScene(_currentRoom.gameObject, _activeScene);
            },
            () =>
            {
                _loading = false;
                _currentRoom.StartRoom();
            }, skipIntro);
    }
    
    //Static Functions
    //================================================================================================================//

    public static void LoadNextLevel() => _instance.TryLoadNext();

    public static void LoadFirstLevel() => _instance.TryLoadFirst();
    
    //================================================================================================================//

}
