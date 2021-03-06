// Custom Action by DumbGameDev
// www.dumbgamedev.com

using UnityEngine;
using PathCreation;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Spline Path")]
    [Tooltip("Spawn objects along the path and save as an array. Everyframe is likely not nessesary (or you will spawn a lot of objects.")]
    public class SpawnObjectAlongPath : FsmStateAction
    {
        [RequiredField]
        [Title("Spawn Object")]
        [Tooltip("Gameobject to spawn.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("Gameobject that contains the path script.")]
        public FsmGameObject pathGameObject;

        [Tooltip("Number of gameobjects to spawn.")]
        [Title("Number of Objects")]
        public FsmFloat number;

        public FsmBool useRotation;
        public FsmBool everyFrame;

        [ActionSection("Results")]
        [UIHint(UIHint.Variable)]
        [Tooltip("Returns all of the gameobjects so they can manipulated further as needed.")]
        [ArrayEditor(VariableType.GameObject)]
        public FsmArray allObjectsArray;

        // private
        private PathCreator _pathCreator;
        private float distance;
        private float totalDistance;
        private float distancePerObject;
        private Vector3 objPosition;
        private Quaternion objRotation;

        public override void Reset()
        {
            pathGameObject = null;
            gameObject = null;
            everyFrame = false;
            number = 0f;
            useRotation = true;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            _pathCreator = pathGameObject.Value.GetComponent<PathCreator>();

            PlaceObjects();

            if (!everyFrame.Value)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            if (everyFrame.Value)
            {
                PlaceObjects();
            }
        }

        void PlaceObjects()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go == null)
            {
                return;
            }

            if (_pathCreator == null) return;
            if (number.Value <= 0) return;

            // get total distance for each object according to path length
            totalDistance = _pathCreator.path.length;
            distancePerObject = totalDistance / number.Value;

            for (int i = 0; i < number.Value; i++)
            {
                // get position and rotation of each object
                distance += distancePerObject;
                objPosition = _pathCreator.path.GetPointAtDistance(distance, EndOfPathInstruction.Loop);

                if (useRotation.Value)
                {
                    objRotation = _pathCreator.path.GetRotationAtDistance(distance, EndOfPathInstruction.Loop);
                }

                // Instantiate each object
                var spawnedObject = GameObject.Instantiate(go, objPosition, objRotation);
                allObjectsArray.Resize(allObjectsArray.Length + 1);
                allObjectsArray.Set(i, spawnedObject);
            }
        }
    }
}