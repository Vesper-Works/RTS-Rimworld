using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
public class SelectionHandler : MonoBehaviour
{
    private Vector3 startPos;
    public GameObject boxSelectGraphic;
    public GameObject objectSelectedGraphic;
    private bool selecting = true;
    private List<Entity> selectedEntities = new List<Entity>();

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI basicText;
    [SerializeField] private GameObject equipmentView;
    private static SelectionHandler instance { get; set; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        if (selecting) { SelectActiveEntities(); }
    }
    private void SelectActiveEntities()
    {
        // if (selectedUIElement != null) { return; }

        if (Input.GetMouseButtonDown(0))
        {
            startPos = GridUtility.GetMouseWorldPosition();
            if (selectedEntities.Count > 0)
            {
                foreach (var entity in selectedEntities)
                {
                    entity.OnDeselect();
                }
            }
            boxSelectGraphic.transform.position = startPos;
        }

        if (Input.GetMouseButton(0))
        {
            boxSelectGraphic.transform.localScale = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - startPos.x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y - startPos.y);
            //boxSelectGraphic.GetComponentInChildren<SpriteRenderer>().size = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - startPos.x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y - startPos.y);
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (!Input.GetKey(KeyCode.LeftShift)) { selectedEntities.Clear(); }

            Collider2D[] colliders = Physics2D.OverlapAreaAll(startPos, Camera.main.ScreenToWorldPoint(Input.mousePosition));

            if (colliders.Length > 1)
            {
                foreach (Collider2D collider in colliders)
                {
                    if (selectedEntities.Count >= 50) { break; }

                    Entity entity = collider.GetComponent<Entity>();
                    if (entity == null) { continue; }

                    //if (entity.Owner != SmartFoxConnection.Connection.MySelf) { continue; }
                    selectedEntities.Add(entity);

                }
            }
            else
            {
                foreach (var cell in GridUtility.FindSquareOfCells(startPos + new Vector3(0.5f, 0.5f, 0), Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0.5f, 0.5f, 0)))
                {
                    if (selectedEntities.Count >= 50) { break; }
                    if (!cell.IsEmpty && cell.EntityContained.Selectable) { selectedEntities.Add(cell.EntityContained); }
                }
            }

            boxSelectGraphic.transform.localScale = Vector2.zero;
        }
        if (Input.GetMouseButtonUp(0) && selectedEntities.Count > 0)
        {
            if (selectedEntities.Exists(x => x.GetType().IsSubclassOf(typeof(MoveableEntity))))
            {
                for (int i = 0; i < selectedEntities.Count; i++)
                {
                    if (selectedEntities[i].GetType().IsSubclassOf(typeof(MoveableEntity)))
                    {
                        if (selectedEntities[i].selectedGraphic == null)
                        {
                            selectedEntities[i].selectedGraphic = Instantiate(objectSelectedGraphic,
                                selectedEntities[i].transform.position, Quaternion.identity, selectedEntities[i].transform);
                            selectedEntities[i].selectedGraphic.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        }
                        selectedEntities[i].OnSelect();
                    }
                    else
                    {
                        selectedEntities.RemoveAt(i);
                        i--;
                    }
                }
            }
            else
            {
                foreach (var entity in selectedEntities)
                {

                    if (entity.selectedGraphic == null)
                    {
                        entity.selectedGraphic = Instantiate(objectSelectedGraphic,
                            entity.transform.position, Quaternion.identity, entity.transform);
                        entity.selectedGraphic.GetComponent<SpriteRenderer>().sortingOrder = 1;
                    }
                    entity.OnSelect();
                }

            }

        }
        if (selectedEntities.Count == 1)
        {
            Entity entity = selectedEntities[0];
            ((RectTransform)gameObject.transform).anchoredPosition = new Vector2(0, 54);
            nameText.text = entity.Name;
            basicText.text = entity.ToBasicString();
            if (typeof(MoveableEntity).IsAssignableFrom(entity.GetType()) && ((MoveableEntity)entity).HasWeapon)
            {
                ((RectTransform)equipmentView.transform).anchoredPosition = new Vector2(0, 280);
                equipmentView.GetComponent<EquipmentViewSetup>().Setup(((MoveableEntity)entity).Weapon);
            }
            else
            {
                ((RectTransform)equipmentView.transform).anchoredPosition = new Vector2(-1000, -1000);
            }
        }
        else { ((RectTransform)gameObject.transform).anchoredPosition = new Vector2(-1000, -1000); }
    }

    public static void DisableSelection()
    {
        instance.selecting = false;
    }
    public static void ClearCurrentSelection()
    {
        if (instance.selectedEntities.Count > 0)
        {
            foreach (var entity in instance.selectedEntities)
            {
                entity.OnDeselect();
            }
        }
        instance.selectedEntities.Clear();
        Debug.Log("Cleared selection");
    }
    public static void EnableSelection()
    {
        instance.selecting = true;
        instance.startPos = GridUtility.GetMouseWorldPosition();
    }
}
