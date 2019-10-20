using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    SpriteRenderer _shieldSprite;
    [SerializeField]
    Color _fullColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    [SerializeField]
    Color _mediumColor = new Color(1.0f, 1.0f, 1.0f, 0.6f);
    [SerializeField]
    Color _lowColor = new Color(1.0f, 1.0f, 1.0f, 0.3f);
    [SerializeField]
    int _maxDamageResistance = 3;
    int _damageResistance;

    // Start is called before the first frame update
    void Start()
    {
        _shieldSprite = GetComponent<SpriteRenderer>();
        if (_shieldSprite == null)
            Debug.LogError("Shield: Sprite Renderer is null");
        else
            TurnOff();
    }

    public void TurnOn()
    {
        _damageResistance = _maxDamageResistance;
        _shieldSprite.color = _fullColor;
        _shieldSprite.enabled = true;
    }

    public bool ReceiveDamage()
    {
        --_damageResistance;
        switch (_damageResistance)
        {
            case 2:
                _shieldSprite.color = _mediumColor;
                break;
            case 1:
                _shieldSprite.color = _lowColor;
                break;
            default:
                TurnOff();
                break;
        }
        return _shieldSprite.enabled;
        
    }

    public void TurnOff()
    {
        _shieldSprite.enabled = false;
    }

    public int DamageResistance()
    {
        return _damageResistance;
    }

    public bool IsActive()
    {
        return _shieldSprite.enabled;
    }
}
