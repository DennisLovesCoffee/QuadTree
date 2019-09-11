using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeVect3 : QuadTreeGO
{
    private readonly QuadTreeVect3[] cells;
    private readonly List<Vector3> storedVerts;

    public QuadTreeVect3(int maxSize, Rect newBounds)
    {
        bounds = newBounds;
        maxObjectCount = maxSize;
        cells = new QuadTreeVect3[4];
        storedVerts = new List<Vector3>(maxSize);
    }

    public void Insert(Vector3 newVert)
    {
        if (cells[0] != null)
        {
            int iCell = GetInsertCell(V3ToV2(newVert));
            if(iCell >= 0)
            {
                cells[iCell].Insert(newVert);
            }
            else
            {
                Debug.Log("Vert is out of rect:" + newVert);
            }
            return;
        }
        
        storedVerts.Add(newVert);
        //Objects exceed the maximum count
        if(storedVerts.Count > maxObjectCount)
        {
            //Split the quad into 4 sections
            if(cells[0] == null)
            {
                float subWidth = (bounds.width / 2f);
                float subHeight = (bounds.height / 2f);
                //NorthEast
                cells[0] = new QuadTreeVect3(maxObjectCount,new Rect(bounds.x + subWidth, bounds.y, subWidth, subHeight));
                //northWest
                cells[1] = new QuadTreeVect3(maxObjectCount,new Rect(bounds.x, bounds.y, subWidth, subHeight));
                //southWest
                cells[2] = new QuadTreeVect3(maxObjectCount,new Rect(bounds.x, bounds.y + subHeight, subWidth, subHeight));
                //southEast
                cells[3] = new QuadTreeVect3(maxObjectCount,new Rect(bounds.x + subWidth, bounds.y + subHeight, subWidth, subHeight));
            }
            
            //Reallocate this quads objects into its children
            for(int i = storedVerts.Count-1; i >= 0; --i)
            {
                Vector2 currentVert = V3ToV2(storedVerts[i]);
                cells[GetInsertCell(currentVert)].Insert(currentVert);
                storedVerts.RemoveAt(i);
            }
        }
        
    }
    
    public void Remove(Vector2 vertex)
    {
        if(ContainsLocation(V3ToV2(vertex)))
        {
            storedVerts.Remove(vertex);
            if(cells[0] != null)
            {
                for(int i=0; i < 4; i++)
                {
                    cells[i].Remove(vertex);
                }
            }
        }
    }
    
    private List<Vector3> RetrieveObjectsInArea(Rect area)
    {
        if(RectOverlap(bounds,area)){
            List<Vector3> returnedObjects = new List<Vector3>();
            
            for(int i=0; i < storedVerts.Count; ++i)
            {
                if(area.Contains((V3ToV2(storedVerts[i]))))
                {
                    returnedObjects.Add(storedVerts[i]);
                }
            }
            
            if(cells[0] != null){
                for(int i = 0; i < 4; i++)
                {
                    List<Vector3> cellObjects = cells[i].RetrieveObjectsInArea(area);
                    if(cellObjects != null)
                    {
                        returnedObjects.AddRange(cellObjects);
                    }
                }
            }
            return returnedObjects;
        }
        return null;
    }
    
    public Vector3 FindClosestVertex(Vector2 position, float maxDistance)
    {
        Rect viewRect = new Rect(position.x - maxDistance/2, position.y - maxDistance/2, maxDistance, maxDistance);
		
        List<Vector3> closestObjects = RetrieveObjectsInArea(viewRect);
        if (closestObjects == null || closestObjects.Count <= 0)
        {
            return Vector3.zero;
        }
		
        Vector3 closestObject = Vector3.zero;
        float closestDist = Mathf.Infinity;

        foreach (Vector3 vert in closestObjects)
        {
            float newDist = Vector2.Distance(V3ToV2(vert), position);
            if (newDist < closestDist)
            {
                closestDist = newDist;
                closestObject = vert;
            }
        }
		
        return closestObject;
    }
    
    private int GetInsertCell(Vector2 location)
    {
        for(int i = 0; i < 4; ++i){
            if(cells[i].ContainsLocation(location))
            {
                return i;
            }
        }
        
        return -1;
    }
    
}

