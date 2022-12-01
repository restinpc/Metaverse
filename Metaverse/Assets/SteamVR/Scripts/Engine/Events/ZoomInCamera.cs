using Engine.Core;
using System;
using UnityEngine;

namespace Engine.Events
{
    /// <summary>
    /// todo
    /// </summary>
    public class ZoomInCamera : Simulation.Event<ZoomInCamera>
    {
        public string objectName = "VRCamera";
        public string fallbackObjectName = "FallbackObjects";
        public int from = 160;
        public int to = 60;
        public Func<Camera, int> callback = null;
        public override void Execute()
        {
            if (Model.application.DEBUG)
            {
                Debug.Log("Engine.Gameplay.Events.Execute(" + objectName + ")");
            }
            Camera camera = null;
            GameObject gameObject = GameObject.Find(objectName);
            if (gameObject != null)
            {
                camera = gameObject.GetComponent<Camera>();
            }
            else
            {
                gameObject = GameObject.Find(fallbackObjectName);
                if (gameObject != null)
                {
                    camera = gameObject.GetComponent<Camera>();
                }
            }
            if (camera != null)
            {
                Model.coroutines["zoomInCamera"] = Model.gameModel.StartCoroutine(
                    Model.gameModel.zoomInCamera(camera, from, to, 0.001f, callback)
                );
            }
        }
    }
}