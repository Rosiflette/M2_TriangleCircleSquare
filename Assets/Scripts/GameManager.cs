using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private RectTransform canva;
    [SerializeField] private Transform column1;
    [SerializeField] private Transform column2;
    [SerializeField] private Transform column3;



    // Start is called before the first frame update
    void Start()
    {
        float third = canva.rect.width / 3;

        column1.position += Vector3.left * third;
        column3.position += Vector3.right * third;

    }

    // Update is called once per frame
    void Update()
    {


    }
}
