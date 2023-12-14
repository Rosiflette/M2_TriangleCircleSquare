using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Emgu.CV;

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject shapesHolder;

    [SerializeField] private List<GameObject> shapes;
    [SerializeField] private List<GameObject> spawnZones;
    [SerializeField] public float speed;
    [SerializeField] private float spawnTime;
    private float t;
    private List<Vector3> startingPoints;


    public static GameManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        Mat img = new Mat("C:\\Users\\rchapelle\\Pictures\\Totoro3.jpg");
        Debug.Log(img.Size);

        if (Instance == null) Instance = this;

        float third = Screen.width/ shapes.Count;
        startingPoints = new List<Vector3>();
        startingPoints.Add(Vector3.left * third);
        startingPoints.Add(Vector3.zero * third);
        startingPoints.Add(Vector3.right * third);
        t = 0;

    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if(t > spawnTime)
        {
            int randomShapeIndex = Random.Range(0,shapes.Count);
            Instantiate(shapes[randomShapeIndex], spawnZones[randomShapeIndex].transform);
            t = 0;
        }


    }

}
