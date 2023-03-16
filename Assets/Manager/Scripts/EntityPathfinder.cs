using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EntityPathfinder.LinePathfinding;

/// <summary>
/// Class to compute for each enemy the path to attack the squad lnowing the obstacle son the road.
/// </summary>
public class EntityPathfinder : MonoBehaviour
{
    [Tooltip("The gameObject of the floor where the squad and the ennemy can walk on.")]
    public GameObject road;
    public Transform obstaclesParent;
    List<Collider> colliderObstacles;

    //Settings values of the pathfinding to choose quality of calculation
    //It may cause big slow downs.
    [Tooltip("Size of a tile in number of units")]
    public float definitionPathFinder = 1;
    [Tooltip("Distance maximum between two tiles linked")]
    public int maximumDistancePathBetweenPoint = 4;
    [Tooltip("When searching the path, depth to find tiles linked.")]
    public int maximumPathfindingDepth = 3;
    [Tooltip("Value in units to have a safe radius and avoid enemy to go too close from obstacle.")]
    public float radiusSafe = 0.5f;

    public List<LinePathfinding> pathfindingLines;
    int sizeXMap;
    int sizeZMap;

    Vector3 positionRoadReal;
    float sizeXHalf;
    float sizeZHalf;

    [Serializable]
    public class LinePathfinding
    {
        public float posZ;
        public List<AccessibleLine> lines;
        [Serializable]
        public class AccessibleLine
        {
            public float startX;
            public float endX;

            public List<int> linkedBefore = new List<int>();
            public List<int> linkedAfter = new List<int>();

        }
       

        public LinePathfinding(float z)
        {
            this.posZ = z;
            this.lines = new List<AccessibleLine>();
        }

    }





    public void Awake()
    {
        InitLevelPathfinder();
    }

