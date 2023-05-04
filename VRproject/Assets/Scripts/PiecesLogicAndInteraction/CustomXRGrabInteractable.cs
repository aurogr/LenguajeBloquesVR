using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomXRGrabInteractable : XRGrabInteractable
{
    [SerializeField] Transform _rightHandGrabAttachPoint;
    [SerializeField] Transform _leftHandGrabAttachPoint;
    [SerializeField] Transform _socketAttachPoint;
    [SerializeField] MeshRenderer _mesh;

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

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        _mesh.material.SetColor("_MultipliedColor", new Color(1, 0, 0, 1));
        base.OnHoverEntered(args);
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        _mesh.material.SetColor("_MultipliedColor", new Color(1, 1, 1, 1));
        base.OnHoverExited(args);
    }
}
