using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using System.Reflection;
using UnityEditor;

namespace EasyDebug
{
    public class PipeConsoleEntity
    {
        public string value;
        private object[] inputs;
        public SeverityTag tag = SeverityTag.Info;

        public Action<string, UnityEngine.Object> logFunction
        {
            get
            {
                switch (tag)
                {
                    case SeverityTag.Info:    return UnityEngine.Debug.Log;
                    case SeverityTag.Debug:   return UnityEngine.Debug.Log;
                    case SeverityTag.Warning: return UnityEngine.Debug.LogWarning;
                    case SeverityTag.Error:   return UnityEngine.Debug.LogError;
                }
                return null;
            }
        }

        public PipeConsoleEntity(object[] _objects)
        {
            this.inputs = _objects;
        }

        /// <summary>
        /// Parses 'objects' into 'value' with 'QDebug.defaultParser'
        /// and then applies format to it.
        /// </summary>
        /// <param name="separator">Optional custom separator</param>
        /// <returns>Returns this</returns>
        public PipeConsoleEntity Parse(string separator = null)
        {
            value = PipeConsole.defaultParser(inputs, separator != null ? separator : PipeConsole.defaultSeparator);
            value = PipeConsole.formatFunction(this);
            return this;
        }

        /// <summary>
        /// Parses 'objects' into 'value' with 'l:parser'
        /// and then applies format to it.
        /// </summary>
        /// <param name="parser">Custom parsing function</param>
        /// <param name="separator">Optional custom separator</param>
        /// <returns>Returns this</returns>
        public PipeConsoleEntity Parse(Func<object[], string, string> parser, string separator = null)
        {
            value = parser(inputs, separator != null ? separator : PipeConsole.defaultSeparator);
            value = PipeConsole.formatFunction(this);
            return this;
        }
        /// <summary>
        /// Parses 'objects' into 'value' with 'l:parser'
        /// and then applies format to it.
        /// </summary>
        /// <param name="parser">Custom parsing function type</param>
        /// <param name="separator">Optional custom separator</param>
        /// <returns>Returns this</returns>
        public PipeConsoleEntity Parse(Parser parser, string separator = null)
        {
            value = PipeConsole.FindParser(parser)(inputs, separator != null ? separator : PipeConsole.defaultSeparator);
            value = PipeConsole.formatFunction(this);
            return this;
        }

        /// <summary>
        /// Sets a tag on the Entity.
        /// </summary>
        public PipeConsoleEntity Tag(SeverityTag tag)
        {
            this.tag = tag;
            return this;
        }

        /// <summary>
        /// Prints value in Unity console.
        /// Doesn't print if tag is not allowed.
        /// </summary>
        /// <returns>Returns true if printed successfully; otherwise false</returns>
        public bool Print(UnityEngine.Object target = null)
        {
            if (!PipeConsole.severityLevel.HasFlag(tag)) return false;

            logFunction(value, target);
            return true;
        }

        /// <summary>
        /// Wraps Entity's value with <color> tag
        /// </summary>
        public PipeConsoleEntity Colorify(string color)
        {
            this.value = $"<color={color}>{value}</color>";
            return this;
        }

        public override bool Equals(object obj)
        {
            return obj is PipeConsoleEntity entity &&
                   value == entity.value &&
                   tag == entity.tag;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(value, tag);
        }

        public static bool operator ==(PipeConsoleEntity a, PipeConsoleEntity b)
        {
            return a.value == b.value && a.tag == b.tag;
        }
        public static bool operator !=(PipeConsoleEntity a, PipeConsoleEntity b)
        {
            return !(a == b);
        }
    }

    public static class PipeConsole
    {
        public static bool serialize = true;
        public static string defaultSeparator = " ";

        public static SeverityTag severityLevel = SeverityTag.Info | SeverityTag.Warning | SeverityTag.Error | SeverityTag.Debug;

        /// <summary>
        /// Function delegate that controls the way of global
        /// output formatting. Can be overriten by user.
        /// </summary>
        public static Func<PipeConsoleEntity, string> formatFunction = Format;

