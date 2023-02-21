using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TrashSocketInteractor : XRSocketInteractor
{
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        PuzzlePieceInteractableObject[] puzzlePieces = args.interactableObject.transform.gameObject.GetComponentsInChildren<PuzzlePieceInteractableObject>();

        Debug.Log("An object entered the trash");

        for (int i = (puzzlePieces.Length - 1); i >= 0; i--)
        {
            if (i != 0)
            {
                puzzlePieces[i - 1].DeactivateSocket();
            }
            puzzlePieces[i].Recycle();
        }

        base.OnSelectEntered(args);
    }
}
