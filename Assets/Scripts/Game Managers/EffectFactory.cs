using UnityEngine;

public class EffectFactory : MonoBehaviour
{
    private static EffectFactory _instance;
    
    [SerializeField, Header("Prefabs")]
    private FloatingText floatingTextPrefab;
    [SerializeField]
    private FloatingSprite floatingSpritePrefab;

    [SerializeField, Header("Particle Prefabs")]
    private ParticleSystem spawningVFXPrefab;
    
    [SerializeField]
    private ParticleSystem smallHitVFXPrefab;
    [SerializeField]
    private ParticleSystem bigHitVFXPrefab;
    [SerializeField]
    private ParticleSystem bumpVFXPrefab;

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

    //Floating VFX
    //================================================================================================================//

    public static FloatingText CreateFloatingText()
    {
        return Instantiate(_instance.floatingTextPrefab, _instance.transform, false);
    }
    
    public static FloatingSprite CreateFloatingSprite()
    {
        return Instantiate(_instance.floatingSpritePrefab, _instance.transform, false);
    }
    
    //Particle VFX
    //================================================================================================================//

    public static ParticleSystem CreateSpawningVFX()
    {
        return Instantiate(_instance.spawningVFXPrefab, _instance.transform, false);
    }
    
    public static ParticleSystem CreateSmallHitVFX()
    {
        return Instantiate(_instance.smallHitVFXPrefab, _instance.transform, false);
    }
    public static ParticleSystem CreateBigHitVFX()
    {
        return Instantiate(_instance.bigHitVFXPrefab, _instance.transform, false);
    }
    public static ParticleSystem CreateBumpVFX()
    {
        return Instantiate(_instance.bumpVFXPrefab, _instance.transform, false);
    }
    
    //================================================================================================================//

}
