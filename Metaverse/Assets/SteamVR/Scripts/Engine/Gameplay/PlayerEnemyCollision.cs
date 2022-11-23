using Platformer.Core;
using Platformer.Mechanics;
using UnityEngine;

namespace Engine.Gameplay
{

    /// <summary>
    /// Fired when a Player collides with an Enemy.
    /// </summary>
    /// <typeparam name="EnemyCollision"></typeparam>
    public class PlayerEnemyCollision : Simulation.Event<PlayerEnemyCollision>
    {
        public EnemyController enemy;
        public PlayerController player;

        public override void Execute()
        {
            if (Model.application.DEBUG)
            {
                Debug.Log("Engine.Gameplay.PlayerEnemyCollision.Execute()");
            }
            Model.player.enemyCollision(enemy);
        }
    }
}