    /// <summary>
    /// Initialize the pathfinding by creating a map of the size of the road object.
    /// </summary>
    public void InitLevelPathfinder()
    {
        colliderObstacles = new List<Collider>();
        for(int i=0; i< obstaclesParent.childCount; i++)
        {
            colliderObstacles.Add(obstaclesParent.GetChild(i).GetComponent<Collider>());
        }

        pathfindingLines = new List<LinePathfinding>();

        bool first = true;
        int nbTimeSinceLastInteresting = -1;
        int timeSinceLastInteresting = -1;

        int nbDefinitionBetweenLine = 3;

        List<AccessibleLine>[] previousLines = new List<AccessibleLine>[nbDefinitionBetweenLine];

        
        for (float z=-road.transform.localScale.z*0.5f+ definitionPathFinder; z<= road.transform.localScale.z * 0.5f- definitionPathFinder; z+= definitionPathFinder)
        {
            List<AccessibleLine> accessibleLines = new List<AccessibleLine>();

            AccessibleLine currentAccessibleLine = null;

            bool isInterestingZ = false;

            for (float x = -road.transform.localScale.x * 0.5f + definitionPathFinder; x <= road.transform.localScale.x * 0.5f - definitionPathFinder; x += definitionPathFinder)
            {

                bool isAccessible = true;
                foreach (Collider coll in colliderObstacles)
                {
                    if (coll.bounds.Contains(road.transform.position+new Vector3(x, 0, z))) isAccessible = false;
                }

                if(isAccessible)
                {
                    if(currentAccessibleLine==null)
                    {
                        currentAccessibleLine = new AccessibleLine();
                        accessibleLines.Add(currentAccessibleLine);
                        currentAccessibleLine.startX = road.transform.position.x + x;
                        currentAccessibleLine.endX = road.transform.position.x + x;
                    }
                    else
                    {
                        currentAccessibleLine.endX = road.transform.position.x + x;
                    }
                }
                else
                {
                    isInterestingZ = true;
                    if (currentAccessibleLine != null)
                    {
                        currentAccessibleLine = null;
                    }
                }

            }



            if (timeSinceLastInteresting >= 0) timeSinceLastInteresting++;
            if (!isInterestingZ)
            {
                nbTimeSinceLastInteresting++;
            }

            if (isInterestingZ && nbTimeSinceLastInteresting>= nbDefinitionBetweenLine)
            {
                LinePathfinding newLine = new LinePathfinding(road.transform.position.z + z - nbDefinitionBetweenLine*definitionPathFinder);
                newLine.lines = previousLines[0];
                pathfindingLines.Add(newLine);
            }

            if (isInterestingZ || first || timeSinceLastInteresting>= nbDefinitionBetweenLine || z + definitionPathFinder> road.transform.localScale.z * 0.5f - definitionPathFinder)
            {
                LinePathfinding newLine = new LinePathfinding(road.transform.position.z + z);
                newLine.lines = accessibleLines;
                pathfindingLines.Add(newLine);
                timeSinceLastInteresting = -1;
            }

            if(isInterestingZ)
            {
                timeSinceLastInteresting = 0;
                nbTimeSinceLastInteresting = 0;
            }

            first = false;

            for (int i = 0; i < nbDefinitionBetweenLine-1; i++)
            {
                previousLines[i] = previousLines[i + 1];
            }
            previousLines[nbDefinitionBetweenLine - 1] = accessibleLines;
        }

        for(int i = 0; i< pathfindingLines.Count-1; i++)
        {
            LinePathfinding lineBefore = pathfindingLines[i];
            LinePathfinding lineAfter = pathfindingLines[i+1];
            
            for(int j=0; j< lineBefore.lines.Count; j++)
            {
                AccessibleLine linePartBefore = lineBefore.lines[j];
                Vector3 positionLinePartBeforeCenter = new Vector3((linePartBefore.startX + linePartBefore.endX) * 0.5f, 0, lineBefore.posZ);
                for (int k = 0; k < lineAfter.lines.Count; k++)
                {
                    AccessibleLine linePartAfter = lineAfter.lines[k];
                    Vector3 positionLinePartAfterCenter = new Vector3((linePartAfter.startX + linePartAfter.endX) * 0.5f, 0, lineAfter.posZ);

                    int nbTest = (int)(Mathf.Max(4,Vector3.SqrMagnitude(positionLinePartAfterCenter - positionLinePartBeforeCenter) / (definitionPathFinder * definitionPathFinder)));
                    float invertNbTest = 1 / ((float)nbTest);

                    bool isLinked = true;

                    for(float t= invertNbTest; t<=1- invertNbTest && isLinked; t+= invertNbTest)
                    {
                        foreach (Collider coll in colliderObstacles)
                        {
                            if (coll.bounds.Contains(t * positionLinePartAfterCenter+ (1-t) * positionLinePartBeforeCenter)) isLinked = false;
                        }
                    }
                    if(isLinked)
                    {
                        linePartBefore.linkedAfter.Add(k);
                        linePartAfter.linkedBefore.Add(j);
                    }
                }
            }
        }


        positionRoadReal = road.transform.position;
        sizeXHalf = road.transform.localScale.x * 0.5f;
        sizeZHalf = road.transform.localScale.z * 0.5f;

    }

