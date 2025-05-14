using UnityEngine;

namespace Code.Scripts
{
    public class TrainsManager : MonoBehaviour
    {
        public Train[] Trains;
        public ResourcesManager ResourcesManager;
        public MapManager MapManager;
        
        private void Start()
        {
            SpawnTrains();
        }
        
        private void SpawnTrains()
        {
            for (int i = 0; i < Trains.Length; i++)
            {
                var train = Trains[i];
                
                train.Initialize(MapManager.GetRandomNode());
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
                    ResourcesManager.Add(baseNode.resourceMultiplier);
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
            var path = MapManager.FindBestPathToMine(train.CurrentNode, train);
            train.SetPath(path);
        }

        private void MoveTrainToBase(Train train)
        {
            var path = MapManager.FindBestPathToBase(train.CurrentNode, train);
            train.SetPath(path);
        }
    }
}