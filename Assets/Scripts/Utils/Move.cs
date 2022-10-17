using System;
using System.Collections;
using System.Collections.Generic;

public class Move
{
    public string msg;
    public TimeSpan time;
    public int direction;
    public FACES face;

    public Move(string value)
    {
        msg = value;
        face = FACES.NULL;
        direction = 0;
        time = DateTime.Now.TimeOfDay;
    }
}