    /// <summary>
    /// From the current position and a position target, find the next position to go to get closer from the target.
    /// </summary>
    public Vector3 GetPositionToGoPathfinding(Vector3 currentPosition, Vector3 positionTarget, float radius, float randomValue)
    {
        int idPositionCurrent = 0;
        int idPositionTarget = 0;

        foreach(LinePathfinding linePathfinding in pathfindingLines)
        {
            if(currentPosition.z>=linePathfinding.posZ)
            {
                idPositionCurrent++;
            }
            if (positionTarget.z >= linePathfinding.posZ)
            {
                idPositionTarget++;
            }
        }
        idPositionCurrent--;
        idPositionTarget--;
        if (idPositionCurrent>= pathfindingLines.Count-1)
        {
            idPositionCurrent = pathfindingLines.Count - 2;
        }
        if (idPositionTarget >= pathfindingLines.Count-1)
        {
            idPositionTarget = pathfindingLines.Count - 2;
        }

        int[] currentAccessibleLine = new int[2];
        int[] targetAccessibleLine = new int[2];
        (currentAccessibleLine[0], currentAccessibleLine[1], targetAccessibleLine[0], targetAccessibleLine[1]) = GetCurrentAccesibleLines(idPositionCurrent, currentPosition.z, currentPosition.x, radius, idPositionTarget, positionTarget.z, positionTarget.x);


        Vector3 positionToGo = currentPosition;

        

        if (idPositionCurrent == idPositionTarget)
        {
            if(currentAccessibleLine[0] == targetAccessibleLine[0] && currentAccessibleLine[1]== targetAccessibleLine[1])
            {
                positionToGo = positionTarget;
                positionToGo.y = currentPosition.y;
                return positionToGo;
            }
            
        }
        int moveDirection = 0;

        if (currentPosition.z < positionTarget.z)
        {
            moveDirection = 0;
        }
        else
        {
            moveDirection = 1;
        }

        LinePathfinding lineBefore = pathfindingLines[idPositionCurrent];
        LinePathfinding lineAfter = pathfindingLines[idPositionCurrent + 1];
        float ratioOnAccessibleLineZ = (currentPosition.z - lineBefore.posZ) / (lineAfter.posZ - lineBefore.posZ);



        AccessibleLine linePartBefore = lineBefore.lines[currentAccessibleLine[0]];
        AccessibleLine linePartAfter = lineAfter.lines[currentAccessibleLine[1]];

        float startX = (1 - ratioOnAccessibleLineZ) * linePartBefore.startX + ratioOnAccessibleLineZ * linePartAfter.startX;
        float endX = (1 - ratioOnAccessibleLineZ) * linePartBefore.endX + ratioOnAccessibleLineZ * linePartAfter.endX;

        float positionRatio = (currentPosition.x - startX) / (endX - startX);


        if (moveDirection == 0)
        {
            float positionX = Mathf.Clamp(positionTarget.x, linePartAfter.startX + radius, linePartAfter.endX - radius);
            positionToGo = new Vector3(1f * positionX + 0f * (linePartAfter.startX + positionRatio * (linePartAfter.endX - linePartAfter.startX)), 0, lineAfter.posZ + definitionPathFinder * 0.5f);
        }
        else if (moveDirection == 1)
        {
            

            float positionX = Mathf.Clamp(positionTarget.x, linePartBefore.startX + radius, linePartBefore.endX - radius);
            positionToGo = new Vector3(1f * positionX + 0f * (linePartBefore.startX + positionRatio * (linePartBefore.endX - linePartBefore.startX)), 0, lineBefore.posZ - definitionPathFinder * 0.5f);

            
        }

        positionToGo.y = currentPosition.y;
        return positionToGo;

    }

