using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    [SerializeField] private string rpmLink;

    [SerializeField] private string username;
    [SerializeField] private string password;
    [SerializeField] private bool remember;

    public SaveData()
    {
        rpmLink = "https://api.readyplayer.me/v1/avatars/6328241e3a656b9c327edc2d.glb";
        username = "";
        password = "";
        remember = false;
    }

    #region GETTERS AND SETTERS

    public bool remember1
    {
        get => remember;
        set => remember = value;
    }
    public string rpmLink1
    {
        get => rpmLink;
        set => rpmLink = value;
    }

    public string username1
    {
        get => username;
        set => username = value;
    }

    public string password1
    {
        get => password;
        set => password = value;
    }

    #endregion
}