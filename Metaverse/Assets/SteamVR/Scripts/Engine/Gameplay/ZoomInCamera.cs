using Platformer.Core;
using System;
using UnityEngine;

namespace Engine.Gameplay
{
    /// <summary>
    /// todo
    /// </summary>
    public class ZoomInCamera : Simulation.Event<ZoomInCamera>
    {
        public string objectName;
        public string fallbackObjectName;
        public int from = 170;
        public int to = 60;
        public Func<Camera, int> callback = null;
        public override void Execute()
        {
            if (Model.application.DEBUG)
            {
                Debug.Log("Engine.Gameplay.ZoomInCamera.Execute(" + objectName + ")");
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