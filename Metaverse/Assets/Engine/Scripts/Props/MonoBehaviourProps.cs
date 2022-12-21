using UnityEngine;

namespace Engine.Props
{
    public class MonoBehaviourProps : MonoBehaviour
    {
        Components.Component component;
        public string componentName;
        public bool start;
        public bool update;
        public bool fixedUpdate;
        public bool lateUpdate;
        public bool onGUI;
        public bool onDisable;
        public bool onEnable;

        void Start()
        {
            if (component == null)
            {
                component = Model.application.getComponentByName(componentName);
            }
            if (component != null && start)
            {
                component.Start();
            }
        }

        void Update()
        {
            if (component != null && update)
            {
                component.Update();
            }
        }

        void FixedUpdate()
        {
            if (component != null && fixedUpdate)
            {
                component.FixedUpdate();
            }
        }
        void LateUpdate()
        {
            if (component != null && lateUpdate)
            {
                component.LateUpdate();
            }
        }
        void OnGUI()
        {
            if (component != null && onGUI)
            {
                component.OnGUI();
            }
        }
        void OnDisable()
        {
            if (component != null && onDisable)
            {
                component.OnDisable();
            }
        }
        void OnEnable()
        {
            if (component != null && onEnable)
            {
                component.OnEnable();
            }
        }
    }
}