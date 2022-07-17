using UnityEngine;

public class Door : MonoBehaviour
{
    private bool _isOpen;

    [SerializeField, Header("Sprites")] private SpriteRenderer[] spriteRenderers;
    [SerializeField] private Sprite[] openSprites;
    [SerializeField] private Sprite[] closedSprites;

    private Collider Collider
    {
        get
        {
            if (_collider == null)
                _collider = GetComponent<Collider>();

            return _collider;
        }
    }
    private Collider _collider;

    public void SetDoorOpen(in bool isOpen)
    {
        _isOpen = isOpen;
        Collider.enabled = isOpen;

        for (var i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = _isOpen ? openSprites[i] : closedSprites[i];
        }
    }

}
