using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
            Debug.LogError("Animator is null");
    }
    
    public void LeftMovOn()
    {
        _animator.SetBool("LeftMov", true);
    }

    public void LeftMovOff()
    {
        _animator.SetBool("LeftMov", false);
    }

    public void RightMovOn()
    {
        _animator.SetBool("RightMov", true);
    }

    public void RightMovOff()
    {
        _animator.SetBool("RightMov", false);
    }
}
