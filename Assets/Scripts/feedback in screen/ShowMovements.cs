using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowMovements : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text text;
    [SerializeField] CubeInputs _cubeInputs;
    [SerializeField] private MovesQueue _movesQueue;

    // Update is called once per frame
    void Update()
    {
        if (_movesQueue.HasMessages())
        {
            Move move = _movesQueue.Dequeue();
            text.text = "" + move.face + move.direction;
        }
    }
}