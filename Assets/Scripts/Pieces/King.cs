using UnityEngine;
using UnityEngine.UI;

public class King : BasePiece
{

    private Rook mLeftRook = null;
    private Rook mRightRook = null;

    public override void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        // Base setup
        base.Setup(newTeamColor, newSpriteColor, newPieceManager);

        // Queen stuff
        mMovement = new Vector3Int(1, 1, 1);
        GetComponent<Image>().sprite = Resources.Load<Sprite>("T_King");
    }

    public override void Kill()
    {
        base.Kill();

        mPieceManager.mIsKingsAlive = false;
    }

    protected override void CheckPathing()
    {
        base.CheckPathing();

        // Right
        mRightRook = GetRook(1, 3);

        // Left
        mLeftRook = GetRook(-1, 4);
    }

    protected override void Move()
    {
        base.Move();


        // Left
        if (CanCastle(mLeftRook))
            mLeftRook.Castle();

        //Right
        if (CanCastle(mRightRook))
            mRightRook.Castle();
    }

    private bool CanCastle(Rook rook)
    {
        if (rook == null)
            return false;
    
        if(rook.mCastleTriggerCell != mCurrentCell)
            return false;

        return true;
    }

    private Rook GetRook(int direction, int count)
    {
        // Has the king moved?
        if (!mIsFirstMove)
            return null;

        // Position
        int currentX = mCurrentCell.mBoardPosition.x;
        int currentY = mCurrentCell.mBoardPosition.y;

        // Go through the cells
        for(int i = 1; i < count; i++)
        {
            int offsetX = currentX + (i * direction);
            CellState cellState = mCurrentCell.mBoard.ValidateCell(offsetX, currentY, this);

            if (cellState != CellState.Free)
                return null;
        }

        // Try and get rook
        Cell rookCell = mCurrentCell.mBoard.mAllCells[currentX + (count * direction), currentY];
        Rook rook = null;


        // Cast
        if (rookCell.mCurrentPiece != null)
        {
            if (rookCell.mCurrentPiece is Rook)
            {
                rook = (Rook)rookCell.mCurrentPiece;
            }
        }

        // Return if no rook
        if (rook == null)
            return null;

        //Check color and movement
        if (rook.mColor != mColor || !rook.mIsFirstMove)
            return null;


        // Add castle trigger to movement 
        mHighlightedCells.Add(rook.mCastleTriggerCell);

        return rook;
    }

}
