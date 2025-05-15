using System;

namespace Code.Scripts.Services
{
    /// <summary>
    /// Ведет учет ресурсов
    /// </summary>
    public class ResourcesService
    {
        public event Action OnResourcesChanged = () => { };
        
        private readonly GameState _gameState;

        public ResourcesService(GameState gameState)
        {
            _gameState = gameState;
        }

        public void AddResources(float amount)
        {
            _gameState.TotalResources += amount;
            OnResourcesChanged.Invoke();
        }

        public float GetTotalResources()
        {
            return _gameState.TotalResources;
        }
    }
}