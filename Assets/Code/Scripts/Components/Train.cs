using System;
using UnityEngine;
using System.Collections.Generic;

namespace Code.Scripts
{
    /// <summary>
    /// Поезд: содержит параметры и умеет передвигаться по заданному пути
    /// </summary>
    public class Train : MonoBehaviour
    {
        public float Speed => _speed;
        [SerializeField] private float _speed = 1f;
        
        public float BaseMiningTime => _baseMiningTime;
        [SerializeField] private float _baseMiningTime = 20f;
        
        public Node CurrentNode => _currentNode;
        private Node _currentNode;

        [NonSerialized] public bool HasResources;

        public Action<Train> OnDestinationArrived = (_) => { };
        public Action<Train> OnMineResourcesCompleted = (_) => { };
        
        private List<Node> _currentPath;
        private int _currentPathIndex;
        private float _distanceToNextNode;
        private float _passedTime;
        private bool _isMining;
        private float _miningTime;

        public void Initialize(Node startNode)
        {
            _currentNode = startNode;
            transform.position = _currentNode.transform.position;
        }

        public void SetPath(List<Node> path)
        {
            _currentPath = path;
            _currentPathIndex = 0;
            ResetDistances();
        }

        private void ResetDistances()
        {
            _passedTime = 0f;
            if (_currentPathIndex < _currentPath.Count)
            {
                _distanceToNextNode = _currentNode.GetDistanceTo(_currentPath[_currentPathIndex]);
            }
        }

        private void Update()
        {
            if (_isMining)
            {
                MineResource();
            }
            else if (_currentPath != null && _currentPathIndex < _currentPath.Count)
            {
                MoveAlongPath();
            }
            else
            {
                ArriveAtDestination();
            }
        }

        private void MoveAlongPath()
        {
            var targetNode = _currentPath[_currentPathIndex];
            _passedTime += Time.deltaTime;
            var timeToPass = _distanceToNextNode / Speed;
            var progress = _passedTime / timeToPass;
            
            transform.forward = (targetNode.transform.position - transform.position).normalized;
            transform.position = Vector3.Lerp(_currentNode.transform.position, targetNode.transform.position, progress);
            
            if (progress > 0.99f)
            {
                _currentNode = targetNode;
                _currentPathIndex++;
                
                ResetDistances();
            }
        }

        private void ArriveAtDestination()
        {
            OnDestinationArrived.Invoke(this);
        }

        public void StartMineResource()
        {
            if (_currentNode is not Mine mine)
            {
                Debug.LogError($"Wrong node type: {_currentNode.GetType()}, expected type: {typeof(Mine)}");
                return;
            }
            
            _isMining = true;
            _miningTime = BaseMiningTime * mine.MiningTimeMultiplier;
        }
        
        private void MineResource()
        {
            _miningTime -= Time.deltaTime;

            if (_miningTime <= 0)
            {
                _isMining = false;
                OnMineResourcesCompleted.Invoke(this);
            }
        }
    }
}