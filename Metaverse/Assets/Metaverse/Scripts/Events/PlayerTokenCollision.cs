using Engine.Core;
using Engine.Mechanics;
using UnityEngine;

namespace Engine.Events
{
    /// <summary>
    /// Fired when a player collides with a token.
    /// </summary>
    /// <typeparam name="PlayerCollision"></typeparam>
    public class PlayerTokenCollision : Simulation.Event<PlayerTokenCollision>
    {
        public Valve.VR.InteractionSystem.Player player;
        public TokenInstance token;

        public override void Execute()
        {
            if (Model.application.DEBUG)
            {
                Debug.Log("Engine.Gameplay.PlayerTokenCollision.Execute()");
            }
            AudioSource.PlayClipAtPoint(token.tokenCollectAudio, token.transform.position);
        }
    }
}