using ArcadeFrontend.Interfaces;
using System;
using System.Collections.Generic;

namespace ArcadeFrontend.Providers
{
    /// <summary>
    /// Run this between renders.
    /// </summary>
    public class NextTickActionProvider : ITick
    {
        private readonly Queue<Action> nextTickActions = new();

        public NextTickActionProvider()
        {
        }

        public void Enqueue(Action action)
        {
            nextTickActions.Enqueue(action);
        }

        public void Tick(float deltaSeconds)
        {
            while (nextTickActions.TryDequeue(out var next))
            {
                next();
            }
        }
    }
}
