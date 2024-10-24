using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeOrientationPuzzle : PuzzleRoom
{
    private int correctlyOrientedShapes = 0;
    private int totalOrientations = 0;


    protected override void SetupPuzzle()
    {
        //****************************** RANDOMIZE THE ORIENTATION OF THE STATUES ******************************//

        totalOrientations = puzzleObjects.Count;
        foreach(var shape in puzzleObjects)
        {
            //GameObject instance = Instantiate(shape, transform);
            //instance.transform.localPosition = GetPointInRoom(index++);
            shape.GetComponentInChildren<OrientShape>().transform.rotation = GetRotation(shape.transform.localPosition);

            OrientShape script = shape.GetComponentInChildren<OrientShape>();
            script.OnCorrectOrientation += CorrectlyOrientedAShape;
            script.OnIncorrectOrientation += ShapeIncorrectlyOriented;
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

        Vector3 directionToCenter = (Vector3.zero - position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToCenter, Vector3.up);

        Vector3 eulerAngle = lookRotation.eulerAngles;
        eulerAngle.y =(int)(Mathf.Round(eulerAngle.y / 5) * 5);

        return Quaternion.Euler(0, eulerAngle.y, 0);
        //return Quaternion.Euler(0, Random.Range(0, 73) * 5, 0);
    }

    public void CorrectlyOrientedAShape()
    {
        Debug.Log("Correctly Oriented a shape!");
        correctlyOrientedShapes++;
        if(CheckForCompletion())
        {
            Inventory.Instance.PuzzleSolved();
            Debug.Log("Solved the puzzle!");
            foreach(var shape in puzzleObjects)
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
