using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTurret : MonoBehaviour
{
    [SerializeField] LayerMask targetLayer;
    [SerializeField] GameObject crosshair;
    [SerializeField] float baseTurnSpeed = 3;
    [SerializeField] GameObject gun;
    [SerializeField] Transform turretBase;
    [SerializeField] Transform barrelEnd;
    [SerializeField] LineRenderer line;

    List<Vector3> laserPoints = new List<Vector3>();
    private Vector3 endPosition;

    // Update is called once per frame
    void Update()
    {
        TrackMouse();
        TurnBase();

        laserPoints.Clear();
        laserPoints.Add(barrelEnd.position);

        Bounce(barrelEnd.forward, barrelEnd.position);
    }

    void TrackMouse()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if(Physics.Raycast(cameraRay, out hit, 1000, targetLayer ))
        {
            crosshair.transform.forward = hit.normal;
            crosshair.transform.position = hit.point + hit.normal * 0.1f;
        }
    }

    void TurnBase()
    {
        Vector3 directionToTarget = (crosshair.transform.position - turretBase.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, directionToTarget.y, directionToTarget.z));
        turretBase.transform.rotation = Quaternion.Slerp(turretBase.transform.rotation, lookRotation, Time.deltaTime * baseTurnSpeed);
    }

    void Bounce(Vector3 r, Vector3 s)
    {
        if (Physics.Raycast(s, r, out RaycastHit hit, 1000.0f, targetLayer))
        {
            laserPoints.Add(hit.point);
            r = -2F * Vector3.Dot(hit.normal, r) * hit.normal + r;


            line.positionCount = laserPoints.Count;
            for (int i = 0; i < line.positionCount; i++)
            {
                line.SetPosition(i, laserPoints[i]);
                s = laserPoints[i];
            }

            Bounce(r, s);
        }

        else
        {
            line.positionCount = line.positionCount + 1;
            endPosition = s + (r * 10);
            line.SetPosition(line.positionCount - 1, endPosition);
        }

        return;
    }
}
