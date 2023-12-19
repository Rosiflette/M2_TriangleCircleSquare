using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Emgu.CV;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject shapesHolder;

    [SerializeField] private List<GameObject> shapes;
    [SerializeField] private List<GameObject> spawnZones;
    [SerializeField] private List<GameObject> doors;
    [SerializeField] public float speed;
    [SerializeField] private float spawnTime;
    [SerializeField] private GameObject canva;
    private float t;
    private List<Vector3> startingPoints;
    public float timePassed = 0;
    public bool isGameEnded = false;
    public int life;

    public static GameManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null) Instance = this;

        float third = Screen.width/ shapes.Count;
        startingPoints = new List<Vector3>();
        startingPoints.Add(Vector3.left * third);
        startingPoints.Add(Vector3.zero * third);
        startingPoints.Add(Vector3.right * third);
        t = 0;
        life = 3;

    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        t += Time.deltaTime;
        if(life <= 0 )
        {
            FinishGame();
        }
        if(t > spawnTime)
        {
            int randomShapeIndex = Random.Range(0,shapes.Count);
            Instantiate(shapes[randomShapeIndex], spawnZones[randomShapeIndex].transform);
            t = 0;
        }
    }

    public void UpdateShapeCount(int circle, int square, int triangle)
    {
        doors[0].SetActive(circle > 0);
        doors[1].SetActive(square > 0);
        doors[2].SetActive(triangle > 0);
    }

    public void FinishGame()
    {
        GameManager.Instance.isGameEnded = true;
        canva.GetComponent<UIManager>().FinishGame();
    }

    public void Restart()
    {
        life = 3;
        isGameEnded = false;
    }
}
