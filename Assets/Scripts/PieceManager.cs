using System;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    [HideInInspector]
    public bool mIsKingsAlive = true;

    public GameObject mPiecePrefab;

    private List<BasePiece> mWhitePieces = null;
    private List<BasePiece> mBlackPieces = null;

    private string[] mPieceOrder = new string[16]
    {
        "P", "P", "P", "P", "P", "P", "P", "P",
        "R", "KN", "B", "Q", "K", "B", "KN", "R"
    };

    private Dictionary<string, Type> mPieceLibrary = new Dictionary<string, Type>()
    {
        {"P",  typeof(Pawn)},
        {"R",  typeof(Rook)},
        {"KN", typeof(Knight)},
        {"B",  typeof(Bishop)},
        {"K",  typeof(King)},
        {"Q",  typeof(Queen)}
    };

    public void Setup(Board board)
    {
        // Create white pieces
        mWhitePieces = CreatePieces(Color.white, new Color32(80, 124, 159, 255), board);

        // Create black pieces
        mBlackPieces = CreatePieces(Color.black, new Color32(210, 95, 64, 255), board);

        // Place pieces
        PlacePieces(1, 0, mWhitePieces, board);
        PlacePieces(6, 7, mBlackPieces, board);

        // With goes first
        SwitchSides(Color.black);

    }

    private List<BasePiece> CreatePieces(Color teamColor, Color32 spriteColor, Board board)
    {
        List<BasePiece> newPieces = new List<BasePiece>();

        for (int i = 0; i < mPieceOrder.Length; i++)
        {
            // Create new object
            GameObject newPieceObject = Instantiate(mPiecePrefab);
            newPieceObject.transform.SetParent(transform);

            // Set scale and position
            newPieceObject.transform.localScale = new Vector3(1, 1, 1);
            newPieceObject.transform.localRotation = Quaternion.identity;

            // Get the type, apply to new object
            string key = mPieceOrder[i];
            Type pieceType = mPieceLibrary[key];

            // Store new piece
            BasePiece newPiece = (BasePiece)newPieceObject.AddComponent(pieceType);
            newPieces.Add(newPiece);

            // Setup piece
            newPiece.Setup(teamColor, spriteColor, this);

        }

        return newPieces;
    }

    private void PlacePieces(int pawnRow, int royaltyRow, List<BasePiece> pieces, Board board)
    {
        for (int i = 0; i < 8; i++)
        {
            // Place pawns
            pieces[i].Place(board.mAllCells[i, pawnRow]);

            // Place royalty
            pieces[i + 8].Place(board.mAllCells[i, royaltyRow]);
        }
    }

    private void SetInteractive(List<BasePiece> allPieces, bool value)
    {
        foreach (BasePiece piece in allPieces)
        {
            piece.enabled = value;
        }
    }

    public void SwitchSides(Color color)
    {
        if(!mIsKingsAlive)
        {
            // Reset pieces
            ResetPieces();

            // King has risen from the dead
            mIsKingsAlive = true;

            // Change color to black, so with can go first again
            color = Color.black;
        }

        bool isBlackTurn = color == Color.white ? true : false;

        //Set interactivity
        SetInteractive(mBlackPieces, isBlackTurn);
        SetInteractive(mWhitePieces, !isBlackTurn);
    }

    public void ResetPieces()
    {
        // Reset white
        foreach(BasePiece piece in mWhitePieces)
        {
            piece.Reset();
        }

        // Reset black
        foreach (BasePiece piece in mBlackPieces)
        {
            piece.Reset();
        }
    }

}

