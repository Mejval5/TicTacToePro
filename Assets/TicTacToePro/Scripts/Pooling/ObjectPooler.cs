using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TicTacToePro.Pooling
{
	[ExecuteAlways]
	public class ObjectPooler : MonoBehaviour
	{
		public PooledObject pooledObject;
		public GameObject objectsHolder;

		public List<GameObject> pool;

		public bool StartSetupGame;
		public bool StartSetupEditor;

		void Start()
		{
			if (Application.isPlaying && StartSetupGame || !Application.isPlaying && StartSetupEditor)
				SetupPool();
		}


#if UNITY_EDITOR
		[Button(nameof(RefreshPools))]
		public bool refreshPool;
		public void RefreshPools()
		{
			StartCoroutine(ResetPool());
		}
#endif

		public IEnumerator ResetPool()
		{
			for (int i = objectsHolder.transform.childCount - 1; i >= 0; i--)
			{
				if (Application.isPlaying)
					Destroy(objectsHolder.transform.GetChild(i).gameObject);
				else
					DestroyImmediate(objectsHolder.transform.GetChild(i).gameObject);
			}

			yield return null;
			yield return null;
			pool = null;
			SetupPool();
		}

		public void SetupPool()
		{
			if (pool == null)
				pool = new();

			PruneOldPool();

			if (pool == null)
				pool = new();
			else if (pool.Count >= pooledObject.amountToPool)
				return;

			for (int i2 = 0; i2 < pooledObject.amountToPool; i2++)
			{
				AddPooledObject(pooledObject.objectToPool);
			}
		}

		void PruneOldPool()
		{
			for (int i = objectsHolder.transform.childCount - 1; i >= 0; i--)
			{
				GameObject child = objectsHolder.transform.GetChild(i).gameObject;
				if (child == null)
					continue;

				bool isInPool = pool.Contains(child);

				if (!isInPool)
				{
					child.SetActive(false);

					if (!Application.isPlaying)
					{
						DestroyImmediate(child);
					}
					else
						Destroy(child);
				}
			}

			for (int i = pool.Count - 1; i >= 0; i--)
			{
				if (pool[i] == null)
					pool.RemoveAt(i);
			}

		}

		GameObject AddPooledObject(Object newObject)
		{
			GameObject pooledObject = null;
			if (Application.isPlaying)
				pooledObject = Instantiate(newObject, objectsHolder.transform.position, Quaternion.identity, objectsHolder.transform) as GameObject;
			else
			{
#if UNITY_EDITOR
				pooledObject = ((GameObject)PrefabUtility.InstantiatePrefab(newObject, objectsHolder.transform));
#endif
			}

			if (pooledObject == null)
				return null;

			pooledObject.transform.position = objectsHolder.transform.position;
			pooledObject.transform.rotation = Quaternion.identity;

			if (objectsHolder.transform != null)
			{
				pooledObject.transform.SetParent(objectsHolder.transform, false);
			}

			pooledObject.SetActive(false);
			pool.Add(pooledObject);
			return pooledObject;
		}

		public void DisableAllPooledObjects()
		{
			foreach (GameObject pooledObject in pool)
			{
				if (pooledObject != null)
					pooledObject.SetActive(false);
			}
		}

		public List<GameObject> GetPooledObjects(GameObject gameObject)
		{
			return pool;
		}

		public GameObject GetPooledObject(PooledObject pooledObject)
		{
			if (pooledObject == null)
			{
				Debug.LogError("Trying to get null from Object Pooler: " + name);
				return null;
			}

			GameObject retrievedObject = pooledObject.objectToPool;

			// return a regular non-pooled version if there is no pool for this type
			List<GameObject> allPooledObjects = GetPooledObjects(retrievedObject);

			if (this.pooledObject == null || allPooledObjects == null)
			{
				Debug.Log("no pooled objects");
				return null;
			}

			// return first available inactive pooled object
			for (int i = 0; i < allPooledObjects.Count; i++)
			{
				if (allPooledObjects[i] != null && !allPooledObjects[i].activeSelf)
				{
					return allPooledObjects[i];
				}
			}


			if (this.pooledObject.objectToPool.name == retrievedObject.name)
			{
				if (this.pooledObject.shouldExpand)
				{
					//Debug.Log("Expanding");
					return AddPooledObject(retrievedObject);
				}
				else
				{
					Debug.LogError("Object cannot be pooled. Pool is all used up, and not expanding.");
				}
			}

			return null;
		}
	}
}