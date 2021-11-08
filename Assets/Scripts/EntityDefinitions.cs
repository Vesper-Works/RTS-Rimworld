using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Tyd;
using UnityEngine;

public class EntityDefinitions : MonoBehaviour
{
    private Dictionary<string, Dictionary<string, Entity>> entitiesByTypes = new Dictionary<string, Dictionary<string, Entity>>();
    private Dictionary<string, Cell> cellDefinitions = new Dictionary<string, Cell>();
    private Dictionary<string, Tab> tabDefinitions = new Dictionary<string, Tab>();
    private Dictionary<string, Body> bodyDefinitions = new Dictionary<string, Body>();
    private Dictionary<string, ResourceType> resourceDefinitions = new Dictionary<string, ResourceType>();
    [HideInInspector] public static EntityDefinitions Instance { get; private set; }
    public Dictionary<string, Cell> CellDefinitions { get => cellDefinitions; set => cellDefinitions = value; }
    public Dictionary<string, Tab> TabDefinitions { get => tabDefinitions; set => tabDefinitions = value; }
    public Dictionary<string, Dictionary<string, Entity>> EntitiesByTypes { get => entitiesByTypes; set => entitiesByTypes = value; }
    public Dictionary<string, Body> BodyDefinitions { get => bodyDefinitions; set => bodyDefinitions = value; }
    public Dictionary<string, ResourceType> ResourceDefinitions { get => resourceDefinitions; set => resourceDefinitions = value; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadAllEntities();
    }

