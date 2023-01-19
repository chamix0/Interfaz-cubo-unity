using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Important class, it is in charge of interpreting the messages coming from the process. Also of maintaining a reference state of the cube(centers)
/// and creating a queue of the moves read by the cube
/// </summary>
public enum FACES
{
    R,
    L,
    U,
    D,
    F,
    B,
    M,
    E,
    S,
    NULL
}

[DefaultExecutionOrder(3)]
public class CubeInputs : MonoBehaviour

{
    private MovesQueue _messages;
    private CubeConectionManager _conection;
    [SerializeField] private MovesQueue _moves; //must be  in a different object
    private List<char> _validationFaces; //list with the possible incomes that are meant to be moves
    public bool isActive = false;
    public Color topColor = Color.clear, FrontColor = Color.clear;

    public Color[,] centers = new Color[3, 4]
    {
        { Color.white, Color.clear, Color.clear, Color.clear },
        {
            Color.green, Color.red, Color.blue,
            new Color(1f, 0.59f, 0.18f)
        },
        { Color.yellow, Color.clear, Color.clear, Color.clear }
    };

    // Start is called before the first frame update
    private void Awake()
    {
        _validationFaces = new List<char>(new[] { 'R', 'L', 'U', 'D', 'F', 'B' });
    }

    void Start()
    {
        _conection = GetComponent<CubeConectionManager>();
        topColor = centers[0, 0];
        FrontColor = centers[1, 0];
        _messages = GetComponent<MovesQueue>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive && _messages.HasMessages())
        {
            Move move = _messages.Dequeue();
            //check if connection lost
            if (move.msg == "connection failed")
                StartCoroutine(_conection.RestablishComunication());

            //check movements
            else if (move.msg != "" && ValidateMoves(move.msg))
            {
                //double moves
                move = DoubleMove(move);
                _moves.Enqueue(move);
                _moves.lastMove = move;
                print(move.face + " " + move.direction);
            }
        }
    }

    private Move ApplyOffset(Move move)
    {
        FACES face = move.face;
        print("top color is " + topColor + " - " + Color.white);
        if (topColor == Color.white)
        {
            if (FrontColor == Color.green) print("green");
            else if (FrontColor == Color.red)
            {
                move.face = FaceSwap(face, FACES.L, FACES.R, FACES.F, FACES.B, FACES.U, FACES.D);
                print("red");
            }
            else if (FrontColor == Color.blue)
            {
                move.face = FaceSwap(face, FACES.B, FACES.F, FACES.L, FACES.R, FACES.U, FACES.D);
                print("blue");
            }
            else
            {
                move.face = FaceSwap(face, FACES.R, FACES.L, FACES.B, FACES.F, FACES.U, FACES.D);
                print("orange");
            }
        }
        else if (topColor == Color.yellow)
        {
            if (FrontColor == Color.green)
            {
                move.face = FaceSwap(face, FACES.F, FACES.B, FACES.L, FACES.R, FACES.D, FACES.U);
                print("red");
            }
            else if (FrontColor == Color.red)
            {
                move.face = FaceSwap(face, FACES.R, FACES.L, FACES.F, FACES.B, FACES.D, FACES.U);
                print("red");
            }
            else if (FrontColor == Color.blue)
            {
                move.face = FaceSwap(face, FACES.B, FACES.F, FACES.R, FACES.L, FACES.D, FACES.U);
                print("blue");
            }
            else
            {
                move.face = FaceSwap(face, FACES.L, FACES.R, FACES.B, FACES.F, FACES.D, FACES.U);
                print("orange");
            }
        }
        else if (topColor == Color.green)
        {
            if (FrontColor == Color.white)
            {
                move.face = FaceSwap(face, FACES.L, FACES.R, FACES.F, FACES.B, FACES.U, FACES.D);
            }
            else if (FrontColor == Color.red)
            {
                move.face = FaceSwap(face, FACES.U, FACES.D, FACES.F, FACES.B, FACES.R, FACES.L);
                print("red");
            }
            else if (FrontColor == Color.yellow)
            {
                move.face = FaceSwap(face, FACES.U, FACES.D, FACES.R, FACES.L, FACES.B, FACES.F);
                print("blue");
            }
            else
            {
                move.face = FaceSwap(face, FACES.U, FACES.D, FACES.B, FACES.F, FACES.L, FACES.R);
                print("orange");
            }
        }
        else if (topColor == Color.blue)
        {
            if (FrontColor == Color.white)
            {
                move.face = FaceSwap(face, FACES.D, FACES.U, FACES.R, FACES.L, FACES.F, FACES.B);
            }
            else if (FrontColor == Color.red)
            {
                move.face = FaceSwap(face, FACES.D, FACES.U, FACES.F, FACES.B, FACES.L, FACES.R);
                print("red");
            }
            else if (FrontColor == Color.yellow)
            {
                move.face = FaceSwap(face, FACES.D, FACES.U, FACES.L, FACES.R, FACES.B, FACES.F);
                print("blue");
            }
            else
            {
                move.face = FaceSwap(face, FACES.D, FACES.U, FACES.B, FACES.F, FACES.R, FACES.L);
                print("orange");
            }
        }
        else if (topColor == Color.red)
        {
            if (FrontColor == Color.white)
            {
                move.face = FaceSwap(face, FACES.R, FACES.L, FACES.U, FACES.D, FACES.F, FACES.B);
            }
            else if (FrontColor == Color.green)
            {
                move.face = FaceSwap(face, FACES.F, FACES.B, FACES.U, FACES.D, FACES.L, FACES.R);
                print("red");
            }
            else if (FrontColor == Color.yellow)
            {
                move.face = FaceSwap(face, FACES.L, FACES.R, FACES.U, FACES.D, FACES.B, FACES.F);
                print("blue");
            }
            else
            {
                move.face = FaceSwap(face, FACES.B, FACES.F, FACES.D, FACES.U, FACES.R, FACES.L);
                print("orange");
            }
        }
        else
        {
            if (FrontColor == Color.white)
            {
                move.face = FaceSwap(face, FACES.L, FACES.R, FACES.D, FACES.U, FACES.F, FACES.B);
            }
            else if (FrontColor == Color.green)
            {
                move.face = FaceSwap(face, FACES.F, FACES.B, FACES.D, FACES.U, FACES.R, FACES.L);
                print("red");
            }
            else if (FrontColor == Color.yellow)
            {
                move.face = FaceSwap(face, FACES.R, FACES.L, FACES.D, FACES.U, FACES.B, FACES.F);
                print("blue");
            }
            else
            {
                move.face = FaceSwap(face, FACES.B, FACES.F, FACES.D, FACES.U, FACES.L, FACES.R);
                print("orange");
            }
        }

        return move;
    }

    private FACES FaceSwap(FACES face, FACES F, FACES B, FACES R, FACES L, FACES U, FACES D)
    {
        if (face == FACES.F) return F;
        if (face == FACES.B) return B;
        if (face == FACES.R) return R;
        if (face == FACES.L) return L;
        if (face == FACES.U) return U;
        if (face == FACES.D) return D;
        return FACES.NULL;
    }

    private Move DoubleMove(Move move)
    {
        Move move1 = GetFace(move);
        if (_moves.lastMove == null) _moves.lastMove = move;
        Move move2 = _moves.lastMove;
        // if (_messages.HasMessages())
        //     move2 = GetFace(_messages.Dequeue());


        print("turn  " + (move1.time));
        print("turn difference " + Math.Abs(move1.time.TotalMilliseconds - move2.time.TotalMilliseconds));
        if (Math.Abs(move1.time.TotalMilliseconds - move2.time.TotalMilliseconds) < 750)
        {
            //could be a double move
            if ((move1.face == FACES.L && move2.face == FACES.R ||
                 move2.face == FACES.L && move1.face == FACES.R) && move1.direction != move2.direction)
            {
                //take out the other face move
                if (move1.face == FACES.L)
                    move1.direction *= -1;
                
                move1.face = FACES.M;
                offsetCentersX(move1.direction);
            }

            else if ((move1.face == FACES.U && move2.face == FACES.D ||
                      move2.face == FACES.D && move1.face == FACES.U) && move1.direction != move2.direction)
            {
                //take out the other face move
                if (move1.face == FACES.U)
                    move1.direction *= -1;
                move1.face = FACES.E;
                offsetCentersY(move1.direction);
            }

            else if ((move1.face == FACES.F && move2.face == FACES.B ||
                      move2.face == FACES.B && move1.face == FACES.F) && move1.direction != move2.direction)
            {
                if (move1.face == FACES.F)
                    move1.direction *= -1;

                move1.face = FACES.S;
                offsetCentersZ(move1.direction);
            }
        }

        return move1;
    }

    public void offsetCentersY(int direction)
    {
        Color[] aux = new Color[4]
            { centers[1, 0], centers[1, 1], centers[1, 2], centers[1, 3] };
        if (direction < 0)
        {
            centers[1, 0] = aux[3];
            centers[1, 1] = aux[0];
            centers[1, 2] = aux[1];
            centers[1, 3] = aux[2];
        }
        else
        {
            centers[1, 0] = aux[1];
            centers[1, 1] = aux[2];
            centers[1, 2] = aux[3];
            centers[1, 3] = aux[0];
        }

        UpdateColors();
    }

    public void offsetCentersX(int direction)
    {
        Color[] aux = new Color[4]
            { centers[0, 0], centers[1, 0], centers[2, 0], centers[1, 2] };

        if (direction > 0)
        {
            centers[0, 0] = aux[3];
            centers[1, 0] = aux[0];
            centers[2, 0] = aux[1];
            centers[1, 2] = aux[2];
        }
        else
        {
            centers[0, 0] = aux[1];
            centers[1, 0] = aux[2];
            centers[2, 0] = aux[3];
            centers[1, 2] = aux[0];
        }

        UpdateColors();
    }

    public void offsetCentersZ(int direction)
    {
        Color[] aux = new Color[4]
            { centers[0, 0], centers[1, 1], centers[2, 0], centers[1, 3] };
        if (direction > 0)
        {
            centers[0, 0] = aux[3];
            centers[1, 1] = aux[0];
            centers[2, 0] = aux[1];
            centers[1, 3] = aux[2];
        }
        else
        {
            centers[0, 0] = aux[1];
            centers[1, 1] = aux[2];
            centers[2, 0] = aux[3];
            centers[1, 3] = aux[0];
        }

        UpdateColors();
    }

    private void UpdateColors()
    {
        topColor = centers[0, 0];
        FrontColor = centers[1, 0];
    }

    private Move GetFace(Move move)
    {
        char[] chars = move.msg.ToCharArray();

        move.direction = chars.Length > 1 ? -1 : 1;

        switch (chars[0])
        {
            case 'R':
                move.face = FACES.R;
                break;
            case 'L':
                move.face = FACES.L;
                break;
            case 'F':
                move.face = FACES.F;
                break;
            case 'B':
                move.face = FACES.B;
                break;
            case 'U':
                move.face = FACES.U;
                break;
            case 'D':
                move.face = FACES.D;
                break;
        }

        return ApplyOffset(move);
    }

    private bool ValidateMoves(string move)
    {
        char[] chars = move.ToCharArray();
        return _validationFaces.Contains(chars[0]);
    }

    public Move GetLastMove()
    {
        return _moves.lastMove;
    }
}