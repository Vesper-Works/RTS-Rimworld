using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;
public class ActionButtonHandler : MonoBehaviour
{
    private List<BaseAction> currentActions = new List<BaseAction>();
    private List<GameObject> currentActionButtons = new List<GameObject>();
    private List<UnityAction> unityActions = new List<UnityAction>();
    private static ActionButtonHandler instance { get; set; }
    private void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); }
    }

    private void Update()
    {
        for (int i = 0; i < currentActions.Count; i++)
        {
            if (Input.GetKeyDown(currentActions[i].Hotkey.ToString().ToLower()))
            {
                currentActionButtons[i].GetComponent<Button>().onClick.Invoke();    
            }
            if (Input.GetKey(currentActions[i].Hotkey.ToString().ToLower()))
            {
                currentActionButtons[i].GetComponent<Image>().color = currentActionButtons[i].GetComponent<Button>().colors.pressedColor;
            }
            else
            {
                currentActionButtons[i].GetComponent<Image>().color = currentActionButtons[i].GetComponent<Button>().colors.normalColor;
            }
        }
    }

    public static void AddAction(BaseAction action, Entity entity)
    {
        void unityAction() { action.Execute(entity); }

        instance.unityActions.Add(unityAction);
        if (instance.currentActions.Exists((value) => value.Text == action.Text))
        {
            Button button = instance.currentActionButtons[instance.currentActions.FindLastIndex((value) => value.Text == action.Text)].GetComponent<Button>();
            button.onClick.AddListener(unityAction);
        }
        else
        {
            GameObject newButton = new GameObject();
            newButton.transform.parent = instance.transform;
            Texture2D texture = ResourceHandler.LoadTexture(action.TexturePath);
            newButton.AddComponent<Image>().sprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f),
                    texture.width,
                    1,
                    SpriteMeshType.Tight,
                    Vector4.zero
                );

            newButton.AddComponent<Button>().onClick.AddListener(unityAction);

            newButton.AddComponent<HoverableUIElement>();
            instance.currentActions.Add(action);
            instance.currentActionButtons.Add(newButton);
            RectTransform rectTransform = (RectTransform)newButton.transform;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.localScale = Vector3.one;
            rectTransform.localPosition = Vector3.zero;

            int buttonCount = Mathf.FloorToInt(instance.currentActionButtons.Count / 2f);
            if (buttonCount % 2 == 0)
            {
                rectTransform.localPosition = new Vector2((rectTransform.rect.width + 5) * buttonCount, (-Camera.main.pixelHeight/2f) + (Camera.main.pixelHeight / 10f));
            }
            else
            {
                rectTransform.localPosition = new Vector2((rectTransform.rect.width + 5) * -buttonCount, 0);
            }
        } // Add text as child!
    }

    public static void RemoveAction(BaseAction action, Entity entity)
    {
        int index = instance.currentActions.FindIndex(x => x.GetType().ToString() == action.GetType().ToString());
        
        Button button = instance.currentActionButtons[index].GetComponent<Button>();

        UnityAction unityAction = instance.unityActions[index];
        button.onClick.RemoveListener(unityAction);
        instance.unityActions.RemoveAt(index);
        if (instance.unityActions.Count == 0)
        {
            instance.currentActionButtons.Remove(button.gameObject);
            instance.currentActions.RemoveAt(index);
            Destroy(button.gameObject);
        }
    }
}
