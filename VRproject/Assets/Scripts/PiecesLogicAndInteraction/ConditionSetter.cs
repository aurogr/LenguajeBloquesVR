using UnityEngine;

public class ConditionSetter : MonoBehaviour
{
    GameConditions _pieceConditionSide = GameConditions.Left;
    GameConditions _pieceConditionColor = GameConditions.Red;
    int _pieceConditionTimes = 0;

    /// <summary>
    /// Get selected condition of a conditional piece
    /// </summary>
    public GameConditions GetConditionColor()
    {
        return _pieceConditionColor;
    }

    /// <summary>
    /// Set condition (should only be used in a dropdown event)
    /// </summary>
    public void SetConditionColor(int pieceCondition)
    {
        switch (pieceCondition)
        {
            case 0:
                _pieceConditionColor = GameConditions.Red;
                break;
            default:
                _pieceConditionColor = GameConditions.Blue;
                break;
        }
    }
    /// <summary>
    /// Get selected condition of a conditional piece
    /// </summary>
    public GameConditions GetConditionSide()
    {
        return _pieceConditionSide;
    }

    /// <summary>
    /// Set condition (should only be used in a dropdown event)
    /// </summary>
    public void SetConditionSide(int pieceCondition)
    {
        switch (pieceCondition)
        {
            case 0:
                _pieceConditionSide = GameConditions.Left;
                break;
            default:
                _pieceConditionSide = GameConditions.Right;
                break;
        }
    }
    /// <summary>
    /// Get selected condition of a conditional piece
    /// </summary>
    public int GetConditionTimes()
    {
        return _pieceConditionTimes;
    }

    /// <summary>
    /// Set condition (should only be used in a dropdown event)
    /// </summary>
    public void SetConditionTimes(int pieceCondition)
    {
        _pieceConditionTimes = pieceCondition;
    }
}
