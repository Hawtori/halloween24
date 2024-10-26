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

    [Header("Maze Puzzle Requirements")]
    [SerializeField] private List<GameObject> mazePuzzlePieces = new List<GameObject>(); // the buttons to press
    [SerializeField] private List<GameObject> mazePuzzleHints = new List<GameObject>(); // the hint images on what order to press buttons in
        

    private void Awake()
    {
        rooms.Add(typeof(ShapeOrientationPuzzle));
        rooms.Add(typeof(MazeWithButtons));

        AddRoomToDictionary(typeof(ShapeOrientationPuzzle), orientationPuzzlePieces, orientationPuzzleHints);
        AddRoomToDictionary(typeof(MazeWithButtons), mazePuzzlePieces, mazePuzzleHints);
    }

    private void AddRoomToDictionary(System.Type roomType, List<GameObject> puzzleObjects, List<GameObject> hintObjects)
    {
        List<List<GameObject>> requirements = new List<List<GameObject>>
        {
            puzzleObjects,
            hintObjects
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

            foreach (var room in roomsList) 
                roomSpawned.roomsWeCanWorkWith.Add(room);

        }

        roomSpawned.enabled = true;
    }

    private void Start()
    {
        InitiateFillingPuzzle();
    }
}
