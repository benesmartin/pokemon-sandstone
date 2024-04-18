using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringExtensions
{
    public static string SetColor(this string message, Color color)
    {
        string hexColor = ColorUtility.ToHtmlStringRGB(color);
        return $"<color=#{hexColor}>{message}</color>";
    }

    public static string SetColor(this string message, string color)
    {
        if (!color.StartsWith("#"))
        {
            return $"<color={color}>{message}</color>";
        }
        else
        {
            return $"<color={color}>{message}</color>";
        }
    }
    public static string SetBold(this string message)
    {
        return $"<b>{message}</b>";
    }
}
