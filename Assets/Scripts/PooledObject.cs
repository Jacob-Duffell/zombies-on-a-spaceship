using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public int PoolIndex { get; private set; }
    public bool active;

    public void Init(int index)
    {
        PoolIndex = index;
        active = false;
    }
}
