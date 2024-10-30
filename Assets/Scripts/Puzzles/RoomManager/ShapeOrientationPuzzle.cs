using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeOrientationPuzzle : PuzzleRoom
{
    private int correctlyOrientedShapes = 0;
    private int totalOrientations = 0;

    List<GameObject> roomsToSpawnStatuesIn = new List<GameObject>();
    List<GameObject> roomsToSpawnHintsIn = new List<GameObject>();
    List<GameObject> roomsToSpawnEnemiesIn = new List<GameObject>();

    protected override void InitRoom()
    {
        Random.InitState(System.DateTime.Now.Millisecond);

        for (int i = 0; i < puzzleObjects.Count; i++)
        {
            int index = Random.Range(1, roomsWeCanWorkWith.Count);
            if (roomsToSpawnStatuesIn.Contains(roomsWeCanWorkWith[index]))
            {
                i--;
                continue;
            }
            roomsToSpawnStatuesIn.Add(roomsWeCanWorkWith[index]);
        }

        for (int i = 0; i < hintObjects.Count; i++)
        {
            int index = Random.Range(1, roomsWeCanWorkWith.Count);
            if (roomsToSpawnStatuesIn.Contains(roomsWeCanWorkWith[index]) || roomsToSpawnHintsIn.Contains(roomsWeCanWorkWith[index]))
            {
                i--;
                continue;
            }
            roomsToSpawnHintsIn.Add(roomsWeCanWorkWith[index]);
        }

        for(int i = 0; i < puzzleEnemies.Count; i++)
        {
            int maxClamp = Mathf.Min(5, roomsWeCanWorkWith.Count);
            int index = Random.Range(1, roomsWeCanWorkWith.Count-maxClamp);
            if (roomsToSpawnEnemiesIn.Contains(roomsWeCanWorkWith[index]))
            {
                i--;
                continue;
            }
            roomsToSpawnEnemiesIn.Add(roomsWeCanWorkWith[index]);
        }
    }

    protected override void SetupPuzzle()
    {
        ////****************************** RANDOMIZE THE ORIENTATION OF THE STATUES ******************************//

        List<GameObject> statues = new List<GameObject>();

        for (int i = 0; i < roomsToSpawnStatuesIn.Count; i++)
        {
            GameObject instance = Instantiate(puzzleObjects[i], roomsToSpawnStatuesIn[i].transform);
            instance.GetComponentInChildren<OrientShape>().transform.rotation = GetRotation(instance.transform.localPosition);

            statues.Add(instance);

            OrientShape script = instance.GetComponentInChildren<OrientShape>();
            script.OnCorrectOrientation += CorrectlyOrientedAShape;
            script.OnIncorrectOrientation += ShapeIncorrectlyOriented;
        }

        for (int i = 0; i < roomsToSpawnHintsIn.Count; i++)
        {
            GameObject instance = Instantiate(hintObjects[i], roomsToSpawnHintsIn[i].transform);

            int yRot = statues[i].GetComponentInChildren<OrientShape>().GetCorrectRotation();

            instance.transform.rotation = Quaternion.Euler(0, yRot + 90, 0);
            Debug.Log($"{statues[i]}'s correct rotation is {yRot} so we set {instance.name}'s rotaiton to {instance.transform.eulerAngles.y}");
        }

        for(int i = 0; i < roomsToSpawnEnemiesIn.Count; i++)
        {
            Instantiate(puzzleEnemies[i], roomsToSpawnEnemiesIn[i].transform);
        }

    }

    private Vector3 GetPointInRoom(int i)
    {
        Collider roomCollider = GetComponentInChildren<Collider>();

        Bounds roomBound = roomCollider.bounds;
        Vector2 roomSize = new Vector2(roomBound.size.x, roomBound.size.z);

        Renderer r = puzzleObjects[i].GetComponentInChildren<Renderer>();
        float inset = Mathf.Max(r.bounds.size.x, r.bounds.size.z);

        Vector3 pos;

        int columns = Mathf.CeilToInt(Mathf.Sqrt(puzzleObjects.Count));
        int rows = Mathf.CeilToInt(puzzleObjects.Count / (float)columns);

        float sectionWidth = roomSize.x / columns;
        float sectionHeight = roomSize.y / rows;

        int col = i % columns;
        int row = i / columns;

        float xPos = roomBound.min.x + inset + (col * sectionWidth) + Random.Range(0, sectionWidth);
        float yPos = roomBound.min.z + inset + (row * sectionHeight) + Random.Range(0, sectionHeight);

        pos = new Vector3(xPos, transform.position.y, yPos);
        

        return pos;
    }

    private Quaternion GetRotation(Vector3 position)
    {

        //Vector3 directionToCenter = (Vector3.zero - position).normalized;
        //Quaternion lookRotation = Quaternion.LookRotation(directionToCenter, Vector3.up);

        //Vector3 eulerAngle = lookRotation.eulerAngles;
        //eulerAngle.y =(int)(Mathf.Round(eulerAngle.y / 10) * 10);

        return Quaternion.Euler(0, (int)(Mathf.Round(Random.Range(0, 361) / 10) * 10), 0);
        //return Quaternion.Euler(0, Random.Range(0, 73) * 5, 0);
    }

    public void CorrectlyOrientedAShape()
    {
        Debug.Log("Correctly Oriented a shape!");
        correctlyOrientedShapes++;
        if(CheckForCompletion())
        {
            isSolved = true;

            Inventory.Instance.PuzzleSolved();
            
            HUDManager.instance.UpdateText("SUCCESS!");

            foreach (var shape in puzzleObjects)
            {
                Destroy(shape.GetComponentInChildren<OrientShape>());
            }
        }
    }

    public void CompletedRoom()
    {
        foreach (var shape in puzzleObjects)
        {
            Destroy(shape.GetComponentInChildren<OrientShape>());
        }
    }

    public void ShapeIncorrectlyOriented()
    {
        Debug.Log("Messed it up!");
        correctlyOrientedShapes--;
    }

    public override bool CheckForCompletion()
    {
        return totalOrientations == correctlyOrientedShapes;
    }
}
