using System;
using UnityEngine;

public class CuttingAnimationController : MonoBehaviour
{
    public Animator anim;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            anim.SetBool("isCutting", true);
        }
        else
        {
            anim.SetBool("isCutting", false);
        }
    }
}
