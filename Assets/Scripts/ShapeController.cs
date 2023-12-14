using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position += Vector3.down * Time.deltaTime * GameManager.Instance.speed;
    }

    private void OnTriggerEnter(Collider collision)
    {
        Destroy(gameObject);
    }
}
