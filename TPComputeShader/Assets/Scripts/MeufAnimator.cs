using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeufAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerController controller;

    // Update is called once per frame
    void Update()
    {
        _animator.SetBool("Jumping", !controller.onGround);

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            _animator.SetBool("Walking", true);
        else _animator.SetBool("Walking", false);
    }
}
