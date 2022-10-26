using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowCenterReference : MonoBehaviour
{
    public Image _imageTop, _imageFront, _imageBottom, _imageRight, _imageBack, _imageLeft;
    [SerializeField] private CubeInputs _cubeInputs;


    // Update is called once per frame
    void Update()
    {
        _imageTop.color = _cubeInputs.centers[0, 0];
        _imageFront.color = _cubeInputs.centers[1, 0];

    }
}