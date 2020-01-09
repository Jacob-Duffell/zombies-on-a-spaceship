using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectPooler : MonoSingleton<ObjectPooler>
{
    /// <summary>
    /// An array of objects that will be used as a template to create the pooled objects
    /// </summary>
    public GameObject[] m_PooledObject;

    /// <summary>
    /// How many of each object will be pooled
    /// </summary>
    public int[] m_PooledAmount;

    /// <summary>
    /// Should the size of the pools increase if all objects have been used
    /// </summary>
    public bool[] m_WillGrow;

    /// <summary>
    /// A list of shelves that will hold the pooled objects when they aren't in use
    /// </summary>
    private List<GameObject> m_ObjectShelves;

    /// <summary>
    /// An array of lists of all the pooled game objects
    /// </summary>
    private List<PooledObject>[] m_PooledObjects;

    protected override bool Awake()
    {
        if (base.Awake())   // run the parents awake function to ensure we are using a singleton pattern
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        return false;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Init();
    }

    void Init()
    {
        m_ObjectShelves = new List<GameObject>();       // initialise the object shelves list
        int pooledObjectCount = m_PooledObject.Length;      // store the amount of different objects that will be pooled
        m_PooledObjects = new List<PooledObject>[pooledObjectCount];    // initialise a list for each array index

        for (int i = 0; i < pooledObjectCount; i++)
        {
            m_PooledObjects[i] = new List<PooledObject>();      // initialise a list for each array index
        }

        if (m_PooledAmount.Length != pooledObjectCount)
        {
            m_PooledAmount = new int[pooledObjectCount];        // ensures the m_PooledAmount array is the same size as the m_PooledObject size
        }

        if (m_WillGrow.Length != pooledObjectCount)
        {
            m_WillGrow = new bool[pooledObjectCount];           // ensures the m_WillGrow array is the same size as the m_PooledObject size
        }

        for (int i = 0; i < pooledObjectCount; i++)         // loop through each object and then do a second loop to create the pooled objects for that type
        {
            string name = m_PooledObject[i].name;       // get the name of the pooled object
            GameObject shelfGO = new GameObject();      // create a new object that will act as a holder for the pooled objects
            shelfGO.name = "Shelf: " + name;            // name the holder
            m_ObjectShelves.Add(shelfGO);               // add it to the shelves list

            if (m_PooledAmount[i] > 0)      // if we need to make some objects
            {
                for (int j = 0; j < m_PooledAmount[i]; j++)     // loop through the amount needed
                {
                    GameObject obj = Instantiate(m_PooledObject[i]);                // create an object
                    PooledObject poolScript = obj.AddComponent<PooledObject>();     // add the pooledObject script to the object
                    poolScript.Init(i);                                             // init the script
                    m_PooledObjects[i].Add(poolScript);                             // add it to the correct list
                    obj.name = name;                                                // name the object
                    obj.SetActive(false);                                           // turn the object off
                    obj.transform.SetParent(m_ObjectShelves[i].transform);          // set the shelf as the parent of the newly created object
                }
            }
        }
    }

    /// <summary>
    /// Returns an object from the object pooler via a name
    /// </summary>
    /// <param name="name">name of the desired object</param>
    public GameObject GetPooledObject(string name)
    {
        int shelfCount = m_ObjectShelves.Count;     // get how many shelves there are so we can loop through the lists
        int currentIndex = -1;                      // this will be used to get the correct list. We set it to -1 as the array index start at 0

        for (int i = 0; i < shelfCount; i++)
        {
            if (m_ObjectShelves[i].name == "Shelf: " + name)        // compare the shelves name to the name of the object that we want. If it matches we set the currentIndex to the loop and break out
            {
                currentIndex = i;
                break;
            }
        }

        if (currentIndex >= 0)      // if a shelf has been found
        {
            int currentPoolCount = m_PooledObjects[currentIndex].Count;     // get the amount of pooled objects for the selected shelf
            int firstAvailable = -1;                                        // this wil be the index of the one we will use

            for (int i = 0; i < currentPoolCount; i++)          // cycle through the objects on the shelf and find an available object
            {
                if (!m_PooledObjects[currentIndex][i].GetComponent<PooledObject>().active)        // if active is set to false we set firstAvailable to be the index of the loop and break out
                {
                    firstAvailable = i;
                    break;
                }
            }

            if (firstAvailable >= 0)        // if firstAvailable has been set we get the object
            {
                m_PooledObjects[currentIndex][firstAvailable].GetComponent<PooledObject>().active = true;
                return m_PooledObjects[currentIndex][firstAvailable].gameObject;
            }

            if (m_WillGrow[currentIndex])   // if firstAvailable hasn't been set that means we need to create another one
            {
                GameObject obj = Instantiate(m_PooledObject[currentIndex]);         // create new object
                PooledObject poolScript = obj.AddComponent<PooledObject>();         // add a pooledObject script
                poolScript.Init(currentIndex);                                      // initialise it
                poolScript.active = true;                                         // set active to true
                obj.SetActive(false);                                               // turn the object off
                obj.transform.SetParent(m_ObjectShelves[currentIndex].transform);   // set the parent to be on the shelf
                m_PooledObjects[currentIndex].Add(poolScript);                      // add it to the correct list
                obj.name = name;                                                    // set the object's name
                return obj;                                                         // return the object
            }
            else
            {
                Debug.LogWarning("No objects left in the pool by the name of " + name + ". Returning NULL");
            }
        }
        else
        {
            Debug.LogError("No object in the object pooler by the name of " + name);
        }

        return null;
    }

    /// <summary>
    /// Used to return an object to the object pooler
    /// </summary>
    /// <param name="obj">The object that will be returned</param>
    public void RecycleObject(GameObject obj)
    {
        PooledObject poolScript = obj.GetComponent<PooledObject>();     // get the pooledObject script

        if (poolScript != null)
        {
            int correctIndex = poolScript.PoolIndex;                                  // get the index from the pool script
            obj.transform.SetParent(m_ObjectShelves[correctIndex].transform, false);    // set the parent of the object to the shelf
            obj.SetActive(false);                                                       // turn the object off
            poolScript.active = false;                                                // set the script to off
        }
        else
        {
            Debug.LogError("Tring to recycle object: " + obj.name + " but no PooledObject exists on it");       // if no pooledObject is on the object we will return an error
        }
    }
}
