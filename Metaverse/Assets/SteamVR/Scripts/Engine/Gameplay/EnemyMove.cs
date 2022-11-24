using Platformer.Core;
using Platformer.Mechanics;
using UnityEngine;

namespace Engine.Gameplay
{

    /// <summary>
    /// Fired when a Player collides with an Enemy.
    /// </summary>
    /// <typeparam name="EnemyMove"></typeparam>
    public class EnemyMove : Simulation.Event<EnemyMove>
    {
        public EnemyController enemy;
        public PatrolPath path;

        public override void Execute()
        {
            if (Model.application.DEBUG && Model.application.DEEP_DEBUG)
            {
                Debug.Log("Engine.Gameplay.EnemyMove.Execute()");
            }
            // enemy.component.move(path);
        }
    }
}