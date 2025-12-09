using UnityEngine;

namespace RePunkk.ReTools
{
    public class HideValueAttribute : PropertyAttribute
    {
        public string conditionalSourceField;
        public bool hideValue;

        public HideValueAttribute(string conditionalSourceField, bool hideValue = true)
        {
            this.conditionalSourceField = conditionalSourceField;
            this.hideValue = hideValue;
        }
    }
}