using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    RectTransform rect;
    BoxCollider box;

    Vector2 size;

    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        size = rect.rect.size;
        box = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (size != rect.rect.size)
        {
            size = rect.rect.size;
            box.size = new Vector3(rect.rect.width, rect.rect.height / 2, 1);
        }
    }
}
