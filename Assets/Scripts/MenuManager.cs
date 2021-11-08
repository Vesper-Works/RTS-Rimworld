using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using TMPro;
using UnityEngine.UI;
public class MenuManager : MonoBehaviour
{
    //----------------------------------------------------------
    // UI elements
    //----------------------------------------------------------

    public GameObject createAccountMenu;
    public GameObject lobbyMenus;
    public TMP_InputField newAccountName;
    public Button createAccountButton;
    public ConnectionController connectionController;


    //----------------------------------------------------------
    // Public properties
    //---------------------------------------------------------- 

    public string playerName;

    //----------------------------------------------------------
    // Private properties
    //----------------------------------------------------------

    void Start()
    {
        playerName = GetPlayerName();
        if (playerName == "")
        {
            ShowAccountCreation();
        }
        else if(!SmartFoxConnection.IsInitialized)
        {
            connectionController.ConnectToServer();
        }
        else
        {
            lobbyMenus.SetActive(true);
        }
    }

    private void ShowAccountCreation()
    {
        createAccountMenu.SetActive(true);
    }

    private string GetPlayerName()
    {
        string path = Application.persistentDataPath + "/PlayerName.name";
        BinaryFormatter formatter = new BinaryFormatter();

        if (File.Exists(path))
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            PlayerData data = (PlayerData)formatter.Deserialize(stream);
            stream.Close();
            return data.name; // TODO: Make sure it's always valid
        }
        return "";
    }

    public void CreateAccount()
    {
        string path = Application.persistentDataPath + "/PlayerName.name";
        BinaryFormatter formatter = new BinaryFormatter();

        if (File.Exists(path))
        {
            Debug.Log("Name file already exists");
        }
        else
        {
            FileStream stream = new FileStream(path, FileMode.Create);
            PlayerData data = new PlayerData(newAccountName.text);
            formatter.Serialize(stream, data);
            stream.Close();
            playerName = newAccountName.text.TrimStart().TrimEnd();
            createAccountMenu.SetActive(false);
            connectionController.ConnectToServer();

        }
    
    }
    public void SaveName(string name)
    {
        string path = Application.persistentDataPath + "/PlayerName.name";
        BinaryFormatter formatter = new BinaryFormatter();

        if (File.Exists(path))
        {
            Debug.Log("Name file already exists");
        }
        else
        {
            FileStream stream = new FileStream(path, FileMode.Create);
            PlayerData data = new PlayerData(name);
            formatter.Serialize(stream, data);
            stream.Close();
        }
        playerName = name;
    }
    public void ValidateNewName()
    {
        char[] invalidChars = {' '};  
        if (!string.IsNullOrEmpty(newAccountName.text.Trim(invalidChars)))
        {
            createAccountButton.interactable = true;
        }
        else
        {
            createAccountButton.interactable = false;
        }
    }

    public void ConnectedToServer()
    {
        lobbyMenus.SetActive(true);
    }
}
