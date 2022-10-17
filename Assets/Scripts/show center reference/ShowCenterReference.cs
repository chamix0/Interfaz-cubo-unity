using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShowCenterReference : MonoBehaviour
{
    [SerializeField] private Image _imageTop;
    [SerializeField] private Image _imageFront;
    [SerializeField] private CubeInputs _cubeInputs;



    // Update is called once per frame
    void Update()
    {
        _imageTop.tintColor = _cubeInputs.topColor;
        _imageFront.tintColor = _cubeInputs.FrontColor;
    }
}