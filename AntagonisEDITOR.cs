using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Antagonis))]
public class AntagonisEDITOR : Editor
{
    private void OnSceneGUI()
    {
        Antagonis fov = (Antagonis)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.PFOV.position, Vector3.up, Vector3.forward, 360, fov.v_radius);

        Vector3 viewAngle01 = DirectionFromAngle(fov.PFOV.eulerAngles.y, -fov.aTGData.angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fov.PFOV.eulerAngles.y, fov.aTGData.angle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(fov.PFOV.position, fov.PFOV.position + viewAngle01 * fov.v_radius);
        Handles.DrawLine(fov.PFOV.position, fov.PFOV.position + viewAngle02 * fov.v_radius);

        if (fov.canSeePlayer)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.PFOV.position, fov.target.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}