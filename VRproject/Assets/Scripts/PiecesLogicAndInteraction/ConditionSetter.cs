using UnityEngine;

public class ConditionSetter : MonoBehaviour
{
    GameConditions _pieceCondition = GameConditions.GoalRed;

    /// <summary>
    /// Get selected condition of a conditional piece
    /// </summary>
    public GameConditions GetCondition()
    {
        return _pieceCondition;
    }

    /// <summary>
    /// Set condition (should only be used in a dropdown event)
    /// </summary>
    public void SetCondition(int pieceCondition)
    {
        switch (pieceCondition)
        {
            case 0:
                _pieceCondition = GameConditions.GoalRed;
                break;
            default:
                _pieceCondition = GameConditions.GoalBlue;
                break;
        }
    }
}
