using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

public static class Extensions
{
    public static void AppendLine(this RichTextBox thisRtxt, Exception ex)
    {
        thisRtxt.AppendText(ex.ToString() + Environment.NewLine);
    }
    public static void AppendLine(this RichTextBox thisRtxt, string text)
    {
        thisRtxt.AppendText(text + Environment.NewLine);
    }
    public static void AppendCharArrayAsHex(this RichTextBox thisRtxt, char[] items)
    {
        string hexStr = "";
        for (int i = 0; i < items.Length; i++)
        {
            int item = items[i];
            hexStr += item.ToString("X2");
            if (i < items.Length - 1) hexStr += ", ";
        }
        thisRtxt.AppendLine(hexStr);
    }

    public static string GetAllToStrings<T>(this T[] thisObj)
    {
        if (thisObj == null) return "";
        string r = "";
        for (int i = 0; i < thisObj.Length; i++)
        {
            if (thisObj[i] == null) continue;
            r += thisObj[i].ToString() + Environment.NewLine;
        }
        return r;
    }

    public static string[] GetAllToStringsAsArray<T>(this T[] thisObj, bool showIndices = false)
    {
        if (thisObj == null) return new string[0];
        string[] items = new string[thisObj.Length];
        for (int i = 0; i < thisObj.Length; i++)
        {
            if (thisObj[i] == null) { items[i] = ""; continue; }
            items[i] = (showIndices?i.ToString().PadLeft(5)+" ":"") + thisObj[i].ToString();
        }
        return items;
    }
    /*
    public static int IndexOf<T>(this T[] thisCharArray, T val)
    {
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        for (int i = 0; i < array.Length; i++)
        {
            if (comparer.Equals(array[i], value))
                return i;
        }
        return -1;
    }
    */

    public static int IndexOf(this char[] thisCharArray, char val)
    {
        for (int i = 0; i < thisCharArray.Length; i++)
            if (thisCharArray[i] == val) return i;
        return -1;
    }

    public static int IndexOf(this byte[] thisByteArray, byte val)
    {
        for (int i = 0; i < thisByteArray.Length; i++)
            if (thisByteArray[i] == val) return i;
        return -1;
    }

    public static string ToString(this byte[] thisByteArray, bool padValues)
    {
        string r = "[ ";
        for (int i = 0; i < thisByteArray.Length; i++)
        {
            if (padValues)
                r += thisByteArray[i].ToString().PadRight(3);
            else
                r += thisByteArray[i].ToString();
            if (i < thisByteArray.Length - 1) r += ", ";
        }
        return r + " ]";
    }

}