    public (int, int, int, int) GetCurrentAccesibleLines(int idPathfindingLineCurrent, float posZCurrent, float posXCurrent, float radius, int idPathfindingLineTarget, float posZTarget, float posXTarget)
    {
        int lineBeforeID = 0;
        int lineAfterID = 0;


        if(idPathfindingLineCurrent == idPathfindingLineTarget)
        {
            LinePathfinding lineBefore = pathfindingLines[idPathfindingLineCurrent];
            LinePathfinding lineAfter = pathfindingLines[idPathfindingLineCurrent + 1];
            float ratioOnAccessibleLineZCurrent = (posZCurrent - lineBefore.posZ) / (lineAfter.posZ - lineBefore.posZ);
            float ratioOnAccessibleLineZTarget = (posZTarget - lineBefore.posZ) / (lineAfter.posZ - lineBefore.posZ);

            float minimumPosition = 0;

            for (int i = 0; i < lineBefore.lines.Count; i++)
            {
                AccessibleLine linePartBefore = lineBefore.lines[i];
                for (int j = 0; j < linePartBefore.linkedAfter.Count; j++)
                {
                    AccessibleLine linePartAfter = lineAfter.lines[linePartBefore.linkedAfter[j]];
                    float startXCurrent = (1 - ratioOnAccessibleLineZCurrent) * linePartBefore.startX + ratioOnAccessibleLineZCurrent * linePartAfter.startX;
                    float endXCurrent = (1 - ratioOnAccessibleLineZCurrent) * linePartBefore.endX + ratioOnAccessibleLineZCurrent * linePartAfter.endX;

                    float startXTarget = (1 - ratioOnAccessibleLineZTarget) * linePartBefore.startX + ratioOnAccessibleLineZTarget * linePartAfter.startX;
                    float endXTarget = (1 - ratioOnAccessibleLineZTarget) * linePartBefore.endX + ratioOnAccessibleLineZTarget * linePartAfter.endX;


                    if (posXCurrent - startXCurrent > radius && endXCurrent - posXCurrent > radius)
                    {
                        if (posXTarget - startXTarget > radius && endXTarget - posXTarget > radius) return (i, linePartBefore.linkedAfter[j], i, linePartBefore.linkedAfter[j]);

                        float ratioPosition = (posXCurrent - startXCurrent) / (endXCurrent - startXCurrent);
                        if(Mathf.Abs(ratioPosition - 0.5f) > minimumPosition)
                        {
                            minimumPosition = Mathf.Abs(ratioPosition - 0.5f);
                            lineBeforeID = i;
                            lineAfterID = linePartBefore.linkedAfter[j];
                        }
                    }
                }
            }
        }
        else if (idPathfindingLineCurrent<idPathfindingLineTarget)
        {
            LinePathfinding lineBefore = pathfindingLines[idPathfindingLineCurrent];
            LinePathfinding lineAfter = pathfindingLines[idPathfindingLineCurrent + 1];
            
            float ratioOnAccessibleLineZCurrent = (posZCurrent - lineBefore.posZ) / (lineAfter.posZ - lineBefore.posZ);

            float minimumPosition = 100000;

            for (int i = 0; i < lineBefore.lines.Count; i++)
            {
                AccessibleLine linePartBefore = lineBefore.lines[i];
                for (int j = 0; j < linePartBefore.linkedAfter.Count; j++)
                {
                    AccessibleLine linePartAfter = lineAfter.lines[linePartBefore.linkedAfter[j]];
                    float startXCurrent = (1 - ratioOnAccessibleLineZCurrent) * linePartBefore.startX + ratioOnAccessibleLineZCurrent * linePartAfter.startX;
                    float endXCurrent = (1 - ratioOnAccessibleLineZCurrent) * linePartBefore.endX + ratioOnAccessibleLineZCurrent * linePartAfter.endX;

                    

                    if (posXCurrent - startXCurrent > radius && endXCurrent - posXCurrent > radius)
                    {
                        float positionX = Mathf.Clamp(posXTarget, linePartAfter.startX + radius, linePartAfter.endX - radius);
                        if (linePartBefore.startX + radius <= linePartBefore.endX - radius)
                        {

                            float distance = Vector3.SqrMagnitude(new Vector3(positionX - posXTarget, 0, lineAfter.posZ - posZTarget));
                            if (distance < minimumPosition)
                            {
                                minimumPosition = distance;
                                lineBeforeID = i;
                                lineAfterID = linePartBefore.linkedAfter[j];
                            }
                        }
                    }
                }
            }
        }
        else if (idPathfindingLineCurrent >idPathfindingLineTarget)
        {
            LinePathfinding lineBefore = pathfindingLines[idPathfindingLineCurrent];
            LinePathfinding lineAfter = pathfindingLines[idPathfindingLineCurrent + 1];

            float ratioOnAccessibleLineZCurrent = (posZCurrent - lineBefore.posZ) / (lineAfter.posZ - lineBefore.posZ);

            float minimumPosition = 100000;

            for (int i = 0; i < lineBefore.lines.Count; i++)
            {
                AccessibleLine linePartBefore = lineBefore.lines[i];
                for (int j = 0; j < linePartBefore.linkedAfter.Count; j++)
                {
                    AccessibleLine linePartAfter = lineAfter.lines[linePartBefore.linkedAfter[j]];
                    float startXCurrent = (1 - ratioOnAccessibleLineZCurrent) * linePartBefore.startX + ratioOnAccessibleLineZCurrent * linePartAfter.startX;
                    float endXCurrent = (1 - ratioOnAccessibleLineZCurrent) * linePartBefore.endX + ratioOnAccessibleLineZCurrent * linePartAfter.endX;



                    if (posXCurrent - startXCurrent > radius && endXCurrent - posXCurrent > radius)
                    {
                        float positionX = Mathf.Clamp(posXTarget, linePartBefore.startX + radius, linePartBefore.endX - radius);

                        if(linePartBefore.startX + radius <= linePartBefore.endX - radius)
                        {
                            float distance = Vector3.SqrMagnitude(new Vector3(positionX - posXTarget, 0, lineBefore.posZ - posZTarget));
                            if (distance < minimumPosition)
                            {
                                minimumPosition = distance;
                                lineBeforeID = i;
                                lineAfterID = linePartBefore.linkedAfter[j];
                            }
                        }
                        
                    }
                }
            }
        }
        else
        {
            LinePathfinding lineBefore = pathfindingLines[idPathfindingLineCurrent];
            LinePathfinding lineAfter = pathfindingLines[idPathfindingLineCurrent + 1];
            float ratioOnAccessibleLineZCurrent = (posZCurrent - lineBefore.posZ) / (lineAfter.posZ - lineBefore.posZ);
            

            float minimumDistance = 100000;

            for (int i = 0; i < lineBefore.lines.Count; i++)
            {
                AccessibleLine linePartBefore = lineBefore.lines[i];
                for (int j = 0; j < linePartBefore.linkedAfter.Count; j++)
                {
                    AccessibleLine linePartAfter = lineAfter.lines[linePartBefore.linkedAfter[j]];
                    float startXCurrent = (1 - ratioOnAccessibleLineZCurrent) * linePartBefore.startX + ratioOnAccessibleLineZCurrent * linePartAfter.startX;
                    float endXCurrent = (1 - ratioOnAccessibleLineZCurrent) * linePartBefore.endX + ratioOnAccessibleLineZCurrent * linePartAfter.endX;

                    if (posXCurrent - startXCurrent > radius && endXCurrent - posXCurrent > radius)
                    {
                        float ratioPositionX = (posXCurrent - startXCurrent) / (endXCurrent - startXCurrent);

                        if (posZCurrent> posZTarget)
                        {
                            float positionXBefore = ratioPositionX * (linePartBefore.endX - linePartBefore.startX) + linePartBefore.startX;
                            float distance = Vector3.SqrMagnitude(new Vector3(positionXBefore - posXTarget, 0, lineBefore.posZ - posZTarget));
                            if(distance< minimumDistance)
                            { 
                                minimumDistance = distance;
                                lineBeforeID = i;
                                lineAfterID = linePartBefore.linkedAfter[j];
                            }
                        }
                        else
                        {
                            float positionXAfter = ratioPositionX * (linePartAfter.endX - linePartAfter.startX) + linePartAfter.startX;
                            float distance = Vector3.SqrMagnitude(new Vector3(positionXAfter - posXTarget, 0, lineAfter.posZ - posZTarget));
                            if (distance < minimumDistance)
                            {
                                minimumDistance = distance;
                                lineBeforeID = i;
                                lineAfterID = linePartBefore.linkedAfter[j];
                            }
                        }
                    }
                }
            }
        }
        


        return (lineBeforeID, lineAfterID, 1, 1);
    }




