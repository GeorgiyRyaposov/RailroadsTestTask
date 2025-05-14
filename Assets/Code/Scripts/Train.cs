using System;
using UnityEngine;
using System.Collections.Generic;

namespace Code.Scripts
{
    public class Train : MonoBehaviour
    {
        public float Speed => _speed;
        [SerializeField] private float _speed = 1f;
        
        public float BaseMiningTime => _baseMiningTime;
        [SerializeField] private float _baseMiningTime = 20f;
        
        public Node CurrentNode => _currentNode;
        private Node _currentNode;

        public bool HasResources;

        public Action<Train> OnDestinationArrived = (_) => { };
        public Action<Train> OnMineResourcesCompleted = (_) => { };
        
        private List<Node> _currentPath;
        private int _currentPathIndex;
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
        }

        private void Update()
        {
            if (_miningTime > 0)
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
            var step = Speed * Time.deltaTime;
            transform.forward = (targetNode.transform.position - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, targetNode.transform.position, step);

            if (Vector3.SqrMagnitude(transform.position - targetNode.transform.position) < 0.01f)
            {
                _currentNode = targetNode;
                _currentPathIndex++;
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
            
            _miningTime = BaseMiningTime * mine.miningTimeMultiplier;
        }
        
        private void MineResource()
        {
            _miningTime -= Time.deltaTime;

            if (_miningTime <= 0)
            {
                OnMineResourcesCompleted.Invoke(this);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.position, Vector3.one * 0.3f);

            // Рисуем путь
            if (_currentPath != null && _currentPathIndex < _currentPath.Count)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, _currentPath[_currentPathIndex].transform.position);

                for (var i = _currentPathIndex; i < _currentPath.Count - 1; i++)
                {
                    Gizmos.DrawLine(_currentPath[i].transform.position, _currentPath[i + 1].transform.position);
                }
            }
        }
    }
}