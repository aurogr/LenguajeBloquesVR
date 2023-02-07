using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomXRGrabInteractable : XRGrabInteractable
{
    [SerializeField] Transform _rightHandGrabAttachPoint;
    [SerializeField] Transform _leftHandGrabAttachPoint;
    [SerializeField] Transform _socketAttachPoint;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject.transform.CompareTag("RightHand"))
        {
            attachTransform = _rightHandGrabAttachPoint;
        }
        else if (args.interactorObject.transform.CompareTag("LeftHand"))
        {
            attachTransform = _leftHandGrabAttachPoint;
        }
        else
        {
            attachTransform = _socketAttachPoint;
        }

        base.OnSelectEntered(args);
    }
}
