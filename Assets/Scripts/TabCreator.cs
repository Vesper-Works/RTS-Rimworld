using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TabCreator : MonoBehaviour
{
    void Start()
    {
        //int currentTabNumber = 1;
        //int totalTabNum = EntityDefinitions.Instance.TabDefinitions.Count;
        //float normalisedTabSize = 1f / totalTabNum;
        foreach (var item in EntityDefinitions.Instance.TabDefinitions)
        {
            GameObject tabGameObject = new GameObject();
            tabGameObject.transform.SetParent(transform);
            tabGameObject.transform.localScale = Vector3.one;
            tabGameObject.transform.localPosition = new Vector3(0, 0, 0);
            Tab tab = (Tab)tabGameObject.AddComponent(item.Value.GetType());
            item.Value.CopyProperties(tab);
            tabGameObject.name = tab.Name;

            GameObject textGameObject = new GameObject(tab.Name);
            textGameObject.transform.SetParent(tabGameObject.transform);
            textGameObject.transform.localScale = Vector3.one;
            textGameObject.transform.localPosition = new Vector3(0, 0, 0);

            TMPro.TextMeshProUGUI textMesh = textGameObject.AddComponent<TMPro.TextMeshProUGUI>();
            textMesh.text = tab.Name;
            textMesh.alignment = TMPro.TextAlignmentOptions.Center;

            Texture2D texture = ResourceHandler.LoadTexture(tab.TexturePath);
            tabGameObject.AddComponent<Image>().sprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f),
                    texture.width,
                    1,
                    SpriteMeshType.Tight,
                    new Vector4(texture.width / 25, texture.height / 25, texture.width / 25, texture.height / 25)

                );
            tabGameObject.GetComponent<Image>().type = Image.Type.Sliced;
            //currentTabNumber++;
        }
    }
}
