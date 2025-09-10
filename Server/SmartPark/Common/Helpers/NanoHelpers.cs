using System.ComponentModel;
using System.Reflection;

namespace SmartPark.Common.Helpers
{
    internal static class NanoHelpers
    {
        // helper method to create and get enum description values instead of its original values.
        public static string GetEnumDescription(System.Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            DescriptionAttribute attribute =
                Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                as DescriptionAttribute;

            return attribute?.Description ?? value.ToString();
        }
    }
}