    public void LoadAllEntities()
    {
        entitiesByTypes.Clear();
        cellDefinitions.Clear();
        TabDefinitions.Clear();
        BodyDefinitions.Clear();
        ResourceDefinitions.Clear();
        LoadResourceTypes();
        LoadBodyDefinitions();
        LoadPlayerEntities();
        LoadCellEntities();
        LoadItemEntities();
        LoadStructureEntities();
        LoadTabEntities();
        LoadProjectileEntities();
    }
    private void LoadPlayerEntities()
    {
        Dictionary<string, Entity> entities = new Dictionary<string, Entity>();
        PlayerEntity basePlayerEntity = gameObject.AddComponent<PlayerEntity>();
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Assets/EntityDefs/PlayerEntities");
        FileInfo[] info = dir.GetFiles("*.tyd", SearchOption.AllDirectories);
        foreach (FileInfo f in info)
        {
            TydFile tydFile = TydFile.FromFile(f.FullName);

            foreach (var node in tydFile.DocumentNode)
            {
                TydString str = node as TydString;

                if (str != null)
                {
                    Debug.Log(str.Value);
                    continue;
                }

                if (node.FullTyd.Contains("*abstract"))
                {
                    LoadEntity((TydCollection)node, ref basePlayerEntity);
                }
                else
                {
                    PlayerEntity newPlayerEntity = gameObject.AddComponent<PlayerEntity>();
                    Reflection.CopyProperties(basePlayerEntity, newPlayerEntity);
                    LoadEntity((TydCollection)node, ref newPlayerEntity);
                    entities.Add(newPlayerEntity.Name, newPlayerEntity);
                }
            }
        }
        EntitiesByTypes.Add("PlayerEntity", entities);
    }
    private void LoadStructureEntities()
    {
        Dictionary<string, Entity> entities = new Dictionary<string, Entity>();
        StructureEntity baseStructureEntity = gameObject.AddComponent<StructureEntity>();
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Assets/EntityDefs/StructureEntities");
        FileInfo[] info = dir.GetFiles("*.tyd", SearchOption.AllDirectories);

        foreach (FileInfo f in info)
        {
            TydFile tydFile = TydFile.FromFile(f.FullName);

            foreach (var node in tydFile.DocumentNode)
            {
                TydString str = node as TydString;

                if (str != null)
                {
                    Debug.Log(str.Value);
                    continue;
                }

                if (node.FullTyd.Contains("*abstract"))
                {
                    LoadEntity((TydCollection)node, ref baseStructureEntity);
                }
                else
                {
                    StructureEntity newStructureEntity = gameObject.AddComponent<StructureEntity>();
                    baseStructureEntity.CopyProperties(newStructureEntity);
                    LoadEntity((TydCollection)node, ref newStructureEntity);
                    entities.Add(newStructureEntity.Name, newStructureEntity);
                }
            }
        }
        EntitiesByTypes.Add("StructureEntity", entities);
    }
    private void LoadItemEntities()
    {
        Dictionary<string, Entity> entities = new Dictionary<string, Entity>();
        ItemEntity baseItemEntity = gameObject.AddComponent<ItemEntity>();
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Assets/EntityDefs/ItemEntities");
        FileInfo[] info = dir.GetFiles("*.tyd", SearchOption.AllDirectories);
        foreach (FileInfo f in info)
        {
            TydFile tydFile = TydFile.FromFile(f.FullName);

            foreach (var node in tydFile.DocumentNode)
            {
                TydString str = node as TydString;

                if (str != null)
                {
                    Debug.Log(str.Value);
                    continue;
                }

                if (node.FullTyd.Contains("*abstract"))
                {
                    LoadEntity((TydCollection)node, ref baseItemEntity);
                }
                else
                {
                    ItemEntity newItemEntity = gameObject.AddComponent<ItemEntity>();
                    Reflection.CopyProperties(baseItemEntity, newItemEntity);
                    LoadEntity((TydCollection)node, ref newItemEntity);
                    entities.Add(newItemEntity.Name, newItemEntity);
                }
            }
        }
        EntitiesByTypes.Add("ItemEntity", entities);
    }
    private void LoadTabEntities()
    {
        Tab baseTabEntity = gameObject.AddComponent<Tab>();
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Assets/EntityDefs/TabEntities");
        FileInfo[] info = dir.GetFiles("*.tyd", SearchOption.AllDirectories);
        foreach (FileInfo f in info)
        {
            TydFile tydFile = TydFile.FromFile(f.FullName);

            foreach (var node in tydFile.DocumentNode)
            {
                TydString str = node as TydString;

                if (str != null)
                {
                    Debug.Log(str.Value);
                    continue;
                }

                if (node.FullTyd.Contains("*abstract"))
                {
                    LoadEntity((TydCollection)node, ref baseTabEntity);
                }
                else
                {
                    Tab newTabEntity = gameObject.AddComponent<Tab>();
                    Reflection.CopyProperties(baseTabEntity, newTabEntity);
                    LoadEntity((TydCollection)node, ref newTabEntity);
                    tabDefinitions.Add(newTabEntity.Name, newTabEntity);
                }
            }
        }
    }
    private void LoadResourceTypes()
    {      
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Assets/EntityDefs/ResourceTypes");
        FileInfo[] info = dir.GetFiles("*.tyd", SearchOption.AllDirectories);
        foreach (FileInfo f in info)
        {
            TydFile tydFile = TydFile.FromFile(f.FullName);

            foreach (var node in tydFile.DocumentNode)
            {
                TydString str = node as TydString;

                if (str != null)
                {
                    Debug.Log(str.Value);
                    continue;
                }
                else
                {
                    ResourceType newResourceType = new ResourceType();
                    LoadEntity((TydCollection)node, ref newResourceType);
                    ResourceDefinitions.Add(newResourceType.Name, newResourceType);
                }
            }
        }
    }
    private void LoadProjectileEntities()
    {
        Dictionary<string, Entity> entities = new Dictionary<string, Entity>();
        ProjectileEntity baseProjectileEntity = gameObject.AddComponent<ProjectileEntity>();
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Assets/EntityDefs/projectileEntities");
        FileInfo[] info = dir.GetFiles("*.tyd", SearchOption.AllDirectories);
        foreach (FileInfo f in info)
        {
            TydFile tydFile = TydFile.FromFile(f.FullName);

            foreach (var node in tydFile.DocumentNode)
            {
                TydString str = node as TydString;

                if (str != null)
                {
                    Debug.Log(str.Value);
                    continue;
                }

                if (node.FullTyd.Contains("*abstract"))
                {
                    LoadEntity((TydCollection)node, ref baseProjectileEntity);
                }
                else
                {
                    ProjectileEntity newProjectileEntity = gameObject.AddComponent<ProjectileEntity>();
                    Reflection.CopyProperties(baseProjectileEntity, newProjectileEntity);
                    LoadEntity((TydCollection)node, ref newProjectileEntity);
                    entities.Add(newProjectileEntity.Name, newProjectileEntity);
                }
            }
        }
        entitiesByTypes.Add("ProjectileEntity", entities);
    }
    private void LoadCellEntities()
    {
        Cell baseCellEntity = new Cell();
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Assets/EntityDefs/CellEntities");
        FileInfo[] info = dir.GetFiles("*.tyd", SearchOption.AllDirectories);
        foreach (FileInfo f in info)
        {
            TydFile tydFile = TydFile.FromFile(f.FullName);

            foreach (var node in tydFile.DocumentNode)
            {
                TydString str = node as TydString;

                if (str != null)
                {
                    Debug.Log(str.Value);
                    continue;
                }

                if (node.FullTyd.Contains("*abstract"))
                {
                    LoadCellEntity((TydCollection)node, ref baseCellEntity);
                }
                else
                {
                    Cell newCellEntity = new Cell();
                    baseCellEntity.CopyProperties(newCellEntity);
                    LoadCellEntity((TydCollection)node, ref newCellEntity);
                    cellDefinitions.Add(newCellEntity.Name, newCellEntity);
                }
            }
        }
    }
    private void LoadBodyDefinitions()
    {    
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Assets/EntityDefs/BodyDefinitions");
        FileInfo[] info = dir.GetFiles("*.tyd", SearchOption.AllDirectories);
        foreach (FileInfo f in info)
        {
            TydFile tydFile = TydFile.FromFile(f.FullName);

            foreach (var node in tydFile.DocumentNode)
            {
                TydString str = node as TydString;

                if (str != null)
                {
                    Debug.Log(str.Value);
                    continue;
                }
                else
                {
                    Body newBody = new Body();
                    LoadBody((TydCollection)node, ref newBody);
                    BodyDefinitions.Add(newBody.Name, newBody);
                }
            }
        }
    }
    private void LoadEntity(TydCollection tydCollection, ref PlayerEntity entity)
    {
        foreach (var valueNode in tydCollection.Nodes)
        {
            TydString value;
            List<BaseComponent> components = new List<BaseComponent>();
            foreach (var component in entity.Components)
            {
                components.Add(component);
            }
            List<BaseAction> actions = new List<BaseAction>();
            foreach (var action in entity.Actions)
            {
                actions.Add(action);
            }
            if (valueNode.GetType() == typeof(TydList))
            {
                if (valueNode.Name == "Components")
                {
                    TydList componentList = (TydList)valueNode;
                    foreach (TydTable componentTable in componentList.Nodes)
                    {
                        components.Add(LoadComponent(componentTable));
                    }
                    entity.Components = components;
                }
                if (valueNode.Name == "Actions")
                {
                    TydList actionList = (TydList)valueNode;
                    foreach (TydTable actionTable in actionList.Nodes)
                    {
                        actions.Add(LoadAction(actionTable));
                    }
                    entity.Actions = actions;
                }

            }
            else
            {
                value = (TydString)valueNode;
                entity.SetPropertyOfName(value.Value, value.Name);
            }
        }
    }
    private void LoadEntity(TydCollection tydCollection, ref ItemEntity entity)
    {
        foreach (var valueNode in tydCollection.Nodes)
        {
            TydString value;
            List<BaseComponent> components = new List<BaseComponent>();
            foreach (var component in entity.Components)
            {
                components.Add(component);
            }
            if (valueNode.GetType() == typeof(TydList))
            {
                if (valueNode.Name == "Components")
                {
                    TydList componentList = (TydList)valueNode;
                    foreach (TydTable componentTable in componentList.Nodes)
                    {
                        components.Add(LoadComponent(componentTable));
                    }
                    entity.Components = components;
                }

            }
            else if (valueNode.GetType() == typeof(TydTable))
            {
            }
            else
            {
                value = (TydString)valueNode;
                entity.SetPropertyOfName(value.Value, value.Name);
            }
        }
    }
    private void LoadEntity(TydCollection tydCollection, ref Tab tab)
    {
        foreach (var valueNode in tydCollection.Nodes)
        {
            TydString value;
            value = (TydString)valueNode;
            tab.SetPropertyOfName(value.Value, value.Name);
        }
    }
    private void LoadEntity(TydCollection tydCollection, ref StructureEntity entity)
    {
        foreach (var valueNode in tydCollection.Nodes)
        {
            TydString value;
            List<BaseComponent> components = new List<BaseComponent>();
            foreach (var component in entity.Components)
            {
                components.Add(component);
            }
            if (valueNode.GetType() == typeof(TydList))
            {
                if (valueNode.Name == "Components")
                {
                    TydList componentList = (TydList)valueNode;
                    foreach (TydTable componentTable in componentList.Nodes)
                    {
                        components.Add(LoadComponent(componentTable));
                    }
                    entity.Components = components;
                }

            }
            else if (valueNode.GetType() == typeof(TydTable))
            {
                if (valueNode.Name == "ItemsNeeded")
                {
                    entity.ResourcesNeeded = LoadResourceList((TydTable)valueNode);
                }
            }
            else
            {
                value = (TydString)valueNode;
                entity.SetPropertyOfName(value.Value, value.Name);
            }
        }
    }
    private void LoadEntity(TydCollection tydCollection, ref ProjectileEntity entity)
    {
        foreach (var valueNode in tydCollection.Nodes)
        {
            TydString value;
            List<BaseComponent> components = new List<BaseComponent>();
            foreach (var component in entity.Components)
            {
                components.Add(component);
            }
            if (valueNode.GetType() == typeof(TydList))
            {
                if (valueNode.Name == "Components")
                {
                    TydList componentList = (TydList)valueNode;
                    foreach (TydTable componentTable in componentList.Nodes)
                    {
                        components.Add(LoadComponent(componentTable));
                    }
                    entity.Components = components;
                }

            }
            else
            {
                value = (TydString)valueNode;
                entity.SetPropertyOfName(value.Value, value.Name);
            }
        }
    }
    private void LoadEntity(TydCollection tydCollection, ref ResourceType entity)
    {
        foreach (var valueNode in tydCollection.Nodes)
        {
            TydString value = (TydString)valueNode;
            entity.SetPropertyOfName(value.Value, value.Name);
        }
    }
    private void LoadCellEntity(TydCollection tydCollection, ref Cell cellEntity)
    {
        foreach (var valueNode in tydCollection.Nodes)
        {
            TydString value = (TydString)valueNode;
            cellEntity.SetPropertyOfName(value.Value, value.Name);
        }
    }

