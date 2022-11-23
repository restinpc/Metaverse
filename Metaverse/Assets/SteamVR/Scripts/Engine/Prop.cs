using UnityEngine;
using System.Collections.Generic;

namespace Engine
{
    public class Prop
    {
        public dynamic value;
        string stringValue;
        int intValue;
        bool boolValue;
        Vector2 vector2Value;
        Dictionary<string, Prop> dictionaryValue;
        Components.Component componentValue;
        System.Type type;
        /*
         * @constructors
         */
        public Prop(string value)
        {
            this.value = value;
            this.stringValue = value;
            this.type = typeof(string);
        }

        public Prop(int value)
        {
            this.value = value;
            this.intValue = value;
            this.type = typeof(int);
        }

        public Prop(bool value)
        {
            this.value = value;
            this.boolValue = value;
            this.type = typeof(bool);
        }

        public Prop(Vector2 value)
        {
            this.vector2Value = value;
            this.type = typeof(Vector2);
        }

        public Prop(Dictionary<string, Prop> value)
        {
            this.dictionaryValue = value;
            this.type = typeof(Dictionary<string, Prop>);
        }

        public Prop(Components.Component value)
        {
            this.componentValue = value;
            this.type = typeof(Component);
        }
        /*
         * @setters
         */
        private void reset()
        {
            this.value = null;
            this.stringValue = null;
            this.intValue = 0;
            this.boolValue = false;
            this.vector2Value = default(Vector2);
            this.dictionaryValue = null;
            this.componentValue = null;
            this.type = default(System.Type);
        }

        public void setValue(string value)
        {
            reset();
            this.value = value;
            this.stringValue = value;
            this.type = typeof(string);
        }

        public void setValue(int value)
        {
            reset();
            this.value = value;
            this.intValue = value;
            this.type = typeof(int);
        }

        public void setValue(bool value)
        {
            reset();
            this.value = value;
            this.boolValue = value;
            this.type = typeof(bool);
        }

        public void setValue(Vector2 value)
        {
            reset();
            this.vector2Value = value;
            this.type = typeof(Vector2);
        }

        public void setValue(Dictionary<string, Prop> value)
        {
            reset();
            this.dictionaryValue = value;
            this.type = typeof(Dictionary<string, Prop>);
        }

        public void setValue(Components.Component value)
        {
            reset();
            this.componentValue = value;
            this.type = typeof(Component);
        }

        /*
         * @getters
         */
        public string getString()
        {
            return this.stringValue;
        }

        public int getInt()
        {
            return this.intValue;
        }

        public bool getBool()
        {
            return this.boolValue;
        }

        public Vector2 getVector2()
        {
            return this.vector2Value;
        }

        public Dictionary<string, Prop> getDictionary()
        {
            return this.dictionaryValue;
        }

        public Components.Component getComponent()
        {
            return this.componentValue;
        }

        public System.Type getType()
        {
            return this.type;
        }
    }
}