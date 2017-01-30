using UnityEngine;

namespace Assets
{
    public enum Rank
    {
        KING = 0,
        BISHOP,
        ELEPHANT,
        CASTLE,
        HORSE,
        CANNON,
        PAWN
    }

    public enum Team
    {
        RED = 0,
        BLACK,
        NONE
    }

    public class Piece
    {
        public Rank _rank;
        public Team _team;

        public Piece(Rank rank, Team team)
        {
            _rank = rank;
            _team = team;
        }

        public Rank GetRank()
        {
            return _rank;
        }

        public Team GetTeam()
        {
            return _team;
        }

        // Can take a piece if you have a rank advantage or its a pawn taking a king.
        public bool CanTake(Piece other)
        {
            return _team != other.GetTeam() && _rank != Rank.CANNON && 
                (_rank <= other.GetRank() || (_rank == Rank.PAWN && other.GetRank() == Rank.KING));
        }

        public void OnMouseOver()
        {
            Debug.Log("You are over me! Rank: " + _rank + " Team: " + _team);
        }

        public override string ToString()
        {
            return "Rank: " + _rank.ToString() +
                ", Team: " + _team.ToString();
        }

        public override bool Equals(object obj)
        {
            var piece = obj as Piece;

            if (piece == null)
            {
                return false;
            }

            return _rank == piece.GetRank() && 
                _team == piece.GetTeam();
        }

        public override int GetHashCode()
        {
            return (int)GetRank();
        }
    }
}
