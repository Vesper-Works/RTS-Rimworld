using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using SFS2XExamples.Tris;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Tyd;
using System.Linq; //Remove soon!
public class GameController : MonoBehaviour
{

    //----------------------------------------------------------
    // UI elements and public properties
    //----------------------------------------------------------

    public Animator chatPanelAnim;
    public ScrollRect chatScrollView;
    public Text chatText;
    public CanvasGroup chatControls;
    public Text stateText;
    public Button restartButton;
    public Sprite testSprite;

    //----------------------------------------------------------
    // Private properties
    //----------------------------------------------------------

    private enum GameState
    {
        WAITING_FOR_PLAYERS = 0,
        RUNNING,
        GAME_WON,
        GAME_LOST,
        GAME_TIE,
        GAME_DISRUPTED
    };

    private SmartFox sfs;
    private bool shuttingDown;
    private int nextIDNumber = 0;
    private int playersReady;
    private Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
    private List<ItemEntity> availableItemEntities = new List<ItemEntity>();
    //private TrisGame trisGame;

    private bool AllPlayersGridReady { get => playersReady >= SmartFoxConnection.Connection.LastJoinedRoom.PlayerList.Count; }

    //----------------------------------------------------------
    // Unity calback methods
    //----------------------------------------------------------
    private static GameController Instance { get; set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

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

        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.AddEventListener(SFSEvent.PUBLIC_MESSAGE, OnPublicMessage);
        sfs.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
        sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
        sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnEventResponse);

        setCurrentGameState(GameState.WAITING_FOR_PLAYERS);

