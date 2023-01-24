using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownManager : MonoBehaviour
{
    [SerializeField] private Town town;
    [SerializeField] private bool debug;

    public bool GetLocation(string name, out Vector3 coords, out float radius)
    {
        coords = Vector3.zero;
        radius = 0f;

        if(string.IsNullOrEmpty(name))
            return false;
        
        name = name.ToLower();
        bool found = false;

        foreach(Location l in town.locations)
        {
            if(l.name.ToLower() == name)
            {
                found = true;
                coords = l.coords;
                radius = l.radius;
            }
        }
        return found;
    }

    private void OnDrawGizmos() 
    {
        if(debug == false) return;
        
        foreach(Location l in town.locations)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(l.coords, new Vector3(.5f, .5f, .5f));
            Gizmos.DrawCube(l.coords + Vector3.up, new Vector3(.25f, 2f, .25f));
            Gizmos.DrawWireSphere(l.coords, l.radius);
        }
    }
}
