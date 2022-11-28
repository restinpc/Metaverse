using Platformer.Core;
using System;
using UnityEngine;

namespace Engine.Gameplay
{
    /// <summary>
    /// todo
    /// </summary>
    public class ZoomOutCamera : Simulation.Event<ZoomInCamera>
    {
        public string objectName;
        public string fallbackObjectName;
        public int from = 60;
        public int to = 170;
        public Func<Camera, int> callback = null;
        public override void Execute()
        {
            if (Model.application.DEBUG)
            {
                Debug.Log("Engine.Gameplay.ZoomOutCamera.Execute(" + objectName + ")");
            }
            Camera camera = null;
            GameObject gameObject = GameObject.Find(objectName);
            if (gameObject != null)
            {
                camera = gameObject.GetComponent<Camera>();
            } else
            {
                gameObject = GameObject.Find(fallbackObjectName);
                if (gameObject != null)
                {
                    camera = gameObject.GetComponent<Camera>();
                }
            }
            if (camera != null)
            {
                Model.coroutines["zoomOutCamera"] = Model.gameModel.StartCoroutine(
                    Model.gameModel.zoomOutCamera(camera, from, to, 0.001f, callback)
                );
            }
        }
    }
}