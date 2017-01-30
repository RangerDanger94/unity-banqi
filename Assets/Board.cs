using UnityEngine;
using System.Collections.Generic;

namespace Assets
{
    struct Square
    {
        public Piece piece;
        public bool flipped;
        public GameObject uObject;
    }

    public class Point2D
    {
        public int x;
        public int y;

        public Point2D(int pX, int pY)
        {
            x = pX;
            y = pY;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Point2D;
            return x == other.x && y == other.y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "X: " + x + " Y: " + y;
        }
    }

    public class Board 
    {
        public static int Width = 8;
        public static int Height = 4;

        private Square[,] spaces = new Square[Width, Height];

        private Point2D selected = null;
        private List<Point2D> takes = new List<Point2D>();

        public Board(List<Piece> configuration, Sprite unflipped)
        {
            int pCounter = 0;

            for(int x = 0; x < Width; ++x)
            {
                for(int y = 0; y < Height; ++y)
                {
                    spaces[x, y].piece = configuration[pCounter];
                    spaces[x, y].flipped = false;

                    
                    pCounter++;

                    spaces[x, y].uObject = new GameObject();
                    spaces[x, y].uObject.transform.position = new Vector3(x, y);
                    spaces[x, y].uObject.AddComponent<SpriteRenderer>();
                    spaces[x, y].uObject.GetComponent<SpriteRenderer>().sprite =
                       unflipped;

                    spaces[x, y].uObject.AddComponent<BoxCollider2D>();
                    spaces[x, y].uObject.AddComponent<InputHandler>();
                }
            }
        }

        public Piece GetPiece(int x, int y)
        {
            Debug.Log(spaces[x, y].piece.ToString());
            return spaces[x, y].piece;
        }

        // Return true when a turn is completed
        public bool UpdatePiece(int x, int y)
        {
            Point2D current = new Point2D(x, y);

            // Flip & End Turn
            if (!spaces[x, y].flipped)
            {
                Debug.Log("Flip & End Turn");
                spaces[x, y].uObject.GetComponent<SpriteRenderer>().sprite =
                        GameManager.GetSprite(spaces[x, y].piece);
                spaces[x, y].flipped = true;
                DeselectPiece();
            }
            // Select Piece
            else if (selected == null)
            {
                Debug.Log("Select Piece");
                if(spaces[x, y].piece != null)
                {
                    SelectPiece(x, y);
                }

                return false;
            }
            // Move & End Turn
            else if (spaces[x, y].piece == null)
            {
                Debug.Log("Moving Piece & End Turn");
                // Remove captured sprite and replace it with capturing one
                spaces[x, y].uObject.GetComponent<SpriteRenderer>().sprite =
                    spaces[selected.x, selected.y].uObject.GetComponent<SpriteRenderer>().sprite;

                spaces[selected.x, selected.y].uObject.GetComponent<SpriteRenderer>().sprite = null;

                // Remove capture piece and replace it with capturing one
                spaces[x, y].piece = spaces[selected.x, selected.y].piece;
                spaces[selected.x, selected.y].piece = null;

                DeselectPiece();
            }
            else
            {
                Piece selectedPiece = spaces[selected.x, selected.y].piece;
                Piece currentPiece = spaces[current.x, current.y].piece;

                // Toggle Selected Piece
                if (current.Equals(selected))
                {
                    Debug.Log("Toggle Piece");
                    DeselectPiece();
                    return false;
                }

                // Change Selected Piece
                else if (selectedPiece.GetTeam() == currentPiece.GetTeam())
                {
                    Debug.Log("Change Piece");
                    DeselectPiece();
                    SelectPiece(x, y);
                    return false;
                }
                // Capture & End Turn
                else
                {
                    Debug.Log("Capture & End Turn");
                    // Remove captured sprite and replace it with capturing one
                    spaces[x, y].uObject.GetComponent<SpriteRenderer>().sprite =
                        spaces[selected.x, selected.y].uObject.GetComponent<SpriteRenderer>().sprite;

                    spaces[selected.x, selected.y].uObject.GetComponent<SpriteRenderer>().sprite = null;

                    // Remove capture piece and replace it with capturing one
                    spaces[x, y].piece = selectedPiece;
                    spaces[selected.x, selected.y].piece = null;

                    DeselectPiece();
                }
            }

            return true;
        }

        public Point2D GetSelected()
        {
            return selected;
        }

        private Point2D CannonUp()
        {
            int x = selected.x;
            int y = selected.y;

            for (int i = y + 1; i < Height; ++i)
            {
                if (spaces[x, i].piece != null)
                {
                    for (int j = i + 1; j < Height; ++j)
                    {
                        if (spaces[x, j].piece != null && spaces[x, j].flipped)
                        {
                            if (spaces[x, j].piece.GetTeam() != spaces[x, y].piece.GetTeam())
                            {
                                return new Point2D(x, j);
                            }
                            else
                            {
                                return null;
                            }
                        }  
                    }
                }
            }

            return null;
        }

