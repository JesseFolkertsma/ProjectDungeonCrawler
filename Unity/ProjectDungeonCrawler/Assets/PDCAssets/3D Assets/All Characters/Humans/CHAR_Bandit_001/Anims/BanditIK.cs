using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanditIK : MonoBehaviour {

    public Transform cam;
    Animator anim;
    Transform leftIKPos;
    Transform rightIKPos;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void SetLeftIKPosition(Transform left)
    {
        leftIKPos = left;
    }

    public void SetRightIKPosition(Transform right)
    {
        rightIKPos = right;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (anim != null)
        {
            if (cam != null)
            {
                Vector3 lookat = cam.position + cam.forward * 20;
                anim.SetLookAtPosition(lookat);
                anim.SetLookAtWeight(1f, .25f, 1f, 1f, 0f);
            }

            if (rightIKPos != null)
            {
                anim.SetIKPosition(AvatarIKGoal.RightHand, rightIKPos.position);
                anim.SetIKRotation(AvatarIKGoal.RightHand, rightIKPos.rotation);
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
            }

            if (leftIKPos != null)
            {
                anim.SetIKPosition(AvatarIKGoal.LeftHand, leftIKPos.position);
                anim.SetIKRotation(AvatarIKGoal.LeftHand, leftIKPos.rotation);
                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
            }
        }
    }
}
