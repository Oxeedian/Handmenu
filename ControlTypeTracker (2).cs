using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlTypeTracker : MonoBehaviour
{
    public enum ControllerType
    {
        Hands,
        Controler
    }

    //OVRCameraRigInteraction > OVRCameraRig > OVRInteractionComprehensive > OVRHands > LeftHangGrabSynthetic > OVRLeftHandVisual > OculusHand_L
    [SerializeField] private SkinnedMeshRenderer leftHandMeshNode;

    [SerializeField] private Transform leftHandAnimatorTransform;
    [SerializeField] private Transform leftControllerVisualTransform;
    [SerializeField] private Vector3 handPos = new Vector3(0, 0.05f, 0.05f);
    [SerializeField] private Vector3 controllerPos = new Vector3(0.06f, 0, 0);

    private ControllerType controllerType = ControllerType.Controler;



    void Start()
    {
        transform.SetParent(leftControllerVisualTransform);
    }

    void Update()
    {
        if(leftHandMeshNode.enabled)
        {
            controllerType = ControllerType.Hands;
            transform.SetParent(leftHandAnimatorTransform);
            transform.localRotation = Quaternion.Euler(-90, -125, 0);
            transform.localPosition = handPos;
        }
        else
        {
            controllerType = ControllerType.Controler;
            transform.SetParent(leftControllerVisualTransform);
            transform.localRotation = Quaternion.Euler(0, 90, 45);
            transform.localPosition = controllerPos;
        }
        transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
    }

    public ControllerType GetControllerType()
    {
         return controllerType;
    }
}
