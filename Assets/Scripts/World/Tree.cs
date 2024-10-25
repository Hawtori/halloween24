using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Tree
{
    List<NavMeshLink> allLinks = new List<NavMeshLink>();

    public class Node
    {
        GameObject room;
        public List<Node> children;

        public Node(GameObject rm)
        {
            room = rm;
            children = new List<Node>();
        }

        public void AddChild(Node child)
        {
            children.Add(child);
        }

        public string GetChildrenNames()
        {
            string names = "";
            foreach (Node child in children) { names += child.room.name + ", "; }

            return names.TrimEnd(',', ' ');
        }

        public string GatherChildrenNames()
        {
            if (children.Count == 0) return ".";

            string names = "";
            foreach(Node child in children)
            {
                names += child.GetRoom().name + ", ";
                names += child.GatherChildrenNames() + " ";
            }

            return names.TrimEnd(',', ' ');
        }

        public GameObject GetRoom() => room;

    }

    public List<GameObject> allRooms = new List<GameObject>();
    public List<Node> allRoomNodes = new List<Node>();

    public Node head;

    public void AddRoom(GameObject room)
    {
        allRooms.Add(room);
    }

    public Node FindNode(GameObject room)
    {
        foreach(Node node in allRoomNodes) 
            if(node.GetRoom() == room)
                return node;
        return null;
    }

    public void AddNode(GameObject parent, GameObject child)
    {
        Node parentNode = FindNode(parent);
        Node childNode = FindNode(child);

        if (parentNode == null)
        {
            parentNode = new Node(parent);
            allRoomNodes.Add(parentNode);
        }

        if (childNode == null)
        {
            childNode = new Node(child);
            allRoomNodes.Add(childNode);
        }

        parentNode.AddChild(childNode);
    }


    public void AddNode(Node parent, Node child)
    {
        Node parentNode = FindNode(parent.GetRoom());
        Node childNode = FindNode(child.GetRoom());

        if(parentNode == null)
        {
            parentNode = new Node(parent.GetRoom());
            allRoomNodes.Add(parentNode);
        }

        if(childNode == null)
        {
            childNode= new Node(child.GetRoom());
            allRoomNodes.Add(childNode);
        }

        parentNode.AddChild(childNode);
    }

    public void RemoveNode(GameObject room)
    {
       if(head == null)
        {
            Debug.Log("No head.");
            return;
        }

       RemoveNodeHelp(head, room);
    }

    private bool RemoveNodeHelp(Node currNode, GameObject roomToRemove)
    {
        if(currNode.GetRoom() == roomToRemove)
        {
            currNode.children.Clear();
            return true;
        }

        for(int i = 0; i < currNode.children.Count; i++)
        {
            if (RemoveNodeHelp(currNode.children[i], roomToRemove))
            {
                currNode.children.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    public void PrintRooms(Node node)
    {
        if (node == null) return;

        Debug.Log(node.GetRoom().name + " -> " + node.GatherChildrenNames());

        foreach (Node child in node.children)
        {
            PrintRooms(child);
        }
    }

    public void CreateNavmeshLinks()
    {
        if (head == null)
        {
            Debug.Log("Head was null, no links created");
            return;
        }

        CreateLinksRecursive(head);
    }

    private void CreateLinksRecursive(Node node)
    {
        foreach(Node child in node.children)
        {
            if (child.GetRoom() == null || node.GetRoom() == null) continue;
            CreateLinkBetweenTwo(node.GetRoom(), child.GetRoom());

            CreateLinksRecursive(child);
        }

        foreach(NavMeshLink link in allLinks)
        {
            link.enabled = false;
            link.enabled = true;
        }
    }

    private void CreateLinkBetweenTwo(GameObject parent, GameObject child)
    {
        
        NavMeshLink link = parent.AddComponent<NavMeshLink>();

        Vector3 parentWorld = parent.transform.position;
        Vector3 childWorld = child.transform.position;

        Vector3 dir = (childWorld - parentWorld).normalized;

        float offsetDistance = 5f;

        Vector3 parentPosition = parentWorld + dir * offsetDistance;
        Vector3 childPosition = childWorld - dir * offsetDistance;

        link.startPoint = parent.transform.InverseTransformPoint(parentPosition);
        link.endPoint = parent.transform.InverseTransformPoint(childPosition);

        link.costModifier = 0; 
        link.bidirectional = true; 
        link.width = 2f;
        link.autoUpdate = true;
        
        allLinks.Add(link);
    }
}
