using System.Collections.Generic;
using UnityEngine;


public class Soldier : MonoBehaviour
{
    private static List<Transform> soldiers = new List<Transform>();

    private void Awake()
    {
        soldiers.Add(transform);
    }

    // TODO ===> SI soldier voit player, ou mecano running away ==> attack + sight augmented

    public static Vector3 Nearest(Vector3 position, out float distance)
    {
        distance = Mathf.Infinity;
        float d;
        Vector3 nearestPos = Vector3.zero;
        foreach (var soldier in soldiers)
        {
            d = Vector3.Distance(position, soldier.position);
            if (d < distance)
            {
                distance = d;
                nearestPos = soldier.position;
            }
        }
        return nearestPos;
    }

    // Call by the Health component 
    public void Die()
    {
        soldiers.Remove(transform);
        Destroy(gameObject);
    }
}
