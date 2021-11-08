using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class Tab : MonoBehaviour
{
    private new string name;
    private string description;
    public GameObject selectedGraphic;
    private string texPath;
    private bool selected;
    private GameObject tabElements;
    private List<GameObject> structureButtons = new List<GameObject>();
    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public string TexturePath
    {
        get
        {
            if (texPath == null) { return "MissingTexture"; }
            return texPath;
        }
        set => texPath = value;
    }
    public bool Selected { get => selected; set => selected = value; }

    private void Start()
    {
        Button button = gameObject.AddComponent<Button>();
        button.onClick.AddListener(OnClick);
        tabElements = new GameObject(Name + " tab elements");
        tabElements.SetActive(false);
        tabElements.AddComponent<Image>().color = new Color(0, 0, 0, 0.8f);
        RectTransform rectTransform = (RectTransform)tabElements.transform;
        rectTransform.SetParent(transform);

        rectTransform.SetParent(transform.parent.parent);
        rectTransform.anchorMax = new Vector2(1, 0.25f);
        rectTransform.anchorMin = new Vector2(0, 0.05f);
        rectTransform.sizeDelta = Vector3.zero;
        rectTransform.localPosition = Vector3.zero;
        rectTransform.anchoredPosition = Vector3.zero;
        rectTransform.localScale = Vector3.one;

        int count = 0;
        foreach (StructureEntity structure in EntityDefinitions.Instance.EntitiesByTypes["StructureEntity"].Values)
        {
            if (structure.BuildTab == Name)
            {
                GameObject buildableIcon = new GameObject(structure.Name);
                Texture2D texture = ResourceHandler.LoadTexture(structure.TexturePath);
                buildableIcon.AddComponent<Image>().sprite = Sprite.Create(texture,
                  new Rect(0, 0, texture.width, texture.height), // section of texture to use
                  new Vector2(0.5f, 0.5f), // pivot in centre
                  texture.width, // pixels per unity tile grid unit
                  1,
                  SpriteMeshType.Tight,
                  Vector4.zero
              );
                //buildableIcon.GetComponent<SpriteRenderer>().sortingLayerName = "UI";

                RectTransform buildableIconRectTransform = buildableIcon.GetComponent<RectTransform>();
                buildableIconRectTransform.SetParent(rectTransform);
                buildableIconRectTransform.localScale = new Vector3(1.5f, 1.5f, 1);
                buildableIconRectTransform.localPosition = Vector3.zero;
                buildableIconRectTransform.localPosition = new Vector2(-800 + (count * 200), 0);

                buildableIcon.AddComponent<Button>().onClick.AddListener(delegate { BuildableOnClick(structure.Name); });
                count++;
                structureButtons.Add(buildableIcon);
            }
        }

    }
    private void BuildableOnClick(string name)
    {
        GhostHandler.SetSelectedStructure(name);
        foreach (var item in structureButtons)
        {
            item.transform.localScale = Vector3.one * 1.5f;
        }
        structureButtons.Find(button => button.name == name).transform.localScale = Vector3.one * 1.8f;
    }
    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (!GhostHandler.Placing && !UIUtility.MouseCoveringStructureSelect)
            {
                tabElements.SetActive(false);
                foreach (var item in structureButtons)
                {
                    item.transform.localScale = Vector3.one * 1.5f;
                }
            }
            if (UIUtility.MouseCoveringStructureSelect && tabElements.activeSelf)
            {
                GhostHandler.ResetSelectedStructure();
                foreach (var item in structureButtons)
                {
                    item.transform.localScale = Vector3.one * 1.5f;
                }
            }
            if (UIUtility.MouseCoveringTabButtons)
            {
                tabElements.SetActive(false);
                GhostHandler.ResetSelectedStructure();
                foreach (var item in structureButtons)
                {
                    item.transform.localScale = Vector3.one * 1.5f;
                }
            }

        }
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonUp(1)) && tabElements.activeSelf)
        {
            tabElements.SetActive(false);
            GhostHandler.ResetSelectedStructure();
            foreach (var item in structureButtons)
            {
                item.transform.localScale = Vector3.one * 1.5f;
            }
        }

    }
    private void OnClick()
    {
        tabElements.SetActive(true);
    }

}
