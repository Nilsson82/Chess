using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public abstract class BasePiece : EventTrigger
{
    [HideInInspector]
    public Color mColor = Color.clear;
    //public bool mIsFirstMove = true;

    protected Cell mOriginalCell = null;
    protected Cell mCurrentCell = null;

    protected RectTransform mRectTransform = null;
    protected PieceManager mPieceManager;

    protected Vector3Int mMovement = Vector3Int.one;
    protected List<Cell> mHighlightedCells = new List<Cell>();

    protected Cell mTargetCell = null;

    public virtual void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        mPieceManager = newPieceManager;

        mColor = newTeamColor;
        GetComponent<Image>().color = newSpriteColor;
        mRectTransform = GetComponent<RectTransform>();
    }

    public virtual void Place(Cell newCell)
    {
        // Cell stuff
        mCurrentCell = newCell;
        mOriginalCell = newCell;
        mCurrentCell.mCurrentPiece = this;

        // Object stuff
        transform.position = newCell.transform.position;
        gameObject.SetActive(true);
    }

    private void CreateCellPath(int xDircetion, int yDirection, int movement)
    {
        // Target possition
        int targetX = mCurrentCell.mBoardPosition.x;
        int targetY = mCurrentCell.mBoardPosition.y;



        // Check each cell
        for (int i = 1; i <= movement; i++)
        {
            targetX += xDircetion;
            targetY += yDirection;

            // Get the state of the taget cell
            CellState cellState = CellState.None;
            cellState = mCurrentCell.mBoard.ValidateCell(targetX, targetY, this);

            // If enemy, add to list, break
            if(cellState == CellState.Enemy)
            {
                mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[targetX, targetY]);
                break;
            }

            // If the cell is not free, break
            if(cellState != CellState.Free)
                break;
        
            // Add to list
            mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[targetX, targetY]);

        }

    }

    protected virtual void CheckPathing()
    {
        // Horizontal
        CreateCellPath(1, 0, mMovement.x);
        CreateCellPath(-1, 0, mMovement.x);

        // Vertical
        CreateCellPath(0, 1, mMovement.y);
        CreateCellPath(0, -1, mMovement.y);

        // Upper diagonal
        CreateCellPath(1, 1, mMovement.z);
        CreateCellPath(-1, 1, mMovement.z);

        // Lower diagonal
        CreateCellPath(-1, -1, mMovement.z);
        CreateCellPath(1, -1, mMovement.z);
    }

    protected void ShowCells()
    {
        foreach (Cell cell in mHighlightedCells)
        {
            cell.mOutlineImage.enabled = true;
        }
    }

    protected void ClearCells()
    {
        foreach (Cell cell in mHighlightedCells)
        {
            cell.mOutlineImage.enabled = false;
        }
        mHighlightedCells.Clear();
    }

    public virtual void Reset()
    {
        Kill();

        Place(mOriginalCell);
    }


    public virtual void Kill()
    {
        // Clerar current cell
        mCurrentCell.mCurrentPiece = null;

        // Remove piece
        gameObject.SetActive(false);
    }

    protected virtual void Move()
    {
        // If there us an enemy piece, remove it
        mTargetCell.RemovePiece();

        // Clear current
        mCurrentCell.mCurrentPiece = null;

        // Switch cells
        mCurrentCell = mTargetCell;
        mCurrentCell.mCurrentPiece = this;

        // Move on board
        transform.position = mCurrentCell.transform.position;
        mTargetCell = null;
    }


    #region Events

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        //Test for cells
        CheckPathing();

        // Show valid cells
        ShowCells();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        
        // Follow pointer
        transform.position += (Vector3)eventData.delta;

        // Check for overlapping avaiable squares
        foreach(Cell cell in mHighlightedCells)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(cell.mRectTransform, Input.mousePosition))
            {
                // If the mouse is within a valid cell, get it, and break.
                mTargetCell = cell;
                break;
            }

            // If the mouse is not within any highlighted cell, we don't have a valid move
            mTargetCell = null;
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        // Hide
        ClearCells();

        // Return to original position
        if(!mTargetCell)
        {
            transform.position = mCurrentCell.gameObject.transform.position;
            return;
        }

        // Move to new cell
        Move();

        // End turn
        mPieceManager.SwitchSides(mColor);
    }

    #endregion
}

