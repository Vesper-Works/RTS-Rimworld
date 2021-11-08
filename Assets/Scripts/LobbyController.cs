using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using SFS2XExamples.FirstPersonShooter;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    //----------------------------------------------------------
    // UI elements
    //----------------------------------------------------------

    public ScrollRect chatScrollView;
    public Text chatText;
    public CanvasGroup chatControls;
    public Text loggedInText;
    public Button StartGameButton;
    public GameObject LobbyLeaderPanel;

    //----------------------------------------------------------
    // Private properties
    //----------------------------------------------------------

    private SmartFox sfs;
    private bool shuttingDown;
    private bool isCreator;

    //----------------------------------------------------------
    // Unity calback methods
    //----------------------------------------------------------

    void Start()
    {
        Application.runInBackground = true;

        if (SmartFoxConnection.IsInitialized)
        {
            sfs = SmartFoxConnection.Connection;
        }
        else
        {
            SceneManager.LoadScene("Main Menu");
            return;
        }

        // Register event listeners
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.AddEventListener(SFSEvent.PUBLIC_MESSAGE, OnPublicMessage);
        sfs.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
        sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
        sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnEventResponse);

        StartGameButton.onClick.AddListener(OnStartGame);

        if (SmartFoxConnection.Connection.LastJoinedRoom.UserCount == 1)
        {
            isCreator = true;
          
            CreateRandomNumberImage();
        }
        else
        {
            isCreator = false;
            LobbyLeaderPanel.SetActive(false);
        }
        RoomData.RoomCreator = isCreator;
    }

    // Update is called once per frame
    void Update()
    {
        if (sfs != null)
            sfs.ProcessEvents();
        OnSendMessageKeyPress();
    }

    void OnApplicationQuit()
    {
        shuttingDown = true;
    }

    //----------------------------------------------------------
    // Public interface methods for UI
    //----------------------------------------------------------

    public void OnSendMessageButtonClick()
    {
        InputField msgField = (InputField)chatControls.GetComponentInChildren<InputField>();

        if (msgField.text != "")
        {
            // Send public message to Room
            sfs.Send(new Sfs2X.Requests.PublicMessageRequest(msgField.text));

            // Reset message field
            msgField.text = "";
            msgField.ActivateInputField();
            msgField.Select();
        }
    }

    public void OnSendMessageKeyPress()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            OnSendMessageButtonClick();
    }

    public void OnLogoutButtonClick()
    {
        // Disconnect from server
        sfs.Disconnect();
    }

    //----------------------------------------------------------
    // Private helper methods
    //----------------------------------------------------------
    private void CreateRandomNumberImage()
    {
        SmartFoxConnection.Connection.Send(new ExtensionRequest("GenerateRNGImage", new SFSObject(), SmartFoxConnection.Connection.LastJoinedRoom));
    }

    private void OnEventResponse(BaseEvent evt)
    {
        object data;

        evt.Params.TryGetValue("cmd", out data);
        string cmd = (string)data;

        evt.Params.TryGetValue("params", out data);
        SFSObject resObj = (SFSObject)data;

        switch (cmd)
        {
            case "StartGame":
               
                reset();
                Debug.Log("Loading scene");
                SceneManager.LoadScene("Game");
                break;
            case "StartMapGeneration":
              
                Debug.Log("Starting map generation");
                GridGenerator.Instance.GenerateGrid(resObj.GetInt("seed"));
                break;
            case "RNGImageGenerated":
               
                Debug.Log("RNG Image Generated");
                float[] imageData = resObj.GetFloatArray("ImageArray");

                Texture2D texture = new Texture2D(100, 100);

                int pixelIntGroup = 0;
                for (int y = 0; y < 100; y++)
                {
                    for (int x = 0; x < 100; x++)
                    {
                        texture.SetPixel(x, y, new Color(imageData[pixelIntGroup], imageData[pixelIntGroup + 1], imageData[pixelIntGroup + 2]));
                        pixelIntGroup += 3;
                    }
                }
                File.WriteAllBytes(Path.Combine(Application.dataPath, "Coconut.png"), texture.EncodeToPNG());

                break;
        }
    }
    private void reset()
    {
        // Remove SFS2X listeners
        sfs.RemoveAllEventListeners();
    }

    private void printSystemMessage(string message)
    {
        chatText.text += "<color=#808080ff>" + message + "</color>\n";
        Canvas.ForceUpdateCanvases();

        // Scroll view to bottom
        chatScrollView.verticalNormalizedPosition = 0;
    }

    private void printUserMessage(User user, string message)
    {
        chatText.text += "<b>" + (user == sfs.MySelf ? "You" : user.Name) + ":</b> " + message + "\n";
        Canvas.ForceUpdateCanvases();

        // Scroll view to bottom
        chatScrollView.verticalNormalizedPosition = 0;
    }

    //----------------------------------------------------------
    // SmartFoxServer event listeners
    //----------------------------------------------------------

    private void OnConnectionLost(BaseEvent evt)
    {
        // Remove SFS2X listeners
        reset();

        if (shuttingDown == true)
            return;

        // Return to login scene
        SceneManager.LoadScene("Main Menu");
    }

    private void OnPublicMessage(BaseEvent evt)
    {
        User sender = (User)evt.Params["sender"];
        string message = (string)evt.Params["message"];

        printUserMessage(sender, message);
    }

    private void OnUserEnterRoom(BaseEvent evt)
    {
        User user = (User)evt.Params["user"];

        // Show system message
        printSystemMessage("User " + user.Name + " entered the room");
    }

    private void OnUserExitRoom(BaseEvent evt)
    {
        User user = (User)evt.Params["user"];

        if (user != sfs.MySelf)
        {
            // Show system message
            printSystemMessage("User " + user.Name + " left the room");
        }
    }

    private void OnStartGame()
    {
        StartGameButton.gameObject.SetActive(false);
        SmartFoxConnection.Connection.Send(new ExtensionRequest("StartMapGeneration", new Sfs2X.Entities.Data.SFSObject(), SmartFoxConnection.Connection.LastJoinedRoom));
    }
}
