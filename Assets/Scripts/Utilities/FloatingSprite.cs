using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingSprite : MonoBehaviour
{
    public enum TYPE
    {
        NONE,
        SKULL,
        TOKEN
    }
    [SerializeField] private TMP_Text tmpText;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer backgroundRenderer;

    [SerializeField] private AnimationCurve alphaCurve;
    [SerializeField] private AnimationCurve riseCurve;
    [SerializeField, Min(0.1f)] private float riseTime;

    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private float addHeight;

    private bool _ready;

    //================================================================================================================//

    public void SetFloatingValues(in Vector3 startWorldPosition, in TYPE spriteType)
    {
        transform.position = startWorldPosition;

        spriteRenderer.sprite = sprites[(int)spriteType];
        
        StartCoroutine(FadeCoroutine());

        _ready = true;
    }

    private IEnumerator FadeCoroutine()
    {
        var startPos = transform.position;
        var endPos = startPos + Vector3.up * addHeight;
        
        var startTextColor = tmpText.color;
        var endTextColor = startTextColor;
        endTextColor.a = 0f;
        
        var startSpriteColor = spriteRenderer.color;
        var endSpriteColor = startSpriteColor;
        endSpriteColor.a = 0f;
        
        var startBackgroundColor = backgroundRenderer.color;
        var endBackgroundColor = startBackgroundColor;
        endBackgroundColor.a = 0f;

        for (var t = 0f; t < riseTime; t += Time.deltaTime)
        {
            var td = riseCurve.Evaluate(t / riseTime);
            var ad = alphaCurve.Evaluate(t / riseTime);

            transform.position = Vector3.Lerp(startPos, endPos, td);
            
            tmpText.color = Color.Lerp(startTextColor, endTextColor, ad);
            spriteRenderer.color = Color.Lerp(startSpriteColor, endSpriteColor, ad);
            backgroundRenderer.color = Color.Lerp(startBackgroundColor, endBackgroundColor, ad);

            yield return null;
        }

        Destroy(gameObject);
    }
}
