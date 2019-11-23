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

        public AIBehaviour(Transform unit)
        {
            _current = unit;
            _players = new List<GameObject>(GameObject.FindGameObjectsWithTag(PLAYER_TAG));
            _actionState = ActionState.WAITING;
            _targetActive = false;
            _cooldownTime = 0f;
            _pointOfInterest = _current.position;
        }

        // update is called once per frame
        public void update()
        {
            var timelapse = Time.deltaTime; 
            playStateAction(timelapse);
            updateMovement(timelapse);
        }

        private void updateMovement(float timelapse)
        {
            
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

        public void removePlayer(GameObject player)
        {
            _players.Remove(player);
            if (_targetActive && player.transform.Equals(_target))
            {
                forceResetTarget();
            }
        }
    }
}