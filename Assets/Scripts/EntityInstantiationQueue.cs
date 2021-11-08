using Sfs2X.Entities.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using System.Diagnostics;
public class EntityInstantiationQueue : MonoBehaviour
{
    private Queue<SFSObject> entityQueue = new Queue<SFSObject>();
    private int nextIDNumber = 0;
    private Stopwatch stopwatch;
    private static EntityInstantiationQueue instance { get; set; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            stopwatch = new Stopwatch();
            stopwatch.Start();
            StartCoroutine("InstantationMachine");
            StartCoroutine("InstantationMachine");
            StartCoroutine("InstantationMachine");
            StartCoroutine("InstantationMachine");
            StartCoroutine("InstantationMachine");
            StartCoroutine("InstantationMachine");
            StartCoroutine("InstantationMachine");
            StartCoroutine("InstantationMachine");  
            StartCoroutine("InstantationMachine");
            StartCoroutine("InstantationMachine");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public static void AddEntityToQueue(SFSObject entityToAdd)
    {
        instance.entityQueue.Enqueue(entityToAdd);
    }

    private IEnumerator InstantationMachine()
    {
        while (true)
        {
            if (entityQueue.Count > 0) { InstantiateEntity(entityQueue.Dequeue()); }
            else
            {
               // UnityEngine.Debug.Log(stopwatch.Elapsed.TotalSeconds);
                //stopwatch.Stop();
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void InstantiateEntity(SFSObject resObj)
    {
        Vector2 position = new Vector2(resObj.GetInt("x"), resObj.GetInt("y"));

        string entityType = resObj.GetUtfString("entityType");
        string entityName = resObj.GetUtfString("entityName");
        Entity entityInfo = EntityDefinitions.Instance.EntitiesByTypes[entityType][entityName];
        GameObject newPlayerEntity = new GameObject(entityName);
        newPlayerEntity.transform.position = position;
        newPlayerEntity.transform.localScale = Vector3.one * entityInfo.SpriteScale;
        var entity = (Entity)newPlayerEntity.AddComponent(entityInfo.GetType());
        entityInfo.CopyProperties(entity);

        List<BaseComponent> initialisedComponents = new List<BaseComponent>();
        for (int k = 0; k < entity.Components.Count; k++)
        {
            BaseComponent component = (BaseComponent)entity.gameObject.AddComponent(entity.Components[k].GetType());
            entity.Components[k].CopyProperties(component);
            component.Startup();
            initialisedComponents.Add(component);
        }
        entity.Components = initialisedComponents;
        int itemAmount = resObj.GetInt("itemAmount");
        if (itemAmount != 0)
        {
            entity.GetComponent<ItemEntity>().Amount = itemAmount;
        }
        entity.ID = nextIDNumber;
        entity.Owner = SmartFoxConnection.Connection.UserManager.GetUserById(resObj.GetInt("owner"));
         GameController.AddEntityToDictionary(nextIDNumber, entity);
        nextIDNumber++;

        string texPath = entity.TexturePath;
        Sprite sprite;        
        if (entity.GetType() == typeof(StructureEntity) && ((StructureEntity)entity).Tileable)
        {
            ((StructureEntity)entity).ReTextureForTiles();
        }
        else
        {
            sprite = ResourceHandler.LoadSprite(texPath);

            if (newPlayerEntity.GetComponent<SpriteRenderer>() == null)
            {
                newPlayerEntity.AddComponent<SpriteRenderer>().sprite =
                    Sprite.Create(sprite.texture, new Rect(0, 0, sprite.texture.width, sprite.texture.height),
             new Vector2(entity.PivotPoints[0], entity.PivotPoints[1]), sprite.texture.width / entityInfo.SpriteScale);
            }
            else
            {
                newPlayerEntity.GetComponent<SpriteRenderer>().sprite =
                    Sprite.Create(sprite.texture, new Rect(0, 0, sprite.texture.width, sprite.texture.height),
             new Vector2(entity.PivotPoints[0], entity.PivotPoints[1]), sprite.texture.width / entityInfo.SpriteScale);
            }

        }


        float[] colours = { entity.Colour.r * 255, entity.Colour.g * 255, entity.Colour.b * 255 };
        if (colours != null) { newPlayerEntity.GetComponent<SpriteRenderer>().color = new Color(colours[0] / 255f, colours[1] / 255f, colours[2] / 255f); }

        if (!entity.GetType().IsSubclassOf(typeof(MoveableEntity)) && entity.GetType() != typeof(MoveableEntity))
        {
            GridUtility.GetGridCell(position).SetEntity(entity);

        }
        else
        {
            newPlayerEntity.AddComponent<BoxCollider2D>();
        }
        if (entity.VaryLocation)
        {
            entity.transform.position += new Vector3(((RandomNumber.Get() * 2f) - 100f) / 250f, ((RandomNumber.Get() * 2f) - 100f) / 250f, 0);
        }
    }
}
