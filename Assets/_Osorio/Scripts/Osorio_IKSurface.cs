using System;
using System.Linq;
using UnityEngine;

public class Osorio_IKSurface : MonoBehaviour
{
    private const float REFRESH_DELAY = 0.5f;

    [SerializeField] private float detectionRadius;
    [SerializeField] private LayerMask detectionMask;
    [SerializeField] private Transform referencePoint;


    /// <summary>
    /// Decides wether or not a surface is suitable for IK snap 
    /// </summary>
    private Collider[] Query()
    {
        return Physics.OverlapSphere(transform.position, detectionRadius, detectionMask);
    }

    // encontrar el objeto cuyo punto mas cercano al punto de referencia es el más cercano
    private Vector3 GetNearestPositionForSnap(Collider[] nearColliders)
    {
        try
        {
            //Encontrar el punto más cercano a ña superficie 
            //Crear una lista de los puntos más cercanos a uno de los colliders
            var closestPoints = nearColliders.Select(collider => collider.ClosestPoint(referencePoint.position));
            //Encuentra el punto más cercano perteneciente al collider más cercano
            Vector3 closestPoint = closestPoints
                .OrderBy(position => Vector3.Distance(referencePoint.position, position)).First();
            return closestPoint;
        }
        catch
        {
            //Si no detecta nada, retornamos la posicion de la misma mano
            return transform.position;
        }
    }

    private void LateUpdate()
    {
        Vector3 positionToSnap = GetNearestPositionForSnap(Query());
        Debug.DrawLine(transform.position, positionToSnap);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
