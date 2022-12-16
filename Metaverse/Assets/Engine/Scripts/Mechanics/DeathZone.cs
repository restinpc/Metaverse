using UnityEngine;

namespace Engine.Mechanics
{
    /// <summary>
    /// DeathZone components mark a collider which will schedule a
    /// PlayerEnteredDeathZone event when the player enters the trigger.
    /// </summary>
    public class DeathZone : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D collider)
        {
            var p = collider.gameObject.GetComponent<Valve.VR.InteractionSystem.Player>();
            if (p != null)
            {
                // todo
            }
        }
    }
}