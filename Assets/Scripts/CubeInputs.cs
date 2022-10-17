using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Configuration;
using UnityEngine;
using UnityEngine.UIElements;

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

public class CubeInputs : MonoBehaviour

{
    private ProcessMessages _messages;
    private Queue<Move> _moves;
    private List<char> _validationFaces;
    public Color topColor = Color.clear, FrontColor = Color.clear;

    Color[,] centers = new Color[3, 4]
    {
        { Color.white, Color.clear, Color.clear, Color.clear },
        {
            Color.green, Color.red, Color.blue,
            new Color(1f, 0.75f, 0f)
        },
        { Color.yellow, Color.clear, Color.clear, Color.clear }
    };

    // Start is called before the first frame update
    private void Awake()
    {
        _moves = new Queue<Move>();
        _validationFaces = new List<char>(new[] { 'R', 'L', 'U', 'D', 'F', 'B' });
    }

    void Start()
    {
        _messages = GetComponent<ProcessMessages>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_messages.HasMessages())
        {
            Move move = _messages.Dequeue();
            if (ValidateMoves(move.msg))
            {
                //double moves

                move = DoubleMove(move);

                _moves.Enqueue(move);
            }
        }
    }

    private Move DoubleMove(Move move)
    {
        Move move1 = move;
        Move move2 = null;
        if (_messages.HasMessages())
        {
            move2 = _messages.Peek();
            if (ValidateMoves(move2.msg))
                move2 = GetFace(move2);
            if (move2.time - move1.time < 0.01f)
            {
                //could be a double move
                if (move1.face == FACES.L && move2.face == FACES.R ||
                    move2.face == FACES.L && move1.face == FACES.R && move1.direction != move2.direction)
                {
                    //take out the other face move
                    _messages.Dequeue();
                    move1.direction *= -1;
                    move1.face = FACES.M;
                    offsetCentersX(move1.direction);
                }

                if (move1.face == FACES.U && move2.face == FACES.D ||
                    move2.face == FACES.D && move1.face == FACES.U && move1.direction != move2.direction)
                {
                    //take out the other face move
                    _messages.Dequeue();
                    move1.direction *= -1;
                    move1.face = FACES.E;
                    offsetCentersY(move1.direction);
                }

                if (move1.face == FACES.F && move2.face == FACES.B ||
                    move2.face == FACES.B && move1.face == FACES.F && move1.direction != move2.direction)
                {
                    //take out the other face move
                    _messages.Dequeue();
                    move1.direction *= -1;
                    move1.face = FACES.S;
                    offsetCentersZ(move1.direction);
                }
            }
        }

        return move1;
    }

    public void offsetCentersY(int direction)
    {
        Color[] aux = new Color [4] { centers[1, 0], centers[1, 1], centers[1, 2], centers[1, 3] };

        if (direction > 0)
        {
            centers[1, 0] = aux[4];
            for (int i = 1; i < 4; i++)
                centers[1, i] = aux[i - 1];
        }
        else
        {
            for (int i = 0; i < 4; i++)
                centers[1, i] = aux[(i + 1) % 4];
        }

        UpdateColors();
    }

    public void offsetCentersX(int direction)
    {
        Color[] aux = new Color [3] { centers[0, 0], centers[1, 0], centers[2, 0] };

        if (direction > 0)
        {
            centers[0, 0] = aux[3];
            for (int i = 1; i < 3; i++)
                centers[i, 0] = aux[i - 1];
        }
        else
        {
            for (int i = 0; i < 3; i++)
                centers[i, 0] = aux[(i + 1) % 3];
        }

        UpdateColors();
    }

    public void offsetCentersZ(int direction)
    {
        Color[,] aux = centers;

        if (direction > 0)
        {
            centers[0, 0] = aux[1, 3];
            centers[1, 1] = aux[0, 0];
            centers[0, 2] = aux[1, 1];
            centers[1, 3] = aux[0, 2];
        }
        else
        {
            centers[0, 0] = aux[1, 1];
            centers[1, 1] = aux[0, 2];
            centers[0, 2] = aux[1, 3];
            centers[1, 3] = aux[0, 0];
        }

        UpdateColors();
    }

    private void UpdateColors()
    {
        topColor = centers[0, 0];
        FrontColor = centers[0, 1];
    }

    private Move GetFace(Move move)
    {
        char[] chars = move.msg.ToCharArray();

        move.direction = chars.Length > 1 ? -1 : 1;

        switch (move.msg)
        {
            case "R":
                move.face = FACES.R;
                break;
            case "L":
                move.face = FACES.L;
                break;
            case "F":
                move.face = FACES.F;
                break;
            case "B":
                move.face = FACES.B;
                break;
            case "U":
                move.face = FACES.U;
                break;
            case "D":
                move.face = FACES.D;
                break;
        }

        return move;
    }

    private bool ValidateMoves(string move)
    {
        char[] chars = move.ToCharArray();
        return _validationFaces.Contains(chars[0]);
    }
}