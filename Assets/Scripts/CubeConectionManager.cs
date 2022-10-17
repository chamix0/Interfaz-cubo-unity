using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CubeConectionManager : MonoBehaviour
{
    private ProcessMessages _processMessages;
    private RunProcess _process;

    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private Button _button;

    // Start is called before the first frame update
    void Start()
    {
        _process = GetComponent<RunProcess>();
        _processMessages = GetComponent<ProcessMessages>();
        StartCoroutine(StablishCommunication());
        _button.interactable = false;
    }


    public void ConnectButton()
    {
        _process.SendMessage("" + _dropdown.value);
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
    }
}