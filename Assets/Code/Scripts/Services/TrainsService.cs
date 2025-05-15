using System.Collections.Generic;
using Code.Scripts.Services;
using VContainer.Unity;

namespace Code.Scripts
{
    /// <summary>
    /// Отвечает за общую логику работы поездов
    /// </summary>
    public class TrainsService : IStartable
    {
        private readonly Train[] _trains;
        private readonly PathService _pathService;
        private readonly ResourcesService _resourcesService;

        public TrainsService(Train[] trains, 
            PathService pathService, ResourcesService resourcesService)
        {
            _trains = trains;
            _pathService = pathService;
            _resourcesService = resourcesService;
        }


        public void Start()
        {
            SpawnTrains();
        }
        
        private void SpawnTrains()
        {
            for (int i = 0; i < _trains.Length; i++)
            {
                var train = _trains[i];
                
                train.Initialize(_pathService.GetRandomNode());
                train.OnDestinationArrived = OnDestinationArrived;
                train.OnMineResourcesCompleted = OnMineResourcesCompleted;

                SetDestination(train);
            }
        }

        private void OnDestinationArrived(Train train)
        {
            SetDestination(train);
        }

        private void SetDestination(Train train)
        {
            if (train.CurrentNode is Base baseNode)
            {
                if (train.HasResources)
                {
                    _resourcesService.AddResources(baseNode.ResourceMultiplier);
                    train.HasResources = false;
                }

                MoveTrainToMine(train);
            }
            else if (train.CurrentNode is Mine)
            {
                if (train.HasResources)
                {
                    MoveTrainToBase(train);
                }
                else
                {
                    train.StartMineResource();
                }
            }
            else
            {
                MoveTrainToMine(train);
            }
        }

        private void OnMineResourcesCompleted(Train train)
        {
            train.HasResources = true;
            MoveTrainToBase(train);
        }
        
        private void MoveTrainToMine(Train train)
        {
            var path = _pathService.FindBestPathToMine(train.CurrentNode, train);
            train.SetPath(path);
        }

        private void MoveTrainToBase(Train train)
        {
            var path = _pathService.FindBestPathToBase(train.CurrentNode, train);
            train.SetPath(path);
        }
    }
}