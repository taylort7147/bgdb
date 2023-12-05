using System;
using System.Xml.Linq;

namespace BggExt.XmlApi2
{
    public static class XmlHelper
    {
        // Attributes
        public static string GetAttributeValue(this XElement? element, string attrName)
        {
            ArgumentNullException.ThrowIfNull(element);
            var attr = element.Attribute(attrName);
            ArgumentNullException.ThrowIfNull(attr);
            return attr.Value;
        }

        public static string GetAttributeValueOrDefault(this XElement? element, string attrName, string defaultValue)
        {
            ArgumentNullException.ThrowIfNull(element);
            var attr = element.Attribute(attrName);
            if (attr != null)
            {
                return attr.Value;
            }
            return defaultValue;
        }

        public static T GetAttributeValue<T>(this XElement? element, string attrName, IFormatProvider? formatProvider = null)
            where T : IParsable<T>
        {
            return T.Parse(element.GetAttributeValue(attrName), formatProvider);
        }

        public static T GetAttributeValueOrDefault<T>(this XElement? element, string attrName, T defaultValue, IFormatProvider? formatProvider = null)
            where T : IParsable<T>
        {
            if (element != null)
            {
                var attr = element.Attribute(attrName);
                if (attr != null)
                {
                    T? value = default;
                    if (T.TryParse(attr.Value, formatProvider, out value))
                    {
                        return value;
                    }
                }
            }
            return defaultValue;
        }
        
        // Elements
        public static string GetElementValue(this XElement? element)
        {
            ArgumentNullException.ThrowIfNull(element);
            return element.Value;
        }

        public static string GetElementValueOrDefault(this XElement? element, string defaultValue)
        {
            if (element != null)
            {
                return element.Value;
            }
            return defaultValue;
        }

        public static T GetElementValue<T>(this XElement? element, IFormatProvider? formatProvider = null)
            where T : IParsable<T>
        {
            ArgumentNullException.ThrowIfNull(element);
            return T.Parse(element.Value, formatProvider);

        }

        public static T GetElementValueOrDefault<T>(this XElement? element, T defaultValue, IFormatProvider? formatProvider = null)
            where T : IParsable<T>
        {
            if (element != null)
            {
                T? value;
                if (T.TryParse(element.Value, formatProvider, out value))
                {
                    return value;
                }
            }
            return defaultValue;
        }
    }
}