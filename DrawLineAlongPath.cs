// Custom Action by DumbGameDev
// www.dumbgamedev.com

using System.Linq;
using UnityEngine;
using PathCreation;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Spline Path")]
    [Tooltip("Use line renderer along spline path.")]
    public class DrawLineAlongPath : FsmStateAction
    {
        [RequiredField]
        [Title("Line Renderer")]
        [CheckForComponent(typeof(LineRenderer))]
        [Tooltip("Line Renderer gameobject should be in the scene.")]
        public FsmGameObject lineGameObject;

        [RequiredField]
        [CheckForComponent(typeof(PathCreator))]
        [Tooltip("Gameobject that contains the path script.")]
        public FsmGameObject pathGameObject;

        [Tooltip("Only use spline points at every Nth value. This will reduce the number of line renderer points to save on performance.")]
        public FsmBool skipValues;

        [Tooltip("Only use spline points at every Nth value. 2 will use every 2d value. 10 will use every 10th value")]
        public FsmInt skipByValue;

        // private
        private Vector3[] pathPoints;

        // private
        private PathCreator _pathCreator;
        private LineRenderer _lineRenderer;

        public override void Reset()
        {
            pathGameObject = null;
            lineGameObject = null;
            skipValues = false;
            skipByValue = 0;
        }

        public override void OnEnter()
        {
            _pathCreator = pathGameObject.Value.GetComponent<PathCreator>();
            _lineRenderer = lineGameObject.Value.GetComponent<LineRenderer>();

            if (_lineRenderer == null || _pathCreator == null)
            {
                Debug.LogError("Missing component for playmaker action 'Draw Line Along Path action'. ");
            }

            DrawLine();
        }

        void DrawLine()
        {
            if (_pathCreator != null) pathPoints = _pathCreator.path.vertices;

            if (!skipValues.Value)
            {
                if (_lineRenderer != null)
                {
                    _lineRenderer.positionCount = pathPoints.Length;
                    _lineRenderer.SetPositions(pathPoints);
                }
            }
            else
            {
                var perXPoints = pathPoints.Where((x, i) => i % skipByValue.Value == 0).ToArray();
                if (_lineRenderer != null)
                {
                    _lineRenderer.positionCount = perXPoints.Length;
                    _lineRenderer.SetPositions(perXPoints);
                }
            }

            Finish();
        }
    }
}