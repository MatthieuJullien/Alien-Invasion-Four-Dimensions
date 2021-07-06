using System.Collections.Generic;
using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    public int Count { get; private set; }

    private void Awake()
    {
        Count = transform.childCount;
    }

    //private void OnDrawGizmosSelected()
    //{
    //    var count = Count;
    //    for (int i = 0; i < count; i++)
    //    {
    //        int j = (i + 1) % count;
    //        Gizmos.DrawSphere(GetWaypointPosition(i), 1f);
    //        Gizmos.DrawLine(GetWaypointPosition(i), GetWaypointPosition(j));
    //    }
    //}

    public Vector3 GetWaypointPosition(int i)
    {
        return transform.GetChild(i).position;
    }
}