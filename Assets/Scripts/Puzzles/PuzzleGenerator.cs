using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class PuzzleGenerator : MonoBehaviour
{
    [Header("Puzzle Room Decision")]
    public List<System.Type> rooms = new List<System.Type>();
    private List<GameObject> roomsList = new List<GameObject>();

    [Header("Dictionary of rooms")]
    private Dictionary<System.Type, List<List<GameObject>>> roomDictionary = new Dictionary<System.Type, List<List<GameObject>>>();

    [Header("Orientation Puzzle Requirements")]
    [SerializeField] private List<GameObject> orientationPuzzlePieces = new List<GameObject>(); // the statues to orient
    [SerializeField] private List<GameObject> orientationPuzzleHints = new List<GameObject>(); // the hint images in what way to orient the shapes
    [SerializeField] private List<GameObject> orientationEnemies = new List<GameObject>(); // the enemies for this level

    [Header("Maze Puzzle Requirements")]
    [SerializeField] private List<GameObject> mazePuzzlePieces = new List<GameObject>(); // the buttons to press
    [SerializeField] private List<GameObject> mazePuzzleHints = new List<GameObject>(); // the hint images on what order to press buttons in
    [SerializeField] private List<GameObject> mazeEnemies = new List<GameObject>(); // the enemies for this level
        

    private void Awake()
    {
        if (WorldGeneration.Instance.isInShop)
        {
            Destroy(this);
            return;
        }

        Random.InitState(System.DateTime.Now.Millisecond);

        rooms.Add(typeof(ShapeOrientationPuzzle));
        rooms.Add(typeof(MazeWithButtons));

        AddRoomToDictionary(typeof(ShapeOrientationPuzzle), orientationPuzzlePieces, orientationPuzzleHints, orientationEnemies);
        AddRoomToDictionary(typeof(MazeWithButtons), mazePuzzlePieces, mazePuzzleHints, mazeEnemies);
    }

    private void AddRoomToDictionary(System.Type roomType, List<GameObject> puzzleObjects, List<GameObject> hintObjects, List<GameObject> enemies)
    {
        List<List<GameObject>> requirements = new List<List<GameObject>>
        {
            puzzleObjects,
            hintObjects,
            enemies
        };
        roomDictionary[roomType] = requirements;
    }

    private void InitiateFillingPuzzle()
    {
        int puzzleToSpawn = Random.Range(0, rooms.Count);

        PuzzleRoom roomSpawned = gameObject.AddComponent(rooms[puzzleToSpawn]) as PuzzleRoom; // change to puzzleToSpawn
        roomSpawned.enabled = false;

        roomsList = WorldGeneration.Instance.GetRooms();

        if (roomDictionary.TryGetValue(rooms[puzzleToSpawn], out List<List<GameObject>> prefabs))
        {
            if (prefabs.Count > 0)
            {
                foreach (var prefab in prefabs[0])
                {
                    roomSpawned.puzzleObjects.Add(prefab);
                }
            }

            if (prefabs.Count > 1)
            {
                foreach (var prefab in prefabs[1]) roomSpawned.hintObjects.Add(prefab);
            }

            if(prefabs.Count > 2)
            {
                foreach(var prefab in prefabs[2]) roomSpawned.puzzleEnemies.Add(prefab);
            }

            foreach (var room in roomsList) 
                roomSpawned.roomsWeCanWorkWith.Add(room);

        }

        roomSpawned.enabled = true;
    }

    private void Start()
    {
        if (WorldGeneration.Instance.isInShop)
        {
            return;
        }
        InitiateFillingPuzzle();
    }
}
