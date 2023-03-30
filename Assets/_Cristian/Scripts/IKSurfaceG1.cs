using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class IKSurfaceG1 : MonoBehaviour
{
    [SerializeField] private Transform detectionReference; // P0
    [SerializeField] private Transform innerDetectionReference; // P1
    [SerializeField] private float radius = 1.0f;
    [SerializeField] private LayerMask detectionLayers;

    Vector3 QueryNearbySurfacePosition()
    {
        Collider[] objectsInRange = Physics.OverlapSphere(detectionReference.position, radius, detectionLayers);
        Vector3[] closestPoints = new Vector3[objectsInRange.Length];
        for (int i = 0; i < objectsInRange.Length; i++)
        {
            closestPoints[i] = objectsInRange[i].ClosestPoint(detectionReference.position);
        }

        Vector3 closestPoint = closestPoints.OrderBy(vector => Vector3.Distance(vector,detectionReference.position)).First();
        if (closestPoint != detectionReference.position)
            return closestPoint;
        Ray ray = new Ray(innerDetectionReference.position, detectionReference.position - innerDetectionReference.position);
        Physics.Raycast(ray, out RaycastHit hit, ray.direction.magnitude, detectionLayers);
        return hit.point;
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(detectionReference.position,radius);
        Gizmos.DrawLine(detectionReference.position,QueryNearbySurfacePosition());
    }
}