    private void LoadBody(TydCollection tydCollection, ref Body body)
    {
        List<BodyPart> mainBodyParts = new List<BodyPart>();
        foreach (var valueNode in tydCollection.Nodes)
        {
            TydString value;

            if (valueNode.GetType() == typeof(TydTable))
            {
                mainBodyParts.Add(LoadBodyPart((TydTable)valueNode));
            }
            else
            {
                value = (TydString)valueNode;
                body.SetPropertyOfName(value.Value, value.Name);
            }
        }
        body.mainBodyParts = mainBodyParts;
    }
    private BodyPart LoadBodyPart(TydTable tydTable)
    {
        BodyPart bodyPart = new BodyPart();
        List<BodyPart> attachedBodyParts = new List<BodyPart>();

        foreach (var valueNode in tydTable.Nodes)
        {
            TydString value;

            if (valueNode.GetType() == typeof(TydTable))
            {
                attachedBodyParts.Add(LoadBodyPart((TydTable)valueNode));
            }
            else
            {
                value = (TydString)valueNode;
                bodyPart.SetPropertyOfName(value.Value, value.Name);
            }
        }
        bodyPart.attachedBodyParts = attachedBodyParts;
        return bodyPart;
    }

    private List<Resource> LoadResourceList(TydTable tydList)
    {
        List<Resource> itemList = new List<Resource>();
        foreach (var node in tydList)
        {
            TydString value = (TydString)node;
            Resource resource = new Resource();
            ResourceType resourceInfo = ResourceDefinitions[value.Name];
            resourceInfo.CopyProperties(resource);
            resource.Amount = int.Parse(value.Value);
            itemList.Add(resource);
        }
        return itemList;
    }

