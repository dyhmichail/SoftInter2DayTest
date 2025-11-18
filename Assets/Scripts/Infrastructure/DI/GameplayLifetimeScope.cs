using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using TestTaskMike.Presentation;
using TestTaskMike.Application;
using Grid = TestTaskMike.Domain.Grid;

namespace TestTaskMike
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        [SerializeField] private GridPresenter gridView;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterMessagePipe(builder);

            RegisterDomain(builder);
            RegisterApplication(builder);
            RegisterView(builder);
        }

        private void RegisterDomain(IContainerBuilder builder)
        {
            //builder.Register(resolver =>
            //{
            //    var subscriber = resolver.Resolve<IAsyncSubscriber<CellClicked>>();
            //    return new Grid(32, 32);
            //}, Lifetime.Singleton);

            builder.Register<Grid>(Lifetime.Singleton)
                .WithParameter("width", 32)
                .WithParameter("height", 32);

            builder.Register<GenerateGridState>(Lifetime.Scoped);
            builder.Register<LoadSavedGameState>(Lifetime.Scoped);
            builder.Register<MainGameState>(Lifetime.Scoped);

            builder.Register(resolver =>
            {
                var s0 = resolver.Resolve<GenerateGridState>();
                var s1 = resolver.Resolve<LoadSavedGameState>();
                var s2 = resolver.Resolve<MainGameState>();
                IGameState[] states = new IGameState[] { s0, s1, s2 };
                return (IReadOnlyList<IGameState>)states;
            }, Lifetime.Scoped).As<IReadOnlyList<IGameState>>();

            builder.RegisterEntryPoint<GameStateEngine>(Lifetime.Scoped);
        }

        private void RegisterMessagePipe(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();

            builder.RegisterMessageBroker<GameStateEnum>(options);

            // Register the CellClicked message so IAsyncPublisher<CellClicked> and IAsyncSubscriber<CellClicked>
            // can be injected and used across the app.
            builder.RegisterMessageBroker<CellClicked>(options);
            builder.RegisterMessageBroker<BuildingSet>(options);

            builder.RegisterMessageBroker<RequestGridInitialization>(options);
            builder.RegisterMessageBroker<GridInitialized>(options);
        }

        private void RegisterApplication(IContainerBuilder builder)
        {
            builder.Register<GridBuilder>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();

            builder.Register<GridInitializationService>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();

            builder.Register<ConstractionService>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();
            builder.Register<UpgradeService>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();
            builder.Register<RemoveService>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();
            builder.Register<MoveService>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();
            builder.Register<ToolsServiceLocator>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();

            builder.Register<EconomyService>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();
        }

        private void RegisterView(IContainerBuilder builder)
        {
            builder.RegisterComponent(gridView);


            // register CellView factory. Factory is referenced by GridBuilderPresenter.
            //builder.RegisterFactory<CardView>(container =>
            //{
            //    CardView InstantiateCardView()
            //    {
            //        return container.Instantiate(_cardViewPrefab, _cardViewParent).GetComponent<CardView>();
            //    }

            //    return InstantiateCardView;
            //},
            //    Lifetime.Scoped);
        }
    }
}