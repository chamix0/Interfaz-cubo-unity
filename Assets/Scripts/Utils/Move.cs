using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public string msg;
    public float time;
    public int direction;
    public FACES face;

    public Move(string value)
    {
        msg = value;
        face = FACES.NULL;
        direction = 0;
        time = Time.unscaledTime;
    }
}