        /// <summary>
        /// Controls global output formatting.
        /// Returns Entity.value by default.
        /// Fullfills the default value of 'formatFunction', that actually controls the formatting.
        /// </summary>
        /// <param name="entity">The Entity to format</param>
        /// <returns>Single formatted string</returns>
        public static string Format(PipeConsoleEntity entity)
        {
            return entity.value;
        }

        /// <summary>
        /// Input values ; separator ; output string
        /// </summary>
        public static Func<object[], string, string> defaultParser = HarshParse;

        /// <summary>
        /// Provides parser function by the type
        /// </summary>
        /// <param name="parser">Parser algorithm type</param>
        /// <returns>Parser algorithm delegate</returns>
        public static Func<object[], string, string> FindParser(Parser parser)
        {
            switch (parser)
            {
                case Parser.Harsh: return HarshParse;
                case Parser.Deep: return DeepParse;
            }
            return null;
        }

        /// <summary>
        /// Sets the parser function to the FindParser(parser)
        /// </summary>
        public static void SetParser(Parser parser)
        {
            defaultParser = FindParser(parser);
        }
        /// <summary>
        /// Sets the parser function to the parser
        /// </summary>
        public static void SetParser(Func<object[], string, string> parser)
        {
            defaultParser = parser;
        }

        /// <summary>
        /// Parses input objects into a single string using The Harsh Algorithm (THA).
        /// Converts each object into a string via .ToString(); separates with separator
        /// if possible or defaultSeparator if l:separator is null.
        /// </summary>
        /// <param name="objects">objects to combine</param>
        /// <param name="separator">separates parsed objects</param>
        /// <returns>Single string</returns>
        public static string HarshParse(object[] objects, string separator = null)
        {
            return string.Join(separator != null ? separator : PipeConsole.defaultSeparator, objects);
        }

        /// <summary>
        /// Parses input objects into a single string using The Deep Algorithm (TDA).
        /// Converts objects to strings via .ToString() but keeping IEnumerables serialized properly.
        /// </summary>
        /// <param name="objects">objects to combine</param>
        /// <param name="separator">separates parsed objects. Does NOT separate IEnumerables' content.</param>
        /// <returns>Single string</returns>
        public static string DeepParse(object[] objects, string separator = null)
        {
            return string.Join(separator != null ? separator : PipeConsole.defaultSeparator, objects.Select(x => x is IEnumerable i && !(x is string) ? string.Join(separator, i.CastToStrings()) : x.ToString()));
        }

        /// <summary>
        /// Commits a new Entity that handles printing.
        /// </summary>
        /// <param name="values">Objects to commit</param>
        /// <returns>Entity to pipe</returns>
        public static PipeConsoleEntity Commit(params object[] values)
        {
            return new PipeConsoleEntity(values);
        }
        /// <summary>
        /// Represents function chain 'Commit(values).Parse().Do()'
        /// The short form of simple chain. Doesn't pipe.
        /// </summary>
        /// <param name="values">Objects to commit</param>
        public static void Print(params object[] values)
        {
            Commit(values).Parse().Print();
        }

        /// <summary>
        /// Sets default float numbers' string representation to
        /// dividing with dot (.) instead of comma (,). Absorbs any string.
        /// </summary>
        /// <param name="divider">The float divider string</param>
        public static void SetFloatDivider(string divider = ".")
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = divider;
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        }

        /// <summary>
        /// Clears Unity console.
        /// </summary>
        public static void ClearConsole()
        {
#if UNITY_EDITOR
            var assembly = Assembly.GetAssembly(typeof(SceneView));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
#endif
        }
    }

    public enum Parser
    {
        Harsh,
        Deep
    }

    public enum SeverityTag
    {
        Info = 1,
        Warning = 2,
        Error = 4,
        Debug = 8
    }
}

public static class EasyDebugExtensions
{
    public static IEnumerable<string> CastToStrings(this IEnumerable ienumerable)
    {
        foreach (var obj in ienumerable)
            yield return obj.ToString();
    }

    /// <summary>
    /// Wraps string with <color> tag
    /// </summary>
    public static string Colorify(this string target, string color)
    {
        return $"<color={color}>{target}</color>";
    }
}
