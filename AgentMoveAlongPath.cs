// Custom Action by DumbGameDev
// www.dumbgamedev.com

using UnityEngine;
using PathCreation;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Spline Path")]
    [Tooltip("Move agent along path according to speed. Should be set to everyframe to move an object.")]
    public class AgentMoveAlongPath : FsmStateAction
    {
        [RequiredField]
        [Title("Agent")]
        [Tooltip("Gameobject to move along path.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("Gameobject that contains the path script.")]
        public FsmGameObject pathGameObject;

        [ObjectType(typeof(EndOfPathInstruction))]
        public FsmEnum endOfPathInstruction;

        public FsmFloat speed;

        public FsmBool ignoreRotation;

        public FsmVector3 positionOffset;
        public FsmVector3 rotationOffset;

        [ActionSection("Results")]
        [UIHint(UIHint.Variable)]
        [Tooltip("Returns the percent of the path travelled between 0 - 1.")]
        public FsmFloat percentTravelled;

        [UIHint(UIHint.Variable)]
        [Tooltip("Returns the total distance of the path.")]
        public FsmFloat pathTotalDistance;

        [UIHint(UIHint.Variable)]
        [Tooltip("Returns the total distance travelled.")]
        public FsmFloat distanceTravelled;

        [Tooltip("Fires event if the agent has reached the end of the path and end of path instructions is set to stop.")]
        public FsmEvent pathCompleteEvent;

        public FsmBool everyFrame;

        private PathCreator _pathCreator;

        public override void Reset()
        {
            speed = 5f;
            pathGameObject = null;
            gameObject = null;
            everyFrame = true;
            distanceTravelled = 0f;
            pathTotalDistance = 0f;
            ignoreRotation = false;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);

            CalculateMovement();
            _pathCreator = pathGameObject.Value.GetComponent<PathCreator>();

            // get total distance if path creator is not null
            if (_pathCreator == null)
            {
                return;
            }
            else
            {
                pathTotalDistance.Value = _pathCreator.path.length;
            }

            if (!everyFrame.Value)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            if (everyFrame.Value)
            {
                CalculateMovement();
            }
        }

        void CalculateMovement()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go == null)
            {
                return;
            }

            if (_pathCreator == null) return;

            // set position
            go.transform.position = positionOffset.Value + _pathCreator.path.GetPointAtDistance(distanceTravelled.Value, (EndOfPathInstruction) endOfPathInstruction.Value);

            // set rotation
            if (!ignoreRotation.Value)
            {
                go.transform.rotation = Quaternion.Euler(rotationOffset.Value) * _pathCreator.path.GetRotationAtDistance(distanceTravelled.Value, (EndOfPathInstruction) endOfPathInstruction.Value);
            }

            // set distance travelled
            distanceTravelled.Value += speed.Value * Time.deltaTime;

            // get total percent travelled
            percentTravelled.Value = distanceTravelled.Value / pathTotalDistance.Value;

            if (pathTotalDistance.Value <= distanceTravelled.Value && (EndOfPathInstruction) endOfPathInstruction.Value == EndOfPathInstruction.Stop)
            {
                Fsm.Event(pathCompleteEvent);
            }
        }
    }
}