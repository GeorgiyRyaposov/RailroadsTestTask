using Code.Scripts.EfficiencyPathSolvers;
using Code.Scripts.Services;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Code.Scripts
{
    public class EntryPoint : LifetimeScope
    {
        [SerializeField] private Train[] _trains;
        [SerializeField] private Node[] _nodes;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_trains);
            builder.RegisterInstance(_nodes);
            
            builder.RegisterEntryPoint<TrainsService>();
            builder.Register<GraphService>(Lifetime.Singleton);
            builder.Register<ResourcesService>(Lifetime.Singleton);
            builder.Register<GameState>(Lifetime.Singleton);
            
            builder.Register<EfficiencyPathService>(Lifetime.Singleton);
            builder.Register<IEfficiencyPathSolver, BaseEfficiencyPathSolver>(Lifetime.Singleton);
            builder.Register<IEfficiencyPathSolver, MineEfficiencyPathSolver>(Lifetime.Singleton);
        }
    }
}