using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private bool startOpen;
    private bool _isOpen;


    [SerializeField, Header("Sprites")] private SpriteRenderer[] spriteRenderers;
    [SerializeField] private Sprite[] openSprites;
    [SerializeField] private Sprite[] closedSprites;


    private Collider _collider;

    // Start is called before the first frame update
    private void Start()
    {
        SetDoorOpen(startOpen);
    }

    public void SetDoorOpen(in bool isOpen)
    {
        _isOpen = isOpen;

        for (var i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = _isOpen ? openSprites[i] : closedSprites[i];
        }
        
    }

}