    private BaseComponent LoadComponent(TydTable componentTable)
    {
        TydString className = (TydString)componentTable.Nodes[0];

        Type t = Type.GetType(className.Value);
        var component = (BaseComponent)gameObject.AddComponent(t);

        foreach (var valueNode in componentTable.Nodes)
        {
            if (valueNode.GetType() == typeof(TydTable))
            {
                component.SetReferenceDictionaryOfName(LoadItemReferences((TydTable)valueNode), valueNode.Name);
            }
            else
            {
                TydString value = (TydString)valueNode;
                if (value.Name.Contains("Class")) { continue; }
                component.SetPropertyOfName(value.Value, value.Name);
            }

        }
        return component;
    }

    private BaseAction LoadAction(TydTable actionTable)
    {
        TydString className = (TydString)actionTable.Nodes[0];

        Type t = Type.GetType(className.Value);
        BaseAction action = (BaseAction)Activator.CreateInstance(t);

        foreach (var valueNode in actionTable.Nodes)
        {
            if (valueNode.GetType() == typeof(TydTable))
            {
                action.SetReferenceDictionaryOfName(LoadItemReferences((TydTable)valueNode), valueNode.Name);
            }
            else
            {
                TydString value = (TydString)valueNode;
                if (value.Name.Contains("Action")) { continue; }
                action.SetPropertyOfName(value.Value, value.Name);
            }

        }
        return action;
    }

    private Dictionary<string, int> LoadItemReferences(TydTable itemsTable)
    {
        Dictionary<string, int> itemDictionary = new Dictionary<string, int>();

        foreach (var valueNode in itemsTable.Nodes)
        {
            TydString value = (TydString)valueNode;
            itemDictionary.Add(value.Name, int.Parse(value.Value));
        }

        return itemDictionary;
    }

    private void LogEachThing(TydCollection tydFile)
    {
        foreach (var node in tydFile)
        {
            TydString str = node as TydString;

            if (str != null)
            {
                Debug.Log(str.Value);
                continue;
            }
            TydCollection collection = (TydCollection)node;
            if (collection.Nodes != null) { LogEachThing(collection); }
        }
        //Debug.Log(tydFile.FullTyd);
    }
}
