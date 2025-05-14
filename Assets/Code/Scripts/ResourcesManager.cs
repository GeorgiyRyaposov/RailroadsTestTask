using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts
{
    /// <summary>
    /// Счетчик ресурсов
    /// </summary>
    public class ResourcesManager : MonoBehaviour
    {
        [SerializeField] private Text _totalResourcesLabel;
        
        private float _totalResources;

        private void Start()
        {
            UpdateLabel();
        }

        public void Add(float amount)
        {
            _totalResources += amount;
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            _totalResourcesLabel.text = $"Total Resources: {_totalResources:F1}";
        }
    }
}