using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINode : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    private bool isDragging;
    private Vector3 offset;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.size = spriteRenderer.size;
    }

    void Update()
    {

    }

    void OnMouseDown()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (boxCollider.OverlapPoint(mousePos))
        {
            Bounds bounds = boxCollider.bounds;
            float height = bounds.size.y;
            float topThreshold = bounds.min.y + 0.8f * height;

            if (mousePos.y >= topThreshold)
            {
                isDragging = true;
                offset = transform.position - mousePos;
            }
        }
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos + offset;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
    }
}
