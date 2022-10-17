using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowCenterReference : MonoBehaviour
{
    public Image _imageTop, _imageFront;
    [SerializeField] private CubeInputs _cubeInputs;


    // Update is called once per frame
    void Update()
    {
        _imageTop.color = _cubeInputs.topColor;
        _imageFront.color = _cubeInputs.FrontColor;
    }
}