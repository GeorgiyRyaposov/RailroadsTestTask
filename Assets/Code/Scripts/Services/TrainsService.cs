using VContainer.Unity;

namespace Code.Scripts.Services
{
    /// <summary>
    /// Отвечает за общую логику работы поездов
    /// </summary>
    public class TrainsService : IStartable
    {
        private readonly Train[] _trains;
        private readonly GraphService _graphService;
        private readonly EfficiencyPathService _efficiencyPathService;
        private readonly ResourcesService _resourcesService;

        public TrainsService(Train[] trains, 
            GraphService graphService, ResourcesService resourcesService, 
            EfficiencyPathService efficiencyPathService)
        {
            _trains = trains;
            _graphService = graphService;
            _resourcesService = resourcesService;
            _efficiencyPathService = efficiencyPathService;
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
                
                train.Initialize(_graphService.GetRandomNode());
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
            var path = _efficiencyPathService.FindBestPath<Mine>(train.CurrentNode, train);
            train.SetPath(path);
        }

        private void MoveTrainToBase(Train train)
        {
            var path = _efficiencyPathService.FindBestPath<Base>(train.CurrentNode, train);
            train.SetPath(path);
        }
    }
}