using Code.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Code.Scripts.Views
{
    /// <summary>
    /// Счетчик ресурсов
    /// </summary>
    public class ResourcesPanel : MonoBehaviour
    {
        [SerializeField] private Text _totalResourcesLabel;
        
        private ResourcesService _resourcesService;

        [Inject]
        private void Construct(ResourcesService resourcesService)
        {
            _resourcesService = resourcesService;
        }
        
        private void Start()
        {
            _resourcesService.OnResourcesChanged += OnResourcesChanged;
            UpdateLabel();
        }

        private void OnDestroy()
        {
            if (_resourcesService != null)
            {
                _resourcesService.OnResourcesChanged -= OnResourcesChanged;
            }
        }

        private void OnResourcesChanged()
        {
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            var value = _resourcesService.GetTotalResources();
            _totalResourcesLabel.text = $"Total Resources: {value:0}";
        }
    }
}