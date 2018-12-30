using UnityEngine;
using UnityEngine.UI;

public class Pawn : BasePiece
{
    public override void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        // Base setup
        base.Setup(newTeamColor, newSpriteColor, newPieceManager);

        // Pawn stuff
        mMovement = mColor == Color.white ? new Vector3Int(0, 1, 1) : new Vector3Int(0, -1, -1);
        GetComponent<Image>().sprite = Resources.Load<Sprite>("T_Pawn");
    }

    protected override void Move()
    {
        base.Move();

        mIsFirstMove = false;

        CheckForPromotion();
    }

    private bool MatchesState(int targetX, int targetY, CellState targetState)
    {
        CellState cellState = CellState.None;
        cellState = mCurrentCell.mBoard.ValidateCell(targetX, targetY, this);

        if(cellState == targetState)
        {
            mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[targetX, targetY]);
            return true;
        }

        return false;
    }

    private void CheckForPromotion()
    {
        // Target position
        int targetX = mCurrentCell.mBoardPosition.x;
        int targetY = mCurrentCell.mBoardPosition.y;

        CellState cellState = mCurrentCell.mBoard.ValidateCell(targetX, targetY + mMovement.y, this);

        if(cellState == CellState.OutOfBounds)
        {
            Color spriteColor = GetComponent<Image>().color;
            mPieceManager.PromotePiece(this, mCurrentCell, mColor, spriteColor);
        }
    }

    protected override void CheckPathing()
    {
        // Target position
        int targetX = mCurrentCell.mBoardPosition.x;
        int targetY = mCurrentCell.mBoardPosition.y;

        // Top left
        MatchesState(targetX - mMovement.z, targetY + mMovement.z, CellState.Enemy);

        // Forward
        if(MatchesState(targetX, targetY + mMovement.y, CellState.Free))
        {
            // If the first forward cell is free, and first move, check för next
            if(mIsFirstMove)
            {
                MatchesState(targetX, targetY + mMovement.y * 2, CellState.Free);
            }
        }

        // Top right
        MatchesState(targetX + mMovement.z, targetY + mMovement.z, CellState.Enemy);

    }
}
