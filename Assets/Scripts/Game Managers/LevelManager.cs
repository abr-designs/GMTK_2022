using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    //Properties
    //================================================================================================================//

    private static LevelManager _instance;

    private UIManager _uiManager;

    [SerializeField, Min(0f)]
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
        TryLoadFirst(true);
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
