using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

public class ConditionSocketInteractor : XRSocketInteractor
{
    public GameConditions GetConditionPiece()
    {
        IXRSelectInteractable obj = this.GetOldestInteractableSelected();
        GameObject conditionPiece = obj.transform.gameObject;

        return conditionPiece.GetComponent<PuzzlePieceInteractableObject>().GetConditionType();
    }
}
