using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Main script, it is in charge of establishing and maintaining the communication between the cube and the application. 
/// </summary>
[DefaultExecutionOrder(2)]
public class CubeConectionManager : MonoBehaviour
{
    private MovesQueue _movesQueue;
    private RunProcess _process;
    private CubeInputs _cubeInputs;
    private string currentDevice = "";
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private Button _button;

    // Start is called before the first frame update
    void Start()
    {
        _process = GetComponent<RunProcess>();
        _movesQueue = GetComponent<MovesQueue>();
        _cubeInputs = GetComponent<CubeInputs>();
        _button.onClick.AddListener(ConnectButton);
        StartCoroutine(GetDevices());
        _button.interactable = false;
    }


    public void ConnectButton()
    {
        currentDevice = _dropdown.options[_dropdown.value].text;
        _process.SendMessageProcess(currentDevice);
        _button.interactable = false;
    }

    /// <summary>
    /// reads from the messages queue the number of available devices and the devices
    /// </summary>
    /// <returns></returns>
    IEnumerator GetDevices()
    {
        yield return new WaitForSeconds(1);
        int count = int.Parse(_movesQueue.Dequeue().msg); //the first message will always be the number of available devices to connect  
        for (int i = 0; i < count; i++)
        {
            _dropdown.options.Add(new TMP_Dropdown.OptionData(_movesQueue.Dequeue().msg));
        }

        _button.interactable = true;
        _cubeInputs.gameObject.SetActive(true);
        _cubeInputs.isActive = true;
    }

    /// <summary>
    /// if communication fails it will retry establishing communication to the previously selected device.
    /// </summary>
    /// <returns></returns>
    public IEnumerator RestablishComunication()
    {
        _process.StartProcess();
        yield return new WaitForSeconds(1);
        _process.SendMessageProcess(currentDevice);
    }
}