        private Point2D CannonDown()
        {
            int x = selected.x;
            int y = selected.y;

            for (int i = y - 1; i >= 0; --i)
            {
                if (spaces[x, i].piece != null)
                {
                    for (int j = i - 1; j >= 0; --j)
                    {
                        if (spaces[x, j].piece != null && spaces[x, j].flipped)
                        {
                            if (spaces[x, j].piece.GetTeam() != spaces[x, y].piece.GetTeam())
                            {
                                return new Point2D(x, j);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }

            return null;
        }

        private Point2D CannonLeft()
        {
            int x = selected.x;
            int y = selected.y;

            for (int i = x - 1; i >= 0; --i)
            {
                if (spaces[i, y].piece != null)
                {
                    for (int j = i - 1; j >= 0; --j)
                    {
                        if (spaces[j, y].piece != null && spaces[j, y].flipped)
                        {
                            if (spaces[j, y].piece.GetTeam() != spaces[x, y].piece.GetTeam())
                            {
                                return new Point2D(j, y);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }

            return null;
        }

        private Point2D CannonRight()
        {
            int x = selected.x;
            int y = selected.y;

            for (int i = x + 1; i < Width; ++i)
            {
                if (spaces[i, y].piece != null)
                {
                    for (int j = i + 1; j < Width; ++j)
                    {
                        if (spaces[j, y].piece != null && spaces[j, y].flipped)
                        {
                            if (spaces[j, y].piece.GetTeam() != spaces[x, y].piece.GetTeam())
                            {
                                return new Point2D(j, y);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }

            return null;
        }

        private void ShowCannonTakes()
        {
            // Cannon
            Point2D cResult = CannonUp();
            if (cResult != null)
            {
                takes.Add(cResult);
            }

            cResult = CannonDown();
            if (cResult != null)
            {
                takes.Add(cResult);
            }

            cResult = CannonLeft();
            if (cResult != null)
            {
                takes.Add(cResult);
            }

            cResult = CannonRight();
            if (cResult != null)
            {
                takes.Add(cResult);
            }
        }

        private bool CanTake(int x, int y, Piece p)
        {
            if (spaces[x,y].piece != null)
            {
                return spaces[x, y].flipped && p.CanTake(spaces[x, y].piece);
            }

            return false;
        }

        // Checks grid against selected piece for any pieces that can be taken
        private void ShowTakes()
        {
            int x = selected.x;
            int y = selected.y;
            Piece selectedPiece = spaces[selected.x, selected.y].piece;

            // Check cell above current
            if (y < Height - 1)
            {
                if (CanTake(x, y+1, selectedPiece)) //(spaces[x, y + 1].flipped && spaces[x, y].piece.CanTake(spaces[x, y + 1].piece))
                {
                    takes.Add(new Point2D(x, y + 1));
                }
            }


            // Check cell to the left of current
            if (x > 0)
            {
                if (CanTake(x-1, y, selectedPiece)) // (spaces[x - 1, y].flipped && spaces[x, y].piece.CanTake(spaces[x - 1, y].piece))
                {
                    takes.Add(new Point2D(x - 1, y));
                }
            }

            // Check cell to the right of current
            if (x < Width - 1)
            {
                if (CanTake(x+1, y, selectedPiece)) // if (spaces[x + 1, y].flipped && spaces[x, y].piece.CanTake(spaces[x + 1, y].piece))
                {
                    takes.Add(new Point2D(x + 1, y));
                }
            }

            // Check cell below current
            if (y > 0)
            {
                if (CanTake(x, y-1, selectedPiece)) //if (spaces[x, y - 1].flipped && spaces[x, y].piece.CanTake(spaces[x, y - 1].piece))
                {
                    takes.Add(new Point2D(x, y - 1));
                }
            }

            if (spaces[x, y].piece.GetRank() == Rank.CANNON)
            {
                ShowCannonTakes();
            }
            

            foreach (Point2D take in takes)
            {
                spaces[take.x, take.y].uObject.GetComponent<SpriteRenderer>().color = Color.blue;
            }
        }

        // Removes highlights for takeable pieces
        private void UnshowTakes()
        {
            foreach (Point2D take in takes)
            {
                spaces[take.x, take.y].uObject.GetComponent<SpriteRenderer>().color = Color.white;
            }

            takes.Clear();
        }

        private void SelectPiece(int x, int y)
        {
            selected = new Point2D(x, y);
            spaces[x, y].uObject.GetComponent<SpriteRenderer>().color = Color.yellow; 
            ShowTakes();
        }

        private void DeselectPiece()
        {
            if (selected == null)
                return;

            spaces[selected.x, selected.y].uObject.GetComponent<SpriteRenderer>().color = Color.white;
            UnshowTakes();
            selected = null;    
        }

        // Valid if:
        // - Not flipped
        // - On playing team
        // - Capturable piece && capturing piece has been selected
        // - Empty space
        public bool ValidPiece(int x, int y, Team turn)
        {
            Point2D current = new Point2D(x, y);
            
            // Takes will be empty if no piece has been selected
            bool canMove = false;
            foreach (Point2D take in takes)
            {
                if (current.Equals(take))
                {
                    canMove = true;
                    break;
                }
            }

            canMove |= spaces[x, y].piece == null;

            if (canMove)
            {
                return true;
            }
           
            return !spaces[x, y].flipped || spaces[x, y].piece.GetTeam() == turn;
        }
    }
}
