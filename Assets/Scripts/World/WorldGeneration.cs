using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class WorldGeneration : MonoBehaviour
{
    [Header("Instance")]
    public static WorldGeneration Instance;

    [Header("Room Management")]
    [SerializeField]
    private List<GameObject> roomDoors;
    private List<GameObject> excessDoors = new List<GameObject>();
    private List<GameObject> roomsList;
    [SerializeField]
    private int maxLevel = 25;
    private int levelcount = 0;

    [Header("Layers")]
    [SerializeField]
    private string doorLayer;

    [Header("Room prefabs")]
    [SerializeField]
    private List<GameObject> rooms = new List<GameObject>();
    [SerializeField]
    private List<GameObject> single = new List<GameObject>();
    [SerializeField]
    private GameObject doorObject;
    public GameObject startingRoom;

    [Header("Data structure")]
    private List<NavMeshSurface> surfaces = new List<NavMeshSurface>();

    private Tree tree = new Tree();
    public float adjacencyDistance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(this);
    }

    private void Start()
    {
        roomsList = new List<GameObject>();
        UnityEngine.Random.InitState((int)Time.time);

        SpawnRooms();

        if (tree.allRoomNodes.Count > 0) tree.head = tree.allRoomNodes[0];
        else Debug.Log("Tree has no nodes");

        PopulateSurfaces();
        //ConnectSurfaces();
    }

    private bool updatedLinks = false;

    private void Update()
    {
        if (updatedLinks) return;
        tree.CreateNavmeshLinks();
        updatedLinks = true;
    }

    private void PopulateSurfaces()
    {
        foreach(var room in roomsList)
        {
            NavMeshSurface surface = room.GetComponentInChildren<NavMeshSurface>();
            if (surface != null)
                surfaces.Add(surface);
            else Debug.Log("no surface found");
        }
    }

    private void SpawnRooms()
    {
        // spawn a room, if you can't try again 
        int currentLoop = 0;
        while (roomDoors.Count > 0 && levelcount < maxLevel)
        //for(int j = 0; j < 3; j++)
        {
            GameObject currentRoom = Instantiate(rooms[UnityEngine.Random.Range(0, rooms.Count)], roomDoors[0].transform.position, Quaternion.identity, transform);

            // couldn't spawn any room in this position
            if (!SpawnRoom(currentRoom))
            {
                currentLoop++;
                if (currentLoop < rooms.Count - 1)
                    continue;
            }

            GameObject parentRoom = roomDoors[0].transform.parent.gameObject;
            tree.AddNode(parentRoom, currentRoom);


            levelcount++;
            roomDoors.RemoveAt(0);

            currentLoop = 0;
        }


        // in any remaining door spots, try spawning an end room
        int i = 0;
        foreach (GameObject door in roomDoors)
        {
            currentLoop = 0;
            // try spawning two random end rooms
startOfLoop:
            if (
            !SpawnRoom(Instantiate(single[UnityEngine.Random.Range(0, single.Count)], door.transform.position, Quaternion.identity, transform), i++)
            )
            {
                // loop once
                if(currentLoop == 0)
                {
                    i--;
                    currentLoop++;
                    goto startOfLoop;
                }

                SpawnDoor(door.transform.position, Quaternion.LookRotation(door.transform.parent.position - door.transform.position, Vector3.up), true);

                // couldn't spawn it, so we spawn a door here
                //if (doorObject)
                //{
                //    Quaternion doorRotation = Quaternion.LookRotation(door.transform.parent.position - door.transform.position, Vector3.up);
                //    Vector3 finalRotation = doorRotation.eulerAngles;

                //    GameObject spawnedDoor = Instantiate(doorObject, door.transform.position, Quaternion.Euler(finalRotation));
                //    Debug.LogWarning("Spawning door where no end room could go");

                //    //Collider[] col = Physics.OverlapBox(spawnedDoor.transform.position, spawnedDoor.GetComponent<Collider>().bounds.extents, Quaternion.identity);
                //    //foreach(var c in col)
                //    //{
                //    //    if(c.gameObject != gameObject && c.gameObject.name.Contains("Door"))
                //    //    {
                //    //        Debug.Log("Destroying another door");
                //    //        //Destroy(c.GetComponent<DoorTemp>());
                //    //        //Destroy(spawnedDoor);
                //    //        break;
                //    //    }
                //    //}
                //}
                //else Debug.Log("No door prefab object");
            }
        }

        //foreach (GameObject door in excessDoors)
        //{
        //    currentLoop = 0;
        //// try spawning two random end rooms
        //startOfLoop:
        //    if (
        //    !SpawnRoom(Instantiate(single[UnityEngine.Random.Range(0, single.Count)], door.transform.position, Quaternion.identity, transform), i++)
        //    )
        //    {
        //        // loop once
        //        if (currentLoop == 0)
        //        {
        //            i--;
        //            currentLoop++;
        //            goto startOfLoop;
        //        }

        //        // couldn't spawn it, so we spawn a door here
        //        if (doorObject)
        //        {
        //            Quaternion doorRotation = Quaternion.LookRotation(door.transform.parent.position - door.transform.position, Vector3.up);
        //            Vector3 finalRotation = doorRotation.eulerAngles;

        //            GameObject spawnedDoor = Instantiate(doorObject, door.transform.position, Quaternion.Euler(finalRotation));

        //            //Collider[] col = Physics.OverlapBox(spawnedDoor.transform.position, spawnedDoor.GetComponent<Collider>().bounds.extents, Quaternion.identity);
        //            //foreach(var c in col)
        //            //{
        //            //    if(c.gameObject != gameObject && c.gameObject.name.Contains("Door"))
        //            //    {
        //            //        Debug.Log("Destroying another door");
        //            //        //Destroy(c.GetComponent<DoorTemp>());
        //            //        //Destroy(spawnedDoor);
        //            //        break;
        //            //    }
        //            //}
        //        }
        //        else Debug.Log("No door prefab object");
        //    }
        //}
    }


    ///
    /// Took too long and had to use almost all my brain cells for this one function so 
    /// variable names are super specific
    ///
    /// <param name="currentRoom">The newly spawned room we are working with</param>
    /// <param name="index">Which door we're working with</param>
    private bool SpawnRoom(GameObject currentRoom, int index = 0)
    {
        // get any door from that room
        List<Transform> doors = GetDoors(currentRoom); // all the doors in the new room
        int startingDoorIndex = UnityEngine.Random.Range(0, doors.Count); // a random door
        int doorIndex = startingDoorIndex;

        int rotationIndex = 0;

        bool flag = false;

        do
        {
            //**************************** OFFSET OF THE ROOM ****************************//

            // offset the room position to match door to door
            // offset distance room's door to room in the direction of prev door's room to prev door
            float offsetDistance = Vector3.Distance(doors[doorIndex].position, currentRoom.transform.position); // the distance between the room's door and the room

            // offset direction
            Vector3 directionFromPrevRoomToPrevDoor = (roomDoors[index].transform.position - roomDoors[index].transform.parent.position).normalized;

            // move room to its position
            currentRoom.transform.position = roomDoors[index].transform.position + (offsetDistance * directionFromPrevRoomToPrevDoor);


            //**************************** ROTATION OF THE ROOM ****************************//

            // rotate the room to point at the door
            currentRoom.transform.rotation = RotateRoom(currentRoom, doors, doorIndex, index);


            if (!IsRoomPositionValid(currentRoom)) // room isn't valid, we retry with rotation
            {
                if (doors.Count == 1) // its a single door room
                {
                    flag = false;
                    break;
                }
                doorIndex = (doorIndex + 1) % doors.Count;
                rotationIndex++;
            }
            else
            { // room is valid, we can add
                flag = true;
                break;
            }

        } while (rotationIndex < doors.Count);

        if (!flag)
        {
            //Debug.Log("Room wasn't valid. Room in question: " + currentRoom.name);
            tree.RemoveNode(currentRoom);
            Destroy(currentRoom);
            return false;
        }

        roomsList.Add(currentRoom);

        
        foreach (Transform door in doors)
        {
            // add any door slots of this room to list of doors
            if (door != doors[doorIndex])
            {
                roomDoors.Add(door.gameObject);
                //SpawnDoor(door.transform.position, Quaternion.LookRotation(currentRoom.transform.position - door.position, Vector3.up));
            }
        }
        return true;
    }

    private void SpawnDoor(Vector3 position, Quaternion rotation, bool endPiece = false)
    {
        if(!doorObject) { Debug.Log("No door prefab"); return; }

        GameObject spawnedDoor = Instantiate(doorObject, position + (Vector3.up * doorObject.transform.localScale.y/2f), rotation);
        if (endPiece) Destroy(spawnedDoor.GetComponent<DoorTemp>());

        Collider[] col = Physics.OverlapBox(spawnedDoor.transform.position, spawnedDoor.GetComponent<Collider>().bounds.extents/2f, Quaternion.identity);

        foreach (var c in col)
        {
            if (c == null) continue;
            if (c.gameObject != spawnedDoor && c.gameObject.name.Contains("Door"))
            {
                if(endPiece)
                    Destroy(c.GetComponent<DoorTemp>());
                Destroy(spawnedDoor);
                return;
            }
        }
    }

    private Quaternion RotateRoom(GameObject currentRoom, List<Transform> doors, int doorIndex, int index)
    {
        // rotate room around y
        // vector from current room to door we're working with
        Vector3 dirRoomToDoor = (doors[doorIndex].position - currentRoom.transform.position).normalized;

        // make the direction match currentRoom to roomDoors[0]
        Vector3 dirRoomToPreviousRoom = (roomDoors[index].transform.position - currentRoom.transform.position).normalized;

        // get angle between dirRoomToDoor and dirRoomToPreviousRoom
        // rotate currentRoom that amount
        float rotationAngleToMatchDoors = Vector3.Angle(dirRoomToDoor, dirRoomToPreviousRoom);

        //Quaternion rotationToDo = Quaternion.AngleAxis(rotationAngleToMatchDoors, Vector3.up);

        // make that direction face the door we're working with
        Quaternion rotateToPointAtBeforeRoom = Quaternion.FromToRotation(dirRoomToDoor, dirRoomToPreviousRoom);

        Quaternion currentRoomRotation = currentRoom.transform.rotation;

        return Quaternion.Euler(currentRoomRotation.eulerAngles.x, rotateToPointAtBeforeRoom.eulerAngles.y, currentRoomRotation.eulerAngles.z);
    }

    private bool IsRoomPositionValid(GameObject currentRoom)
    {
        Bounds roomBound = currentRoom.GetComponentInChildren<Renderer>().bounds;
        foreach (GameObject room in roomsList)
        {
            Bounds rmBound = room.GetComponentInChildren<Renderer>().bounds;
            if (roomBound.Intersects(rmBound))
            {
                return false;
            }
        }

        return true;
    }

    private List<Transform> GetDoors(GameObject room)
    {
        List<Transform> result = new List<Transform>();

        foreach(Transform child in room.transform)
        {
            if (child.gameObject.layer == LayerMask.NameToLayer(doorLayer))
            {
                result.Add(child);
            }
        }

        return result;
    }

    public List<GameObject> GetRooms() => roomsList;

}

