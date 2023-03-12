using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to compute for each enemy the path to attack the squad lnowing the obstacle son the road.
/// </summary>
public class EnemyPathfinder : MonoBehaviour
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

    TilePathfinding[,] mapPosition;
    int sizeXMap;
    int sizeZMap;

    Vector3 positionRoadReal;
    float sizeXHalf;
    float sizeZHalf;

    /// <summary>
    /// Class for each part of the map keeping information if it is a part with or without obstacle
    /// and saving information on path if already calculated.
    /// </summary>
    public class TilePathfinding
    {
        public int posX;
        public int posZ;
        public bool isAccessible;
        public bool isInit = false;

        public List<TilePathfinding> tilePathFindingLinked;
        public List<float> tilePathFindingLinkedDistance;
        public List<TilePathfinding> firstPathFindingTile;

        public TilePathfinding(int x, int z, bool isAccessible)
        {
            this.posX = x;
            this.posZ = z;
            this.isAccessible = isAccessible;
            this.tilePathFindingLinked = new List<TilePathfinding>();
            this.tilePathFindingLinkedDistance = new List<float>();
            this.firstPathFindingTile = new List<TilePathfinding>();
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

        sizeXMap = (int)((road.transform.localScale.x+2) / definitionPathFinder);
        sizeZMap = (int)((road.transform.localScale.z+2) / definitionPathFinder);
        mapPosition = new TilePathfinding[sizeXMap, sizeZMap];

        positionRoadReal = road.transform.position;
        sizeXHalf = road.transform.localScale.x * 0.5f;
        sizeZHalf = road.transform.localScale.z * 0.5f;

    }

    /// <summary>
    /// From the current position and a position target, find the next position to go to get closer from the target.
    /// </summary>
    public Vector3 GetPositionToGoPathfinding(Vector3 currentPosition, Vector3 positionTarget, float randomValue)
    {
        //Convert the current position in tile position.
        int[] currentPositionTileInt = ConvertWorldPositionToTile(currentPosition);

        //Generate the tiles around this position.
        GeneratePathFinderUsefulPoint(currentPositionTileInt[0], currentPositionTileInt[1]);

        //Convert the current position in tile position.
        int[] targetPositionTile = ConvertWorldPositionToTile(positionTarget);
        //Generate the tiles around this position.
        GeneratePathFinderUsefulPoint(targetPositionTile[0], targetPositionTile[1]);


        List<TilePathfinding> tilePathfindingReachable = new List<TilePathfinding>();
        List<float> tilePathfindingDistance = new List<float>();
        List<TilePathfinding> tilePathfindingReachableFirst = new List<TilePathfinding>();


        TilePathfinding currentPositionTile = mapPosition[currentPositionTileInt[0], currentPositionTileInt[1]];
        //From the current position tile, get all the tiles reachable from it.
        FillPossibilitiesLinked(currentPositionTile, maximumPathfindingDepth, 0, null, ref tilePathfindingReachable, ref tilePathfindingDistance, ref tilePathfindingReachableFirst);

        float smallestDistance = 1000000;
        TilePathfinding tileToGo = null;
        //Loop on each tile recahable from the current tile and check the ones that has the shortest path to the target.
        for (int i=0; i< tilePathfindingReachable.Count; i++)
        {
            //We had a random to the distance so path can be different between two enemies of the same position and makes it more random.
            float distanceToTarget = UnityEngine.Random.Range(0f, 1f) + tilePathfindingDistance[i]* tilePathfindingDistance[i] + Vector3.SqrMagnitude(ConvertTileToWorldPosition(tilePathfindingReachable[i]) - positionTarget);
            if(distanceToTarget< smallestDistance)
            {
                smallestDistance = distanceToTarget;
                tileToGo = tilePathfindingReachableFirst[i];
                
            }
        }
        Vector3 positionToGo;
        if (tileToGo!=null)
        {
            //If a tile has been found, convert it to a world position.
            positionToGo = ConvertTileToWorldPosition(tileToGo);
            //We add more random using the random value, so each enemy on the same tile will have different position.
            positionToGo += new Vector3((0.5f + (Mathf.Abs(randomValue) * 0.5f)) * definitionPathFinder, 0, (0.5f + (Mathf.Abs(randomValue) * 0.5f)) * definitionPathFinder);

        }
        else
        {
            //If no tile has been found, just go to the squad
            positionToGo = positionTarget;
        }

        
        positionToGo.y = currentPosition.y;
        return positionToGo;
    }

    /// <summary>
    /// Starting from a tile of the map, check all the tiles reachable with a depth of levelExplo
    /// </summary>
    public void FillPossibilitiesLinked(TilePathfinding currentPositionTile, int levelExplo, float distance, TilePathfinding tileFirst, ref List<TilePathfinding> tilePathfindingReachable, ref List<float> tilePathfindingDistance, ref List<TilePathfinding> tilePathfindingReachableFirst)
    {
        for (int i = 0; i < currentPositionTile.tilePathFindingLinked.Count; i++)
        {
            bool shouldAdd = true;
            for(int j=0; j< tilePathfindingReachable.Count; j++)
            {
                if(tilePathfindingReachable[j] == currentPositionTile.tilePathFindingLinked[i])
                {
                    float distanceNew = distance + tilePathfindingDistance[j];
                    if(distanceNew< tilePathfindingDistance[j])
                    {
                        tilePathfindingDistance[j] = distanceNew;
                        tilePathfindingReachableFirst[j] = tileFirst;
                        shouldAdd = false;
                    }
                }
            }
            if(shouldAdd)
            {
                tilePathfindingReachable.Add(currentPositionTile.tilePathFindingLinked[i]);
                tilePathfindingReachableFirst.Add(currentPositionTile.firstPathFindingTile[i]);
                tilePathfindingDistance.Add(currentPositionTile.tilePathFindingLinkedDistance[i]);
            }

            if (levelExplo > 1)
            {
                FillPossibilitiesLinked(currentPositionTile.tilePathFindingLinked[i], levelExplo-1, currentPositionTile.tilePathFindingLinkedDistance[i], currentPositionTile.firstPathFindingTile[i], ref tilePathfindingReachable, ref tilePathfindingDistance, ref tilePathfindingReachableFirst);
            }
        }
    }

    /// <summary>
    /// Method to create and initialize the tiles around the position posX, posZ
    /// It will look all around the position and compute what tiles are reachable and what distance to reach them.
    /// </summary>
    public void GeneratePathFinderUsefulPoint(int posX, int posZ)
    {
        if (mapPosition[posX, posZ] != null && mapPosition[posX, posZ].isInit) return;

        for (int x = Mathf.Max(0, posX - maximumDistancePathBetweenPoint); x <= Mathf.Min(sizeXMap - 1, posX + maximumDistancePathBetweenPoint); x++)
        {
            for (int z = Mathf.Max(0, posZ - maximumDistancePathBetweenPoint); z <= Mathf.Min(sizeZMap - 1, posZ + maximumDistancePathBetweenPoint); z++)
            {
                if (mapPosition[x, z] == null)
                {
                    Vector3 positionTile = ConvertTileToWorldPosition(x ,z);
                    bool isAccessible = true;
                    foreach (Collider coll in colliderObstacles)
                    {
                        BoxCollider boxCollider = coll.GetComponent<BoxCollider>();

                        if (coll.bounds.Contains(positionTile)) isAccessible = false;
                        
                    }
                    TilePathfinding newTile = new TilePathfinding(x, z, isAccessible);
                    mapPosition[x, z] = newTile;
                   
                }
            }
        }

        for (int x = Mathf.Max(0, posX - maximumDistancePathBetweenPoint); x <= Mathf.Min(sizeXMap - 1, posX + maximumDistancePathBetweenPoint); x++)
        {
            for (int z = Mathf.Max(0, posZ - maximumDistancePathBetweenPoint); z <= Mathf.Min(sizeZMap - 1, posZ + maximumDistancePathBetweenPoint); z++)
            {
                TilePathfinding currentTile = mapPosition[x, z];

                if (currentTile != null && currentTile.isAccessible && !currentTile.isInit)
                {
                    for (int i=0; i<4; i++)
                    {
                        TilePathfinding tileNeighbor = null;
                        
                        if(i==0 && x>0) tileNeighbor = mapPosition[x - 1, z];
                        else if(i==1 && x< sizeXMap - 1) tileNeighbor = mapPosition[x + 1, z];
                        else if(i==2 && z>0) tileNeighbor = mapPosition[x, z-1];
                        else if(i==3 && z< sizeZMap - 1) tileNeighbor = mapPosition[x, z+1];

                        if (tileNeighbor != null && tileNeighbor.isAccessible)
                        {

                            bool shouldAddTileNeighbor = true;
                            for (int j2 = 0; j2 < currentTile.tilePathFindingLinked.Count && shouldAddTileNeighbor; j2++)
                            {
                                TilePathfinding tileLinkedCurrent = currentTile.tilePathFindingLinked[j2];
                                if (tileLinkedCurrent == tileNeighbor)
                                {
                                    shouldAddTileNeighbor = false;
                                    currentTile.tilePathFindingLinkedDistance[j2] = 1;
                                    currentTile.firstPathFindingTile[j2] = tileNeighbor;
                                }

                            }
                            if (shouldAddTileNeighbor)
                            {
                                currentTile.tilePathFindingLinked.Add(tileNeighbor);
                                currentTile.firstPathFindingTile.Add(tileNeighbor);
                                currentTile.tilePathFindingLinkedDistance.Add(1);
                            }



                            for (int j=0; j<tileNeighbor.tilePathFindingLinked.Count; j++)
                            {
                                
                                TilePathfinding tileLinked = tileNeighbor.tilePathFindingLinked[j];
                                float distance = tileNeighbor.tilePathFindingLinkedDistance[j];

                                if(distance<maximumDistancePathBetweenPoint)
                                {
                                    bool shouldAddTileLinked = true;

                                    for (int j2 = 0; j2 < currentTile.tilePathFindingLinked.Count && shouldAddTileLinked; j2++)
                                    {
                                        TilePathfinding tileLinkedCurrent = currentTile.tilePathFindingLinked[j2];
                                        if (tileLinkedCurrent == tileLinked)
                                        {
                                            shouldAddTileLinked = false;
                                            float distanceCurrent = currentTile.tilePathFindingLinkedDistance[j2];

                                            if (1+distance < distanceCurrent)
                                            {
                                                currentTile.tilePathFindingLinkedDistance[j2] = 1+distance;
                                                currentTile.firstPathFindingTile[j2] = tileNeighbor.firstPathFindingTile[j];
                                            }
                                        }

                                    }
                                    if (shouldAddTileLinked)
                                    {
                                        currentTile.tilePathFindingLinked.Add(tileLinked);
                                        currentTile.firstPathFindingTile.Add(tileNeighbor);
                                        currentTile.tilePathFindingLinkedDistance.Add(1 + distance);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        TilePathfinding currentTileNow = mapPosition[posX, posZ];
        currentTileNow.isInit = true;

        for(int i=0; i< currentTileNow.tilePathFindingLinked.Count; i++)
        {
            TilePathfinding linkedTile = currentTileNow.tilePathFindingLinked[i];
            


        }

    }

    /// <summary>
    /// Convert a tile position in wolrd position.
    /// </summary>
    public Vector3 ConvertTileToWorldPosition(TilePathfinding tile)
    {
        return ConvertTileToWorldPosition(tile.posX, tile.posZ);
    }

    /// <summary>
    /// Convert a tile position (x, y) in wolrd position.
    /// </summary>
    public Vector3 ConvertTileToWorldPosition(int posX, int posZ)
    {
        return new Vector3(posX * definitionPathFinder - sizeXHalf + positionRoadReal.x, 0, posZ * definitionPathFinder - sizeZHalf + positionRoadReal.z);
    }

    /// <summary>
    /// Convert a world position in tile position (x,y)
    /// </summary>
    public int[] ConvertWorldPositionToTile(Vector3 worldPosition)
    {
        int[] tilePosition = new int[2];

        tilePosition[0] = (int)(((worldPosition.x - positionRoadReal.x) + sizeXHalf) / definitionPathFinder);
        tilePosition[1] = (int)(((worldPosition.z - positionRoadReal.z) + sizeZHalf) / definitionPathFinder);

        return tilePosition;
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
}
