using UnityEngine;

public class EffectFactory : MonoBehaviour
{
    private static EffectFactory _instance;
    
    [SerializeField, Header("Prefabs")]
    private FloatingText floatingTextPrefab;
    [SerializeField]
    private FloatingSprite floatingSpritePrefab;

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

    public static FloatingText CreateFloatingText()
    {
        return Instantiate(_instance.floatingTextPrefab, _instance.transform, false);
    }
    
    public static FloatingSprite CreateFloatingSprite()
    {
        return Instantiate(_instance.floatingSpritePrefab, _instance.transform, false);
    }
}
