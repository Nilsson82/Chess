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
        int currentX = mCurrentCell.mBoardPosition.x;
        int currentY = mCurrentCell.mBoardPosition.y;

        

        // Check each cell
        for (int i = 1; i <= movement; i++)
        {
            currentX += xDircetion;
            currentY += yDirection;

            // TODo: Get the state of target cell

            // Add to list
            mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[currentX, currentY]);

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


        Debug.Log("Size: " + mHighlightedCells.Count);

        foreach (Cell cell in mHighlightedCells)
        {
            Debug.Log("Foreath: ");
            Debug.Log("Enable: " + cell.mOutlineImage.enabled);
            cell.mOutlineImage.enabled = true;
        }
        Debug.Log("End: ");
    }

    protected void ClearCells()
    {
        foreach (Cell cell in mHighlightedCells)
        {
            cell.mOutlineImage.enabled = false;
        }
        mHighlightedCells.Clear();
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
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        // Hide
        ClearCells();
    }

    #endregion
}

