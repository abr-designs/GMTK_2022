using System;
using System.Collections;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static Action OnRestartPressed;
    //Properties
    //================================================================================================================//

    [SerializeField, Header("Timer")]
    private Image timerImage;

    private bool _showingTimer = true;

    [SerializeField]
    private Sprite[] timerSprites;

    [SerializeField, Header("Labels")]
    private TMP_Text timerText;
    [SerializeField]
    private TMP_Text enemiesKilledText;
    [SerializeField]
    private TMP_Text tokensEarnedText;

    [SerializeField, Header("Death Screen")]
    private GameObject deathScreenWindow;
    [SerializeField]
    private Button restartButton;
    
    [SerializeField, Header("Win Screen")]
    private GameObject winScreenWindow;
    [SerializeField]
    private Button newGameButton;

    [SerializeField, Header("Fading UI")]
    private Image fadeImage;
    
    private bool _playedTune;


    //Unity Functions
    //================================================================================================================//

    private void OnEnable()
    {
        RoomGameTimer.TickEvent += TickEvent;
        CoinCollectable.OnCoinCollected += OnCoinCollected;
        Enemy.OnEnemyKilled += OnEnemyKilled;
        Room.OnLost += OnLost;
        LevelManager.OnWon += OnWon;
    }

    

    // Start is called before the first frame update
    private void Start()
    {
        TickEvent(0);
        deathScreenWindow.SetActive(false);
        winScreenWindow.SetActive(false);

        SetupButtons();

        ShowTickTimer(false);
    }

    private void LateUpdate()
    {
        timerText.text = $"{GameStateManager.TotalTime:#0.00}";
    }

    private void OnDisable()
    {
        RoomGameTimer.TickEvent -= TickEvent;
        Room.OnLost -= OnLost;
        LevelManager.OnWon -= OnWon;
    }

    //UIManager Functions
    //================================================================================================================//

    private void SetupButtons()
    {
        void RestartGame()
        {
            LevelManager.LoadFirstLevel();
            deathScreenWindow.SetActive(false);
            winScreenWindow.SetActive(false);
            GameStateManager.Reset();
            ResetUICounters();
            
        }
        
        restartButton.onClick.AddListener(RestartGame);
        newGameButton.onClick.AddListener(RestartGame);
    }

    public void ShowTickTimer(bool showTickTimer)
    {
        if(_showingTimer == showTickTimer)
            return;

        _showingTimer = showTickTimer;
        timerImage.enabled = showTickTimer;
    }
    private void TickEvent(int tick)
    {
        if(_showingTimer == false)
            return;
        
        timerImage.sprite = timerSprites[tick];
        
        AudioController.PlaySound(tick == 4 ? SOUND.ACTION : SOUND.TICK, 0.75f);

    }
    
    private void OnCoinCollected()
    {
        GameStateManager.TotalCoins++;
        tokensEarnedText.text = GameStateManager.TotalCoins.ToString();
    }
    
    private void OnEnemyKilled()
    {
        GameStateManager.TotalKills++;
        enemiesKilledText.text = GameStateManager.TotalKills.ToString();
    }

    private void ResetUICounters()
    {
        tokensEarnedText.text = "0";
        enemiesKilledText.text = "0";
        
        _playedTune = false;
    }

    
    private void OnLost()
    {
        if (deathScreenWindow.activeInHierarchy)
            return;
        
        
        deathScreenWindow.SetActive(true);
        
        if (_playedTune)
            return;
        
        AudioController.PlayMusic(MUSIC.LOSE);
        _playedTune = true;

    }

    private void OnWon()
    {
        if (winScreenWindow.activeInHierarchy)
            return;
        
        winScreenWindow.SetActive(true);

        if (_playedTune)
            return;
        
        AudioController.PlayMusic(MUSIC.WIN);
        _playedTune = true;
    }

    
    //Fade Functions
    //================================================================================================================//

    private bool _isFading;
    public void FadeScreen(in float fadeTime, in Action OnFadedCallback, in Action OnFadeComplete, in bool skipIntro)
    {
        if (_isFading)
            return;
        
        if(skipIntro)
            StartCoroutine(FadeNoIntroCoroutine(fadeTime, OnFadedCallback, OnFadeComplete));
        else
            StartCoroutine(FadeCoroutine(fadeTime, OnFadedCallback, OnFadeComplete));
    }

    private IEnumerator FadeCoroutine(float fadeTime, Action OnFadedCallback, Action OnFadeComplete)
    {
        var halfFadeTime = fadeTime / 2f;
        var startColor = Color.clear;
        var endColor = Color.black;

        fadeImage.enabled = true;
        fadeImage.color = startColor;
        for (var t = 0f; t < halfFadeTime; t += Time.deltaTime)
        {
            var td = t / halfFadeTime;

            fadeImage.color = Color.Lerp(startColor, endColor, td);
            
            yield return null;
        }

        fadeImage.color = endColor;
        OnFadedCallback?.Invoke();
        yield return new WaitForSeconds(0.5f);
        
        for (var t = 0f; t < halfFadeTime; t += Time.deltaTime)
        {
            var td = t / halfFadeTime;

            fadeImage.color = Color.Lerp(endColor, startColor, td);
            
            yield return null;
        }
        
        OnFadeComplete?.Invoke();
        fadeImage.enabled = false;
    }
    
    private IEnumerator FadeNoIntroCoroutine(float fadeTime, Action OnFadedCallback, Action OnFadeComplete)
    {
        var halfFadeTime = fadeTime / 2f;
        var startColor = Color.clear;
        var endColor = Color.black;

        fadeImage.enabled = true;
        fadeImage.color = endColor;

        fadeImage.color = endColor;
        OnFadedCallback?.Invoke();

        yield return new WaitForSeconds(0.5f);
        
        for (var t = 0f; t < halfFadeTime; t += Time.deltaTime)
        {
            var td = t / halfFadeTime;

            fadeImage.color = Color.Lerp(endColor, startColor, td);
            
            yield return null;
        }
        
        OnFadeComplete?.Invoke();
        fadeImage.enabled = false;
    }
    
    //================================================================================================================//
}
