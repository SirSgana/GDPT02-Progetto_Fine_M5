using UnityEngine;

public class RayMonster : MonoBehaviour
{
    public LayerMask layerMask;
    int rayCount = 20;

    RaycastHit hit;
    float boxWidth;

    void Start()
    {
        // get box half width, divided with raycount to fit them all in
        boxWidth = GetComponent<Renderer>().bounds.size.x * 0.5f / (float)rayCount;
    }

    void Update()
    {
        // shooting direction
        var dir = transform.forward;
        // shoot many rays
        for (int i = 0; i < rayCount; i++)
        {
            // get ray start position, based on boxwidth, using lerp to interpolate positions from start edge to end edge
            var start = Vector3.Lerp(transform.position - (transform.right * (rayCount)) * boxWidth, transform.position + (transform.right * (rayCount)) * boxWidth, i / ((float)rayCount - 1));

            var ray = new Ray(start, dir);
            if (Physics.Raycast(ray, out hit, 999, layerMask))
            {
                // got hit
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);
            }
            else // no hits
            {
                Debug.DrawRay(ray.origin, ray.direction * 999, Color.green);
            }
        }
    }
}