        // Create game logic controller instance
        //trisGame = new TrisGame();
        //trisGame.InitGame(sfs);
    }
    System.Random rng;
    private void Start()
    {
        rng = new System.Random();
        //CreateEntity("Pawn", 0.1f, 5, GridUtility.GetMouseGridPosition());
    }

    void Update()
    {
        if (sfs != null)
            sfs.ProcessEvents();
        OnSendMessageKeyPress();

        for (int i = 0; i < EntityDefinitions.Instance.EntitiesByTypes["PlayerEntity"].Count; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                PlayerEntity playerEntity = (PlayerEntity)EntityDefinitions.Instance.EntitiesByTypes["PlayerEntity"].ElementAt(i).Value;
                CreateEntity(playerEntity.GetType().Name, playerEntity.Name, GridUtility.GetMouseGridPosition(), false);
            }
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            var item = gameObject.AddComponent<ItemEntity>();
            EntityDefinitions.Instance.EntitiesByTypes["ItemEntity"]["Colt 1911"].CopyProperties(item);

            List<ItemEntity> itemList = new List<ItemEntity>();
            itemList.Add(item);
            GridUtility.DropItemsAroundPosition(itemList, GridUtility.GetMouseGridPositionVector());
            Destroy(item);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            var item = gameObject.AddComponent<ItemEntity>();
            EntityDefinitions.Instance.EntitiesByTypes["ItemEntity"]["AK-47"].CopyProperties(item);

            List<ItemEntity> itemList = new List<ItemEntity>();
            itemList.Add(item);
            GridUtility.DropItemsAroundPosition(itemList, GridUtility.GetMouseGridPositionVector());
            Destroy(item);
        }
        if (Input.GetMouseButtonDown(2))
        {
            Debug.Log(GridUtility.GetCellAtMousePosition().IsEmpty);
            Debug.Log(GridUtility.GetCellAtMousePosition().Impassable);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            var resourceType = EntityDefinitions.Instance.ResourceDefinitions["Wood"];
            var resource = new Resource();
            resourceType.CopyProperties(resource);
            resource.Amount = 10;
            ResourceTracker.AddResource(resource);
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L))
        {
            EntityDefinitions.Instance.LoadAllEntities();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.timeScale == 0) { Time.timeScale = 1; }
            else { Time.timeScale = 0; }
        }
    }

    void OnApplicationQuit()
    {
        shuttingDown = true;
    }

    //----------------------------------------------------------
    // Public interface methods for UI
    //----------------------------------------------------------

    public void OnChatTabClick()
    {
        chatPanelAnim.SetBool("panelOpen", !chatPanelAnim.GetBool("panelOpen"));
    }

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

    public void OnRestartButtonClick()
    {
        //trisGame.RestartGame();
    }

    public void OnLeaveGameButtonClick()
    {
        // Remove SFS2X listeners
        reset();

        // Leave current room
        sfs.Send(new LeaveRoomRequest());

        // Return to lobby scene
        SceneManager.LoadScene("Main Menu");
    }

    //----------------------------------------------------------
    // Public methods
    //----------------------------------------------------------

    public static void SerialiseMapDataOnServer(string[] mapData, int width, int height)
    {
        var data = new SFSObject();
        data.PutInt("width", width);
        data.PutInt("height", height);
        data.PutUtfStringArray("mapData", mapData);
        SmartFoxConnection.Connection.Send(new ExtensionRequest("SerialiseMapData", data, SmartFoxConnection.Connection.LastJoinedRoom));
    }


    public static void GridReady()
    {
        SmartFoxConnection.Connection.Send(new ExtensionRequest("PlayerReady", new SFSObject(), SmartFoxConnection.Connection.LastJoinedRoom));
    }

    public static Entity GetEntityFromID(int ID)
    {
        return Instance.entities[ID];
    }
    private void OnEventResponse(BaseEvent evt)
    {
        object data;

        evt.Params.TryGetValue("cmd", out data);
        string cmd = (string)data;

        evt.Params.TryGetValue("params", out data);
        SFSObject resObj = (SFSObject)data;

        Vector2 position = Vector2.zero;
        Vector2 positionFloat = Vector2.zero;
        try
        {
            positionFloat = new Vector2(resObj.GetFloat("x"), resObj.GetFloat("y"));
        }
        catch (Exception)
        {
        }
        try
        {
            position = new Vector2(resObj.GetInt("x"), resObj.GetInt("y"));
        }
        catch (Exception)
        {
        }


        switch (cmd)
        {
            case "EntityPositionUpdate":
                int ID = resObj.GetInt("ID");
                entities[ID].transform.position = position;
                break;
            case "UpdateTile":

                int2 location = new int2((int)position.x, (int)position.y);

                GridUtility.UpdateCell(location, resObj.GetUtfString("texturePath"), resObj.GetBool("passable"), resObj.GetInt("itemAmount"));
                break;
            case "MoveToNode":
                MoveableEntity moveableEntity = (MoveableEntity)entities[resObj.GetInt("ID")];
                if (position == Vector2.zero)
                {
                    moveableEntity.MoveToLocation(positionFloat);
                }
                else
                {
                    moveableEntity.MoveToLocation(position);
                }

                break;
            case "CreateProjectile":
                GameObject projectile = new GameObject("Projectile");
                projectile.layer = 8;
                projectile.transform.position = positionFloat;
                projectile.AddComponent<Rigidbody2D>().gravityScale = 0;
                ProjectileEntity projectileEntity = projectile.AddComponent<ProjectileEntity>();
                EntityDefinitions.Instance.EntitiesByTypes["ProjectileEntity"][resObj.GetUtfString("projectileType")].CopyProperties(projectileEntity);
                projectile.AddComponent<SpriteRenderer>().sprite = ResourceHandler.LoadSprite(projectileEntity.TexturePath);
                Vector2 velocity = new Vector2(resObj.GetFloat("velocityX"), resObj.GetFloat("velocityY"));
                projectile.AddComponent<PolygonCollider2D>().isTrigger = true;
                projectileEntity.SetInMotion(velocity, velocity.magnitude, resObj.GetInt("shooterID"));

                break;

            case "CreateHitscanRay":
                ProjectileEntity projectileInformation = (ProjectileEntity)EntityDefinitions.Instance.EntitiesByTypes["ProjectileEntity"][resObj.GetUtfString("projectileType")];

                GameObject rayGraphicGameObject = new GameObject("Ray Graphic");
                rayGraphicGameObject.layer = 8;
                rayGraphicGameObject.transform.position = positionFloat;


                velocity = new Vector2(resObj.GetFloat("velocityX"), resObj.GetFloat("velocityY"));

                //ContactFilter2D contactFilter2D = new ContactFilter2D();
                //LayerMask layerMask = new LayerMask();
                //layerMask.value = 144; //10010000
                //contactFilter2D.layerMask = layerMask;
                Vector2 hitPoint = Vector2.zero;

                RaycastHit2D[] hits = new RaycastHit2D[10]; Physics2D.Raycast(positionFloat, velocity, new ContactFilter2D(), hits);
                bool hitMoveableEntity = false;
                foreach (var hit in hits)
                {
                    if (hit.collider == null) { continue; }
                    if (!hit.collider.gameObject.name.Equals("MouseCollider")
                        && !hit.collider == GetEntityFromID(resObj.GetInt("shooterID")).GetComponent<BoxCollider2D>()
                        && hit.collider.gameObject.layer != 8
                        && hit.collider.gameObject.layer != 5)
                    {
                        hit.collider.GetComponent<Entity>().TakeDamage(
                            projectileInformation.Damage, projectileInformation.Penetration, velocity.sqrMagnitude, projectileInformation.Mass);
                        hitPoint = hit.point;
                        hitMoveableEntity = true;
                    }
                }
                if (!hitMoveableEntity)
                {
                    Vector2 currentPosition = positionFloat;
                    Vector2 direction = velocity.normalized;
                    Vector2 finalPosition = positionFloat + velocity;
                    int safetyCount = 0;
                    while (Mathf.RoundToInt(currentPosition.x) != Mathf.RoundToInt(finalPosition.x)
                        || Mathf.RoundToInt(currentPosition.y) != Mathf.RoundToInt(finalPosition.y))
                    {
                        currentPosition += direction/4f;
                        Cell cellAtLocation = GridUtility.GetGridCell(currentPosition);
                        if(cellAtLocation == null)
                        {
                            hitPoint = currentPosition;
                            break;
                        }
                        if (!cellAtLocation.IsEmpty && cellAtLocation.EntityContained.BlockProjectiles)
                        {
                            cellAtLocation.EntityContained.TakeDamage(
                            projectileInformation.Damage, projectileInformation.Penetration, velocity.sqrMagnitude, projectileInformation.Mass);
                            hitPoint = currentPosition;
                            break;
                        }
                        safetyCount++;
                        if (safetyCount >= 1000)
                        {
                            Debug.LogWarning("Found nothing for 1000 loops");
                            break;
                        }
                    }


                    rayGraphicGameObject.AddComponent<RayGraphic>().CreateGraphic(projectileInformation.TexturePath, positionFloat, hitPoint);

                }

                break;
            case "CreateEntity":
                EntityInstantiationQueue.AddEntityToQueue(resObj);
                break;

            case "PlayerReady":
                playersReady++;
                if (AllPlayersGridReady) { Grid.Instance.SetAllGridTextures(); }
                break;

            case "PlaySoundOneShot":
               
                if(resObj.GetInt("callerID") != SmartFoxConnection.Connection.MySelf.Id)
                {
                    AudioSource.PlayClipAtPoint(ResourceHandler.LoadAudio(resObj.GetUtfString("fireSoundEffectPath")), positionFloat);
                }
                break;
        }
    }

    public static void CreateEntity(string entityType, string entityName, int2 position, bool ownerless)
    {
        if (GridUtility.CellIsImpassable(position)) { Debug.LogError("Tried to spawn entity somewhere impassable!"); return; }
        var data = new SFSObject();
        data.PutUtfString("entityType", entityType);
        data.PutUtfString("entityName", entityName);
        data.PutInt("x", position.x);
        data.PutInt("y", position.y);
        if (!ownerless) { data.PutInt("owner", SmartFoxConnection.Connection.MySelf.Id); }
        SmartFoxConnection.Connection.Send(new ExtensionRequest("CreateEntity", data, SmartFoxConnection.Connection.LastJoinedRoom));
    }
    public static void CreateEntity(string entityType, string entityName, Vector2 position, bool ownerless)
    {
        if (GridUtility.CellIsImpassable(position)) { Debug.LogError("Tried to spawn entity somewhere impassable!"); return; }
        var data = new SFSObject();
        data.PutUtfString("entityType", entityType);
        data.PutUtfString("entityName", entityName);
        data.PutInt("x", Mathf.RoundToInt(position.x));
        data.PutInt("y", Mathf.RoundToInt(position.y));
        if (!ownerless) { data.PutInt("owner", SmartFoxConnection.Connection.MySelf.Id); }
        SmartFoxConnection.Connection.Send(new ExtensionRequest("CreateEntity", data, SmartFoxConnection.Connection.LastJoinedRoom));
    }
    public static void CreateEntity(string entityType, string itemName, int itemAmount, int2 position, bool ownerless)
    {
        var data = new SFSObject();
        data.PutUtfString("entityType", entityType);
        data.PutUtfString("entityName", itemName);
        data.PutInt("itemAmount", itemAmount);
        data.PutInt("x", position.x);
        data.PutInt("y", position.y);
        if (!ownerless) { data.PutInt("owner", SmartFoxConnection.Connection.MySelf.Id); }
        SmartFoxConnection.Connection.Send(new ExtensionRequest("CreateEntity", data, SmartFoxConnection.Connection.LastJoinedRoom));
    }
    public static void CreateProjectile(Vector2 position, Vector2 velocity, string projectileType, int shooterID)
    {
        var data = new SFSObject();
        data.PutFloat("x", position.x);
        data.PutFloat("y", position.y);
        data.PutFloat("velocityX", velocity.x);
        data.PutFloat("velocityY", velocity.y);
        data.PutUtfString("projectileType", projectileType);
        data.PutInt("shooterID", shooterID);
        SmartFoxConnection.Connection.Send(new ExtensionRequest("CreateProjectile", data, SmartFoxConnection.Connection.LastJoinedRoom));
    }
    public static void CreateHitscanRay(Vector2 position, Vector2 velocity, string projectileType, int shooterID)
    {
        var data = new SFSObject();
        data.PutFloat("x", position.x);
        data.PutFloat("y", position.y);
        data.PutFloat("velocityX", velocity.x);
        data.PutFloat("velocityY", velocity.y);
        data.PutUtfString("projectileType", projectileType);
        data.PutInt("shooterID", shooterID);
        SmartFoxConnection.Connection.Send(new ExtensionRequest("CreateHitscanRay", data, SmartFoxConnection.Connection.LastJoinedRoom));
    }
    public static void PlaySoundOneShot(string fireSoundEffectPath, Vector2 position, int id)
    {
        var data = new SFSObject();
        data.PutUtfString("fireSoundEffectPath", fireSoundEffectPath);
        data.PutInt("callerID", id);
        data.PutInt("x", (int)position.x);
        data.PutInt("y", (int)position.y);
        SmartFoxConnection.Connection.Send(new ExtensionRequest("PlaySoundOneShot", data, SmartFoxConnection.Connection.LastJoinedRoom));
    }


    //public static void CreateProjectileStream(Vector2 position, Vector2 velocity, string projectileType, int shooterID, int projectileCount, int rateOfFire)
    //{
    //    var data = new SFSObject();
    //    data.PutFloat("x", position.x);
    //    data.PutFloat("y", position.y);
    //    data.PutFloat("velocityX", velocity.x);
    //    data.PutFloat("velocityY", velocity.y);
    //    data.PutUtfString("projectileType", projectileType);
    //    data.PutInt("shooterID", shooterID);
    //    SmartFoxConnection.Connection.Send(new ExtensionRequest("CreateProjectileStream", data, SmartFoxConnection.Connection.LastJoinedRoom));
    //}

    public static void RemoveItemFromAvailableItemsList(ItemEntity item)
    {
        Instance.availableItemEntities.Remove(item);
    }

    public static void AddEntityToDictionary(int ID, Entity entity)
    {
        Instance.entities.Add(ID, entity);
    }
    //----------------------------------------------------------
    // Private methods
    //----------------------------------------------------------

    private void setCurrentGameState(GameState state)
    {
        if (state == GameState.WAITING_FOR_PLAYERS)
        {
            stateText.text = "Waiting for your opponent";
        }
        else if (state == GameState.RUNNING)
        {
            // Nothing to do; the state text is updated by the TrisGame instance
        }
        else if (state == GameState.GAME_DISRUPTED)
        {
            stateText.text = "Opponent disconnected; waiting for new player";
        }
        else
        {
            stateText.text = "GAME OVER";

            if (state == GameState.GAME_LOST)
            {
                stateText.text += "\nYou've lost!";
            }
            else if (state == GameState.GAME_WON)
            {
                stateText.text += "\nYou won!";
            }
            else if (state == GameState.GAME_TIE)
            {
                stateText.text += "\nIt's a tie!";
            }
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
}