// ********************************* EXTRAS ********************************* //
//private void ConnectSurfaces()
//{
//    for (int i = 0; i < surfaces.Count; i++)
//    {
//        for (int j = i + 1; j < surfaces.Count; j++)
//        {
//            if (IsRoomAdjacent(surfaces[i], surfaces[j]))
//            {
//                CreateLink(surfaces[i], surfaces[j]);
//            }
//        }
//    }

//    // get node
//    // get child node
//    // create link
//    // redo using child node


//}

//private bool IsRoomAdjacent(NavMeshSurface A, NavMeshSurface B)
//{
//    if(Vector3.Distance(A.transform.position, B.transform.position) < adjacencyDistance) Debug.Log("Is adjacent: " + A.transform.name + " and " + B.transform.name);
//    else Debug.Log("Is not adjacent: " + A.transform.name + " and " + B.transform.name);

//    return Vector3.Distance(A.transform.position, B.transform.position) < adjacencyDistance;
//}

//private void CreateLink(NavMeshSurface A, NavMeshSurface B)
//{
//    //Vector3 startPoint = A.transform.position; // Adjust to a specific point if needed
//    //Vector3 endPoint = B.transform.position; // Adjust to a specific point if needed

//    //NavMeshLink link = new GameObject("NavMeshLink").AddComponent<NavMeshLink>();
//    //link.startPoint = startPoint;
//    //link.endPoint = endPoint;
//    //link.width = 1.0f; // Adjust as necessary
//    //link.costModifier = 1; // Adjust cost if needed
//    //link.bidirectional = true;

//    //// Optionally, you can set the area type for the link
//    //link.area = 0; // Set the appropriate area

//    Vector3 startPoint = A.transform.position + new Vector3(0, 0.5f, 0); // Adjust height if necessary
//    Vector3 endPoint = B.transform.position + new Vector3(0, 0.5f, 0); // Adjust height if necessary

//    NavMeshLink link = new GameObject("NavMeshLink").AddComponent<NavMeshLink>();
//    link.startPoint = startPoint;
//    link.endPoint = endPoint;
//    link.width = 1.0f; // Adjust width as needed
//    link.costModifier = 1; // Adjust cost if needed
//    link.bidirectional = true;
//    link.area = 0; // Set the appropriate area type
//}
