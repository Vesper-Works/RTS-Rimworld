using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Logging;
using Sfs2X.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionController : MonoBehaviour
{
    //----------------------------------------------------------
    // Public properties
    //---------------------------------------------------------- 

    public MenuManager menuManager;
    [HideInInspector] public static ConnectionController Instance { get; private set; } //Singleton structure.
   

    //----------------------------------------------------------
    // Private properties
    //----------------------------------------------------------

    private SmartFox sfs;
    public string ipAddress;
    private int port;

    //----------------------------------------------------------
    // Unity calback methods
    //----------------------------------------------------------
    private void Awake()
    {     
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {
        // As Unity is not thread safe, we process the queued up callbacks on every frame
        if (sfs != null)
            sfs.ProcessEvents();
    }

    void OnApplicationQuit()
    {
        // Always disconnect before quitting
        if (sfs != null && sfs.IsConnected)
            sfs.Disconnect();
    }

    // Disconnect from the socket when ordered by the main Panel scene
    public void Disconnect()
    {
        OnApplicationQuit();
    }

    //----------------------------------------------------------
    // Public interface methods for UI
    //----------------------------------------------------------

    public void ConnectToServer()
    {
        if (sfs == null || !sfs.IsConnected)
        {

            // CONNECT

            // Enable interface
            enableInterface(false);

            // Initialize SFS2X client and add listeners
            // WebGL build uses a different constructor
#if !UNITY_WEBGL
            sfs = new SmartFox();
#else
				sfs = new SmartFox(UseWebSocket.WS_BIN);
#endif
            SmartFoxConnection.Connection = sfs;
            sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
            sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
            sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
            sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);

            sfs.AddLogListener(LogLevel.INFO, OnInfoMessage);
            sfs.AddLogListener(LogLevel.WARN, OnWarnMessage);
            sfs.AddLogListener(LogLevel.ERROR, OnErrorMessage);


            // Set connection parameters
            //ipAddress = SFS2XExamples.Panel.Settings.ipAddress;
            //ipAddress = "5.69.8.116";
            port = SFS2XExamples.Panel.Settings.port;

            ConfigData cfg = new ConfigData();
            cfg.Host = ipAddress;
            cfg.Port = 9933;
            cfg.Zone = "RTS Rimworld";
            cfg.Debug = false;

            // Connect to SFS2X
            sfs.Connect(cfg);
        }
        else
        {

            // DISCONNECT

            // Disable button
            //button.interactable = false;

            // Disconnect from SFS2X
            sfs.Disconnect();
        }
    }

    //----------------------------------------------------------
    // Private helper methods
    //----------------------------------------------------------

    private void enableInterface(bool enable)
    {
        //hostInput.interactable = enable;
        //portInput.interactable = enable;
        //debugToggle.interactable = enable;

        //button.interactable = enable;
        //buttonLabel.text = "CONNECT";
    }

    private void reset()
    {
        // Remove SFS2X listeners
        sfs.RemoveEventListener(SFSEvent.CONNECTION, OnConnection);
        sfs.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);

        sfs.RemoveLogListener(LogLevel.INFO, OnInfoMessage);
        sfs.RemoveLogListener(LogLevel.WARN, OnWarnMessage);
        sfs.RemoveLogListener(LogLevel.ERROR, OnErrorMessage);

        sfs = null;

        // Enable interface
        enableInterface(true);
    }

    //----------------------------------------------------------
    // SmartFoxServer event listeners
    //----------------------------------------------------------

    private void OnConnection(BaseEvent evt)
    {
        if ((bool)evt.Params["success"])
        {
            // Login
            sfs.Send(new Sfs2X.Requests.LoginRequest(menuManager.playerName));
        }
        else
        {
            Debug.LogError("Connection failed: " + (string)evt.Params["errorMessage"]);
            reset();
        }
    }

    private void OnConnectionLost(BaseEvent evt)
    {
        // Remove SFS2X listeners and re-enable interface
        reset();
    }
    private void OnLogin(BaseEvent evt)
    {
        // Remove SFS2X listeners and re-enable interface
        reset();
        menuManager.ConnectedToServer();
    }

    private void OnLoginError(BaseEvent evt)
    {
        // Disconnect
        sfs.Disconnect();

        // Remove SFS2X listeners and re-enable interface
        // reset();

        // Show error message
        Debug.LogError("Login failed: " + (string)evt.Params["errorMessage"]);
    }

    //----------------------------------------------------------
    // SmartFoxServer log event listeners
    //----------------------------------------------------------

    public void OnInfoMessage(BaseEvent evt)
    {
        string message = (string)evt.Params["message"];
        ShowLogMessage("INFO", message);
    }

    public void OnWarnMessage(BaseEvent evt)
    {
        string message = (string)evt.Params["message"];
        ShowLogMessage("WARN", message);
    }

    public void OnErrorMessage(BaseEvent evt)
    {
        string message = (string)evt.Params["message"];
        ShowLogMessage("ERROR", message);
    }

    private void ShowLogMessage(string level, string message)
    {
        message = "[SFS > " + level + "] " + message;
        Debug.Log(message);
    }
}

