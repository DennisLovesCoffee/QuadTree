using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeGO
{
	protected int maxObjectCount;
	private readonly List<GameObject> storedObjects;
	protected Rect bounds;
	private readonly QuadTreeGO[] cells;

	//max size is the amount of objects in a list
	//bounds is the Rect of you map or the object in which you place the game objects
	public QuadTreeGO(int maxSize, Rect newBounds){
		bounds = newBounds;
		maxObjectCount = maxSize;
		cells = new QuadTreeGO[4];
		storedObjects = new List<GameObject>(maxSize);
	}

	protected QuadTreeGO()
	{ }
	
	public void Insert(GameObject objectToInsert)
	{
		if(cells[0] != null)
		{
			int iCell = GetInsertCell(Get2DPosition(objectToInsert));
			if(iCell >= 0)
			{
				cells[iCell].Insert(objectToInsert);
			}
			else
			{
				Debug.Log("gameObject is out of the insert rect" + Get2DPosition(objectToInsert) + objectToInsert);
			}
			return;
		}
		storedObjects.Add(objectToInsert);
		//Objects exceed the maximum count
		if(storedObjects.Count > maxObjectCount)
		{
			//Split the quad into 4 sections
			if(cells[0] == null)
			{
				float subWidth = (bounds.width / 2f);
				float subHeight = (bounds.height / 2f);
				
				//NorthEast
				cells[0] = new QuadTreeGO(maxObjectCount,new Rect(bounds.x + subWidth, bounds.y, subWidth, subHeight));
				//northWest
				cells[1] = new QuadTreeGO(maxObjectCount,new Rect(bounds.x, bounds.y, subWidth, subHeight));
				//southWest
				cells[2] = new QuadTreeGO(maxObjectCount,new Rect(bounds.x, bounds.y + subHeight, subWidth, subHeight));
				//southEast
				cells[3] = new QuadTreeGO(maxObjectCount,new Rect(bounds.x + subWidth, bounds.y + subHeight, subWidth, subHeight));
			}
			//Reallocate this quads objects into its children
			for(int i = storedObjects.Count-1; i >= 0; --i)
			{
				GameObject storedObj = storedObjects[i];
				cells[GetInsertCell(Get2DPosition(storedObj))].Insert(storedObj);
				storedObjects.RemoveAt(i);
			}
		}
	}
	public void Remove(GameObject objectToRemove)
	{
		if(ContainsLocation(Get2DPosition(objectToRemove)))
		{
			storedObjects.Remove(objectToRemove);
			if(cells[0] != null)
			{
				for(int i=0; i < 4; i++)
				{
					cells[i].Remove(objectToRemove);
				}
			}
		}
	}
	private List<GameObject> RetrieveObjectsInArea(Rect area)
	{
		if(RectOverlap(bounds,area)){
			List<GameObject> returnedObjects = new List<GameObject>();
			for(int i=0; i < storedObjects.Count; ++i)
			{
				if(area.Contains(Get2DPosition(storedObjects[i])))
				{
					returnedObjects.Add(storedObjects[i]);
				}
			}
			if(cells[0] != null){
				for(int i = 0; i < 4; i++)
				{
					List<GameObject> cellObjects = cells[i].RetrieveObjectsInArea(area);
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

	public GameObject FindClosestGameObject(Vector2 position, float maxDistance)
	{
		Rect viewRect = new Rect(position.x - maxDistance/2, position.y - maxDistance/2, maxDistance, maxDistance);
		
		List<GameObject> closestObjects = RetrieveObjectsInArea(viewRect);
		if (closestObjects == null || closestObjects.Count <= 0)
		{
			return null;
		}
		
		GameObject closestObject = null;
		float closestDist = Mathf.Infinity;

		foreach (GameObject obj in closestObjects)
		{
			float newDist = Vector2.Distance(Get2DPosition(obj), position);
			if (newDist < closestDist)
			{
				closestDist = newDist;
				closestObject = obj;
			}
		}
		
		return closestObject;
	}

	protected bool ContainsLocation(Vector2 location)
	{
		return bounds.Contains(location);
	}
	
	private int GetInsertCell(Vector2 location)
	{
		for(int i=0; i < 4; i++){
			if(cells[i].ContainsLocation(location))
			{
				return i;
			}
		}
		return -1;
	}
	private static bool ValueInRange(float value, float min, float max)
	{ return (value >= min) && (value <= max); }

	protected static bool RectOverlap(Rect A, Rect B)
	{
	    bool xOverlap = ValueInRange(A.x, B.x, B.x + B.width) ||
	                    ValueInRange(B.x, A.x, A.x + A.width);

	    bool yOverlap = ValueInRange(A.y, B.y, B.y + B.height) ||
	                    ValueInRange(B.y, A.y, A.y + A.height);

	    return xOverlap && yOverlap;
	}

	private static Vector2 Get2DPosition(GameObject obj)
	{
		Vector3 pos = obj.transform.position;
		return V3ToV2(pos);
	}
	
	protected static Vector2 V3ToV2(Vector3 vector3)
	{
		return new Vector2(vector3.x, vector3.z);
	}
}
