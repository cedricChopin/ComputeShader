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
        _animator.SetFloat("Speed", controller.rb.velocity.magnitude);
    }
}
