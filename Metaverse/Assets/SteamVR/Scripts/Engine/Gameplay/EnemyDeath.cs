using Platformer.Core;
using Platformer.Mechanics;
using UnityEngine;

namespace Engine.Gameplay
{
    /// <summary>
    /// Fired when the health component on an enemy has a hitpoint value of  0.
    /// </summary>
    /// <typeparam name="EnemyDeath"></typeparam>
    public class EnemyDeath : Simulation.Event<EnemyDeath>
    {
        public EnemyController enemy;

        public override void Execute()
        {
            if (Model.application.DEBUG)
            {
                Debug.Log("Engine.Gameplay.EnemyDeath.Execute()");
            }
            // enemy.component.death();
        }
    }
}