    /// <summary>
    /// Clamp a world position to stay inside the pathfinding map
    /// </summary>
    public Vector3 ClampPosition(Vector3 positon)
    {
        Vector3 positionClamped = positon;

        if (positionClamped.x < -sizeXHalf + positionRoadReal.x+ radiusSafe) positionClamped.x = -sizeXHalf + positionRoadReal.x+ radiusSafe;
        else if (positionClamped.x > sizeXHalf + positionRoadReal.x- radiusSafe) positionClamped.x = sizeXHalf + positionRoadReal.x- radiusSafe;
        if (positionClamped.z < -sizeZHalf + positionRoadReal.z+ radiusSafe) positionClamped.z = -sizeZHalf + positionRoadReal.z+ radiusSafe;
        else if (positionClamped.z > sizeZHalf + positionRoadReal.z- radiusSafe) positionClamped.z = sizeZHalf + positionRoadReal.z- radiusSafe;

        return positionClamped;
    }

    /// <summary>
    /// Return the position for the fake PvP squad to spawned.
    /// </summary>
    public Vector3 GetPositonStartSquad(Vector3 positionSquadPlayer, float radiusSquad, float minDistance)
    {
        Vector3 positionToSpawn = positionSquadPlayer + minDistance * Vector3.forward;

        if (positionSquadPlayer.z + minDistance > sizeZHalf + positionRoadReal.z - radiusSafe - radiusSquad) positionToSpawn = -positionSquadPlayer + minDistance * Vector3.forward;
        else if (positionSquadPlayer.z - minDistance > -sizeZHalf + positionRoadReal.z + radiusSafe + radiusSquad)
        {
            if(UnityEngine.Random.Range(0,1f)>0.5f) positionToSpawn = -positionSquadPlayer + minDistance * Vector3.forward;
        }



        return ClampPosition(positionToSpawn);
    }
}
