using System.Collections.Generic;
//using TestTaskMike.Controller;
using TestTaskMike.Gameplay;
//using TestTaskMike.Presenter;
//using TestTaskMike.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using UnityEngine.UI;
using TestTaskMike.Presenter;

namespace TestTaskMike.DI
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        [SerializeField] private GridView gridView;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterMessagePipe(builder);

            RegisterDomain(builder);
            RegisterApplication(builder);
            RegisterView(builder);
        }

        private void RegisterDomain(IContainerBuilder builder)
        {
            // Resolve the CellClicked subscriber and pass it into Grid so cells can subscribe.
            builder.Register(resolver =>
            {
                var subscriber = resolver.Resolve<IAsyncSubscriber<CellClicked>>();
                return new Grid(32, 32, subscriber);
            }, Lifetime.Singleton);

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

            // If you add other messages (e.g. GridInitialized) register them here as well:
            // builder.RegisterMessageBroker<GridInitialized>(options);
        }

        private void RegisterApplication(IContainerBuilder builder)
        {
            builder.Register<GridBuilder>(Lifetime.Scoped).AsImplementedInterfaces();
            builder.Register<ConstractionService>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();
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