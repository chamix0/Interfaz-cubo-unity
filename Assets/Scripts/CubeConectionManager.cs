using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(2)]
public class CubeConectionManager : MonoBehaviour
{
    private ProcessMessages _processMessages;
    private RunProcess _process;
    private CubeInputs _cubeInputs;
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private Button _button;

    // Start is called before the first frame update
    void Start()
    {
        _process = GetComponent<RunProcess>();
        _processMessages = GetComponent<ProcessMessages>();
        _cubeInputs = GetComponent<CubeInputs>();
        StartCoroutine(StablishCommunication());
        _button.interactable = false;
    }


    public void ConnectButton()
    {
        _process.SendMessageProcess("" + _dropdown.value);
        _button.interactable = false;
    }

    IEnumerator StablishCommunication()
    {
        yield return new WaitForSeconds(1);
        int count = int.Parse(_processMessages.Dequeue().msg);
        for (int i = 0; i < count; i++)
        {
            _dropdown.options.Add(new TMP_Dropdown.OptionData(_processMessages.Dequeue().msg));
        }

        _button.interactable = true;
        _cubeInputs.gameObject.SetActive(true);
        _cubeInputs.isActive = true;
    }
}