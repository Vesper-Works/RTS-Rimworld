using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoTab : MonoBehaviour
{
    private TextMeshPro text;
    private Entity entity;
    private int direction = 1;
    private int numLines;
    LineRenderer lineRenderer;
    private bool hovered;
    public void DisplayInfo(Entity entity)
    {
        gameObject.layer = 5;
        text = GetComponentInChildren<TextMeshPro>();
        text.text = entity.ToBasicString();
        text.fontSize = 0.8f;
        text.enableWordWrapping = false;
        this.entity = entity;
        Vector2 entityPosition = entity.transform.position;
        transform.position = new Vector2(entityPosition.x + (Camera.main.orthographicSize / 2f), entityPosition.y);

        numLines = text.text.Split('\n').Length - 1;

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 3;
        lineRenderer.SetPosition(0, entityPosition);
        lineRenderer.SetPosition(1, transform.position);
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.endColor = new Color(0, 0, 0, 0.6f);
        lineRenderer.startColor = new Color(0, 0, 0, 0.6f);
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        
    }

    private void Update()
    {
        text = GetComponentInChildren<TextMeshPro>();
        if (entity != null && text != null) { text.text = entity.ToBasicString(); }

        Vector2 entityPosition = entity.transform.position;

        Vector2 relativeEntityPosition = new Vector2(Camera.main.WorldToScreenPoint(entityPosition, Camera.MonoOrStereoscopicEye.Mono).x / Camera.main.pixelWidth,
                                                     Camera.main.WorldToScreenPoint(entityPosition, Camera.MonoOrStereoscopicEye.Mono).y / Camera.main.pixelHeight);

        if (relativeEntityPosition.x < 0.3f)
        {
            direction = 1;

        }
        else if (relativeEntityPosition.x > 0.7f)
        {
            direction = -1;
        }
        transform.position = new Vector2((entityPosition.x + ((Camera.main.orthographicSize / 4f + 2) * direction)),
                                         (entityPosition.y + 2 + (0.05f * numLines)));
        lineRenderer.SetPosition(0, entityPosition);
        lineRenderer.SetPosition(1, 
            new Vector2((entityPosition.x + ((Camera.main.orthographicSize / 2f + 2) * direction) + (Camera.main.orthographicSize/2 * -direction)),
                        entityPosition.y + 2));
        lineRenderer.SetPosition(2, 
            new Vector2((entityPosition.x + ((Camera.main.orthographicSize / 2f + 2) * direction) + ((Camera.main.orthographicSize / 2 - 0.05f) * -direction)),
                        entityPosition.y + 2));

        transform.localScale = new Vector2(Camera.main.orthographicSize / 2f * (1 / transform.parent.localScale.x), Camera.main.orthographicSize / 2f * (1 / transform.parent.localScale.y));
        GetComponent<BoxCollider2D>().size = new Vector2(1f, 0.1f * numLines);
        GetComponent<SpriteRenderer>().size = new Vector2(1f, 0.1f * numLines);
        //transform.GetChild(0).localPosition = new Vector2(0, (-0.15f * numLines));
        if (Input.GetMouseButtonDown(0) && hovered)
        {
            DisplayIndepthStats();
        }
        if (!entity.Selected) { Destroy(gameObject); }

        //transform.localScale = new Vector3(1 / transform.parent.localScale.x, 1 / transform.parent.localScale.y, 1 / transform.parent.localScale.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.Equals("MouseCollider"))
        {
            hovered = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name.Equals("MouseCollider"))
        {
            hovered = false;
        }
    }
    public void DisplayIndepthStats()
    {
        InfoTabCreator.CreateDetailedInfoTab(entity);
    }
}
