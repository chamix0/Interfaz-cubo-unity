using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class ProcessMessages : MonoBehaviour
{
    private Queue<Move> messages;

    // Start is called before the first frame update
    private void Awake()
    {
        messages = new Queue<Move>();
    }

    public bool HasMessages()
    {
        return messages.Count > 0;
    }

    public void Enqueue(string msg)
    {
        Move aux = new Move(msg);
        messages.Enqueue(aux);
    }

    public Move Dequeue()
    {
        return messages.Dequeue();
    }

    public Move Peek()
    {
        return messages.Peek();
    }

    // Update is called once per frame
    void Update()
    {
    }
}