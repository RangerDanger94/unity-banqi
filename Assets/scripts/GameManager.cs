using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Assets;

public class GameManager : MonoBehaviour {
    public Sprite[] blackSprites;
    public Sprite[] redSprites;
    public Sprite unflipped;
    public Sprite redTeam;
    public Sprite blackTeam;

    public static Dictionary<Piece, Sprite> spriteMap = new Dictionary<Piece, Sprite>();
    public List<Piece> pieces = new List<Piece>();
    public static Board board;
    public static GameObject turnIndicator;
    private static Team turn = Team.NONE;

    private static GameManager instance = null;
    public static GameManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

	// Use this for initialization
	void Start () {
        turnIndicator = GameObject.FindGameObjectWithTag("TurnIndicator");
        LoadSprites();
        InitPieces();

        ShufflePieces();
        board = new Board(pieces, unflipped);
	}

    private void Update()
    {
        SetTurnIndicator();
    }

    public static void SelectPiece(int x, int y)
    {
        if (board.ValidPiece(x, y, turn) || turn == Team.NONE)
        {
            if (board.UpdatePiece(x, y)) {
                if (turn == Team.RED)
                {
                    turn = Team.BLACK;
                }
                else if (turn == Team.BLACK)
                {
                    turn = Team.RED;
                }

                if (turn == Team.NONE)
                {
                    turn = board.GetPiece(x, y).GetTeam(); 
                    if (turn == Team.RED)
                    {
                        turn = Team.BLACK;
                    }
                    else if (turn == Team.BLACK)
                    {
                        turn = Team.RED;
                    }
                }
            }
        }

        
    }

    private void SetTurnIndicator()
    {
        if (turn == Team.RED)
        {
            turnIndicator.GetComponent<SpriteRenderer>().sprite = redTeam;
        } 
        else if (turn == Team.BLACK)
        {
            turnIndicator.GetComponent<SpriteRenderer>().sprite = blackTeam;
        }
    }

    private void LoadSprites()
    {
        // Map sprites 
        for(Rank i = Rank.KING; i <= Rank.PAWN; ++i)
        {
            Piece bP = new Piece(i, Team.BLACK);
            spriteMap[bP] = blackSprites[(int)i];

            Piece rP = new Piece(i, Team.RED);
            spriteMap[rP] = redSprites[(int)i];
        }
    }

    private void InitPieces()
    {
        pieces.Clear();

        // Kings (1 Piece)
        pieces.Add(new Piece(Rank.KING, Team.RED));
        pieces.Add(new Piece(Rank.KING, Team.BLACK));

        // Bishops -> Cannons (2 Pieces)
        for (Rank r = Rank.BISHOP; r <= Rank.CANNON; ++r)
        {
            for (int i = 0; i < 2; ++i)
            {
                pieces.Add(new Piece(r, Team.RED));
                pieces.Add(new Piece(r, Team.BLACK));
            }
        }

        // Pawns (5 Pieces)
        for (int i = 0; i < 5; ++i)
        {
            pieces.Add(new Piece(Rank.PAWN, Team.RED));
            pieces.Add(new Piece(Rank.PAWN, Team.BLACK));
        }
    }

    private void ShufflePieces()
    {
        System.Random rnd = new System.Random();
        pieces = pieces.OrderBy(x => rnd.Next()).ToList();
    }

    public static Sprite GetSprite(Piece p)
    {
        return spriteMap[p];
    }
}
