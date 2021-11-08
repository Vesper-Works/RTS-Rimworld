using System;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// A static class for reflection type functions
/// </summary>
public static class Reflection
{
    /// <summary>
    /// Extension for 'Object' that copies the properties to a destination object. Courtesy of https://stackoverflow.com/questions/930433/apply-properties-values-from-one-object-to-another-of-the-same-type-automaticall
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="destination">The destination.</param>
    public static void CopyProperties(this object source, object destination)
    {
        // If any this null throw an exception
        if (source == null || destination == null)
            throw new Exception("Source or/and Destination Objects are null");
        // Getting the Types of the objects
        Type typeDest = destination.GetType();
        Type typeSrc = source.GetType();

        // Iterate the Properties of the source instance and  
        // populate them from their desination counterparts  
        PropertyInfo[] srcProps = typeSrc.GetProperties();
        foreach (PropertyInfo srcProp in srcProps)
        {
            if (!srcProp.CanRead)
            {
                continue;
            }
            if (srcProp.DeclaringType.Namespace == "UnityEngine")
            {
                continue;
            }
            PropertyInfo targetProperty = typeDest.GetProperty(srcProp.Name);
            if (targetProperty == null)
            {
                continue;
            }
            if (!targetProperty.CanWrite)
            {
                continue;
            }
            if (targetProperty.GetSetMethod(true) != null && targetProperty.GetSetMethod(true).IsPrivate)
            {
                continue;
            }
            if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
            {
                continue;
            }
            if (!targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType))
            {
                continue;
            }
            // Passed all tests, lets set the value
            targetProperty.SetValue(destination, srcProp.GetValue(source, null),null);
        }
    }

    public static void SetPropertyOfName(this object source, string value, string propertyName)
    {
        try
        {
            Type type = source.GetType();
            Type typeDest = value.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyName);
            if (propertyInfo.PropertyType == typeof(Color))
            {
                string[] colorValues = value.Split(',');
                Color color = new Color(int.Parse(colorValues[0]), int.Parse(colorValues[1]), int.Parse(colorValues[2]));
                propertyInfo.SetValue(source, color);
            }
            else if (propertyInfo.PropertyType.IsEnum)
            {
                propertyInfo.SetValue(source, Enum.Parse(propertyInfo.PropertyType, value));
            }
            else
            {
                propertyInfo.SetValue(source, Convert.ChangeType(value, propertyInfo.PropertyType));
            }
        
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to set property: " + propertyName + ". Source: " + source + ", Value: " + value + Environment.NewLine + e.Message);
        }

    }   
    
    public static void SetReferenceDictionaryOfName(this object source, Dictionary<string, int> value, string propertyName)
    {
        try
        {
            Type type = source.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyName);
            propertyInfo.SetValue(source, value, null);       

        }
        catch (Exception e)
        {
            Debug.LogError("Failed to set property: " + propertyName + "  :  " + e);
        }

    }

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
}