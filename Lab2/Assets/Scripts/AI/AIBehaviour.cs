using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Com.MyCompany.MyGame.AI
{
    public class AIBehaviour
    {
        public static string PLAYER_TAG = "Player";
        public static float DETECTION_RANGE = 10f;
        public static float MINIMUM_POI_RANGE = 3f;
        public static float MINIMUM_SHOOTING_RANGE = 5f;
        
        private List<GameObject> _players;
        private Transform _current;
        private ActionState _actionState;
        private Vector3 _pointOfInterest;
        private Transform _target;
        private bool _targetActive;
        private float _cooldownTime;

        public MovementRotationState _movementRotationState;
        public MovementTranslationState _movementTranslationState;

        public AIBehaviour(Transform unit)
        {
            _current = unit;
            _players = new List<GameObject>(GameObject.FindGameObjectsWithTag(PLAYER_TAG));
            _actionState = ActionState.WAITING;
            _movementRotationState = MovementRotationState.NONE;
            _movementTranslationState = MovementTranslationState.NONE;
            _targetActive = false;
            _cooldownTime = 0f;
            _pointOfInterest = _current.position;
        }

        // update is called once per frame
        public void update()
        {
            var timelapse = Time.deltaTime; 
            playStateAction(timelapse);
            updateMovementState(timelapse);
        }

        private void updateMovementState(float timelapse)
        {
            if (_actionState == ActionState.WANDERING) {
                movementAdjustmentToObjective(_pointOfInterest, MINIMUM_POI_RANGE);
            } else if (_actionState == ActionState.APPROACHING) {
                movementAdjustmentToObjective(_target.position, MINIMUM_SHOOTING_RANGE);
            }
        }

        private void playStateAction(float timelapse)
        {
            _cooldownTime -= timelapse;
            if (_cooldownTime > 0) {
                resumeStateAction();
            } else {
                completeStateAction();
            }
        }

        private void resumeStateAction()
        {
            if (_actionState == ActionState.WANDERING) {
                resumeWanderingAction();
            } else if (_actionState == ActionState.APPROACHING) {
                resumeApproachAction();
            }
        }

        // resume action when AI is wandering between points of interests, looking for an enemy
        private void resumeWanderingAction()
        {
            searchTarget();
            if (_targetActive) {
                _actionState = ActionState.TARGETING;
                _cooldownTime = 1f;
            } else {
                if (Vector3.Distance(_current.position, _pointOfInterest) < MINIMUM_POI_RANGE) {
                    _cooldownTime = 0f;
                }
            }
        }

        private void resumeApproachAction()
        {
            if (Vector3.Distance(_current.position, _target.position) < MINIMUM_SHOOTING_RANGE) {
                _cooldownTime = 0f;
            }
        }

        private void completeStateAction()
        {
            if (_actionState == ActionState.WAITING) {
                completeWaitingAction();
            } else if (_actionState == ActionState.WANDERING) {
                completeWanderingAction();
            } else if (_actionState == ActionState.APPROACHING) {
                completeApproachAction();
            }
        }

        private void completeWaitingAction()
        {
            searchTarget();
            if (_targetActive) {
                _actionState = ActionState.TARGETING;
                _cooldownTime = 1f;
            } else {
                _actionState = ActionState.WANDERING;
                _cooldownTime = 4f;
                _pointOfInterest = generatePointOfInterest();
            }
        }

        private void completeWanderingAction()
        {
            _actionState = ActionState.WAITING;
            _cooldownTime = Random.Range(1f, 3f);
        }

        private void completeApproachAction()
        {
            
        }

        private void searchTarget()
        {
            var closestUnit = _current;
            var closestDistance = float.MaxValue;
            foreach (var player in _players)
            {
                var playerTr = player.transform;
                var playerDistance = Vector3.Distance(_current.position, playerTr.position);
                if (playerDistance < DETECTION_RANGE) {
                    if (playerDistance < closestDistance) {
                        closestUnit = playerTr;
                        closestDistance = playerDistance;
                    }
                }
            }
            if (closestDistance != float.MaxValue) {
                setTarget(closestUnit);
            }
        }

        private Vector3 generatePointOfInterest()
        {
            float distance = Random.Range(DETECTION_RANGE % 2, DETECTION_RANGE);
            int degreeVariation = Random.Range(-135, 135);
            var currentPosition = _current.position;
            var orientation = Util.toRad(_current.eulerAngles.y + degreeVariation);
            return new Vector3(
                currentPosition.x - distance * (float)Math.Sin(orientation),
                currentPosition.y,
                currentPosition.z - distance * (float)Math.Cos(orientation));
        }

        private void setTarget(Transform newTarget)
        {
            _target = newTarget;
            _targetActive = true;
        }

        private void forceResetTarget()
        {
            _targetActive = false;
            _target = null;
            if (_actionState != ActionState.WAITING && _actionState != ActionState.WANDERING)
            {
                _actionState = ActionState.WAITING;
                _cooldownTime = Random.Range(1f, 3f);
            }
        }

        private void movementAdjustmentToObjective(Vector3 objective, float minDistance)
        {
            var angle = alignmentToObjective(objective);
            translationToObjective(objective, angle, minDistance);
        }

        private double alignmentToObjective(Vector3 objective)
        {
            var currentPosition = _current.position;
            var relativeAngle = Util.angleBetweenVec(
                currentPosition.x,
                objective.x,
                currentPosition.z,
                objective.z);
            relativeAngle += _current.rotation.eulerAngles.y;
            if (relativeAngle > 180) {
                relativeAngle -= 360;
            }
            if (relativeAngle > 0) {
                _movementRotationState = MovementRotationState.LEFT;
            } else if (relativeAngle < 0) {
                _movementRotationState = MovementRotationState.RGHT;
            } else {
                _movementRotationState = MovementRotationState.NONE;
            }
            return relativeAngle;
        }

        private void translationToObjective(Vector3 objective, double relativeAngle, float minDistance)
        {
            var currentPosition = _current.position;
            var distance = Vector3.Distance(currentPosition, objective);
            if (distance < minDistance || Math.Abs(relativeAngle) > 90) {
                _movementTranslationState = MovementTranslationState.SLOW;
            } else if (Math.Abs(relativeAngle) > 40 || distance < minDistance * 1.5) {
                _movementTranslationState = MovementTranslationState.HALF_FORWARD;
            } else {
                _movementTranslationState = MovementTranslationState.FORWARD;
            }
        }

        public void removePlayer(GameObject player)
        {
            _players.Remove(player);
            if (_targetActive && player.transform.Equals(_target)) {
                forceResetTarget();
            }
        }
    }
}