﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class TestCode : MonoBehaviour
{
    private Transform startPos, endPos;
    public Node startNode { get; set; }
    public Node goalNode { get; set; }
    public List<Node> pathArray;
    public GameObject objStartCube, objEndCube;
    private float elapsedTime = 0.0f;
    //Interval time between pathfinding
    public float intervalTime = 1.0f;

    public List<Node> openList;

    IEnumerator currentPath;

    Vector3 endPostion;
    float distanceBetweenPoints;

    public float movespeed;
    public bool autoCountPath;

    public bool showCalculatedObstacles;

    public float nextWaypointDistance;

    void Start()
    {
        objStartCube = GameObject.FindGameObjectWithTag("Start");
        objEndCube = GameObject.FindGameObjectWithTag("End");
        pathArray = new List<Node>();
        
        FindPath();
    }
    void Update()
    {

        //Count path more often if target is near
        if (autoCountPath)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= intervalTime)
            {
                elapsedTime = 0.0f;
                FindPath();
            }
        }
        else {
            if (Input.GetButtonDown("Jump"))
            {
                FindPath();
            }
        }

    }

    public void FindPath()
    {
        PriorityQueue.closedList.Clear();
        PriorityQueue.openList.Clear();
        startPos = objStartCube.transform;
        endPos = objEndCube.transform;
        //startNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(startPos.position)));
        //goalNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(endPos.position)));
        if (endPos.position != endPostion) {
            endPostion = endPos.position;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            pathArray = AStar.FindPath(startPos.position, endPos.position);
            sw.Stop();
            print("Time took to calculate path: " + sw.ElapsedMilliseconds + "ms");
            //Debug.Log(pathArray[0].position);


            //Debug.Log("New path calculated");
            if (currentPath != null)
            {
                StopCoroutine(currentPath);
            }
            currentPath = movepath(pathArray);
            StartCoroutine(currentPath);
        }


    }


    public IEnumerator movepath(List<Node> pathArray) {
        int security = 0;
        int i = 0;
        if (pathArray == null || pathArray.Count <= 1)
        {
            yield return null;
        }
        for (int j = 0; j < pathArray.Count; j++) {
            if (i == 0 && pathArray.Count > 1)
            {
                i = 1;
            }

            else {
                i = j;
            }
            //if (pathArray.Count > 1)
            //{
            //    print(AStar.GetDistance(pathArray[i] ,pathArray[i+1]));
            //}
            while (/*(objStartCube.transform.position - pathArray[i].position).sqrMagnitude > nextWaypointDistance * nextWaypointDistance*/objStartCube.transform.position != pathArray[i].position) {                
                objStartCube.transform.position =  Vector3.MoveTowards(objStartCube.transform.position, pathArray[i].position, Time.deltaTime* movespeed);
                if (objStartCube.transform.position == pathArray[i].position) {
                    //Debug.Log("Goal reaced");
                }
                yield return null;
                //if (security > 2000) {                    
                //    Debug.Break();
                //    break;
                //}
                security++;
            }
        }

        yield return null;
    }

    void OnDrawGizmos()
    {
        if (pathArray == null)
            return;
        if (pathArray.Count > 0)
        {
            int index = 1;
            foreach (Node node in pathArray)
            {
                if (index < pathArray.Count)
                {
                    Node nextNode = (Node)pathArray[index];
                    UnityEngine.Debug.DrawLine(node.position, nextNode.position,
                    Color.green);
                    index++;
                }
            }
        }
        if (showCalculatedObstacles)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < PriorityQueue.openList.Count; i++)
            {
                Gizmos.DrawCube(PriorityQueue.openList[i], Vector3.one);

            }
            Gizmos.color = Color.red;
            for (int i = 0; i < PriorityQueue.closedList.Count; i++)
            {
                Gizmos.DrawCube(PriorityQueue.closedList[i], Vector3.one * 0.3f);

            }
        }


    }
}