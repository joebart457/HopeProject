using Hope.Compiler.Models.Enums;
using System.Reflection;

namespace Hope.Compiler._Parser
{
    internal class ArgumentRule
    {
        public ArgumentType Type { get; set; }
        public PropertyInfo Property { get; set; }

        public ArgumentRule(ArgumentType type, PropertyInfo property)
        {
            Type = type;
            Property = property;
        }

        public void FillProperty(object instance, object value)
        {
            Property.SetValue(instance, value);
        }
    }
}
