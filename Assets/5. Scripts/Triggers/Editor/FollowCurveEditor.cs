using Triggers;
using UnityEditor;
using UnityEngine;

namespace Triggers.Editor
{
    [CustomEditor(typeof(FollowCurve))]
    public class FollowCurveEditor : UnityEditor.Editor
    {
        private FollowCurve _curve;
        private int _selectedPointIndex = -1;

        private void OnEnable()
        {
            _curve = (FollowCurve)target;
        }

        private void OnSceneGUI()
        {
            if (_curve.points == null)
                return;

            // Draw selectable points as spheres
            for (int i = 0; i < _curve.points.Length; i++)
            {
                Vector3 pointWorldPosition = _curve.transform.TransformPoint(_curve.points[i]);

                if (Handles.Button(pointWorldPosition, Quaternion.identity, 0.1f, 0.2f, Handles.SphereHandleCap))
                {
                    _selectedPointIndex = i; // Set this point as selected when clicked
                }

                // Draw a larger sphere to indicate the selected point
                if (i == _selectedPointIndex)
                {
                    Handles.color = Color.yellow;
                    Handles.SphereHandleCap(0, pointWorldPosition, Quaternion.identity, 0.15f, EventType.Repaint);
                }
            }

            // If a point is selected, allow it to be moved using a position handle
            if (_selectedPointIndex >= 0 && _selectedPointIndex < _curve.points.Length)
            {
                EditorGUI.BeginChangeCheck();

                // Get the world position of the selected point
                Vector3 selectedPointWorldPosition = _curve.transform.TransformPoint(_curve.points[_selectedPointIndex]);
                Vector3 newPointWorldPosition = Handles.PositionHandle(selectedPointWorldPosition, Quaternion.identity);

                // If the position of the point has changed, update it
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_curve, "Move Curve Point");
                    _curve.points[_selectedPointIndex] = _curve.transform.InverseTransformPoint(newPointWorldPosition);
                }
            }
        }
        

    }
}