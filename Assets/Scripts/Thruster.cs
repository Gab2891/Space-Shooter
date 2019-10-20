using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : MonoBehaviour
{
    SpriteRenderer _thrusterSprite;
    [SerializeField]
    Color _normalColor = new Color(1.0f, 1.0f, 1.0f, 0.7f);
    [SerializeField]
    Color _boostedColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    bool _boosted = false;

    // Start is called before the first frame update
    void Start()
    {
        _thrusterSprite = GetComponent<SpriteRenderer>();
        if (_thrusterSprite == null)
            Debug.LogError("Thruster:: Sprite Renderer is null");
        else
            _thrusterSprite.color = _normalColor;
    }

    public void Boost()
    {
        _boosted = true;
        _thrusterSprite.color = _boostedColor;
    }

    public void Normal()
    {
        _boosted = false;
        _thrusterSprite.color = _normalColor;
    }

    public bool IsBossted()
    {
        return _boosted;
    }
}
