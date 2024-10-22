using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> roomDoors;
    [SerializeField]
    private int maxLevel = 25;
    private int levelcount = 0;

    [SerializeField]
    private string doorLayer;

    [SerializeField]
    private List<GameObject> rooms = new List<GameObject>();
    [SerializeField]
    private GameObject single;

    private void Start()
    {
        Random.InitState((int)Time.time);

        SpawnRooms();
    }

    private void SpawnRooms()
    {
        while (roomDoors.Count > 0 && levelcount < maxLevel)
        {
            // spawn door
            GameObject currentRoom = Instantiate(rooms[Random.Range(0, rooms.Count)], roomDoors[0].transform.position, Quaternion.identity, transform);

            SpawnRoom(currentRoom);

            levelcount++;
            roomDoors.RemoveAt(0);
        }

        int i = 0;
        foreach (GameObject door in roomDoors)
        {
            SpawnRoom(Instantiate(single, roomDoors[i].transform.position, Quaternion.identity, transform), i++);
        }
    }


    ///
    /// Took too long and had to use almost all my brain cells for this one function so 
    /// variable names are super specific
    ///
    /// <param name="currentRoom">The newly spawned room we are working with</param>
    /// <param name="index">Which door we're working with</param>
    private void SpawnRoom(GameObject currentRoom, int index = 0)
    {
        // get any door from that room
        List<Transform> doors = GetDoors(currentRoom); // all the doors in the new room
        int doorIndex = Random.Range(0, doors.Count); // a random door


        //**************************** OFFSET OF THE ROOM ****************************//

        // offset the room position to match door to door
        // offset distance room's door to room in the direction of prev door's room to prev door
        float offsetDistance = Vector3.Distance(doors[doorIndex].position, currentRoom.transform.position); // the distance between the room's door and the room

        // offset direction
        Vector3 directionFromPrevRoomToPrevDoor = (roomDoors[index].transform.position - roomDoors[index].transform.parent.position).normalized; 

        // move room to its position
        currentRoom.transform.position = roomDoors[index].transform.position + (offsetDistance * directionFromPrevRoomToPrevDoor);
              

        //**************************** ROTATION OF THE ROOM ****************************//

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
        Quaternion rotateToPointAtBeforeRoom = Quaternion.FromToRotation(dirRoomToDoor, dirRoomToPreviousRoom);// * currentRoom.transform.rotation;

        Quaternion currentRoomRotation = currentRoom.transform.rotation;

        Quaternion newRotationForCurrentRoom = Quaternion.Euler(currentRoomRotation.eulerAngles.x, rotateToPointAtBeforeRoom.eulerAngles.y, currentRoomRotation.eulerAngles.z);

        // rotate the room to point at the door
        currentRoom.transform.rotation = newRotationForCurrentRoom;

        foreach (Transform door in doors)
        {
            if (door != doors[doorIndex]) roomDoors.Add(door.gameObject);
        }

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

}
