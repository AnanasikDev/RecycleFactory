using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace EasyDebug
{
    public enum ConsoleCommandType
    {
        /// <summary>
        /// Command can be identified and called using only name
        /// </summary>
        Global,

        /// <summary>
        /// Command can be identified and called using gameobject's name and name of the command
        /// </summary>
        ObjectRelative
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class Command : Attribute
    {
        public string name { get; private set; }
        public ConsoleCommandType type { get; private set; }
        // TODO: add optional custom prefix (i.e. player, time, etc ~ alias for gameobject name)

        public Command(string name, ConsoleCommandType type)
        {
            this.name = name;
            this.type = type;
        }
    }

    internal class CommandInfo
    {
        /// <summary>
        /// Object that the command is being applied to. Null if global/unspecified
        /// </summary>
        public string objectName = "";

        /// <summary>
        /// Name of the function/method being called
        /// </summary>
        public string functionName = "";

        /// <summary>
        /// Arguments passed through to the command
        /// </summary>
        public object[] args;

        public static CommandInfo Empty { get { return new CommandInfo(); } }

        public override bool Equals(object obj)
        {
            return obj is CommandInfo info &&
                   objectName == info.objectName &&
                   functionName == info.functionName &&
                   EqualityComparer<object[]>.Default.Equals(args, info.args);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(objectName, functionName, args);
        }

        public static bool operator ==(CommandInfo a, CommandInfo b)
        {
            return a.objectName == b.objectName && a.functionName == b.functionName && (a.args == null || b.args == null || a.args.SequenceEqual(b.args));
        }
        public static bool operator !=(CommandInfo a, CommandInfo b)
        {
            return !(a == b);
        }
    }

    public class CommandLineEngine
    {
        public BindingFlags access = BindingFlags.Public |
                                     BindingFlags.NonPublic |
                                     BindingFlags.Static |
                                     BindingFlags.Instance;

        List<MethodInfo> methods = new List<MethodInfo>();
        List<Command> commands = new List<Command>();

        public void Init()
        {
            methods = Assembly.GetExecutingAssembly()
                .GetTypes()
                .SelectMany(t => t.GetMethods(access))
                .Where(m => m.GetCustomAttributes<Command>().Any())
                .ToList();

            commands = methods.SelectMany(m => m.GetCustomAttributes<Command>()).ToList();

            Debug.Log("Found " + methods.Count + " commands available:");

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<Command>();
                Debug.Log("Found command: " + method.Name + " with return type = " + method.ReturnType + "; Attribute name is " + attr.name + " of length = " + attr.name.Length);
            }
        }

        /// <summary>
        /// Searches for a component on the GameObject with the specified method and invokes it if found.
        /// </summary>
        public void TryInvokeMethodOnGameObject(GameObject gameObject, MethodInfo methodInfo)
        {
            // Get the name of the method we are looking for
            string methodName = methodInfo.Name;

            // Loop through each component attached to the GameObject
            foreach (var component in gameObject.GetComponents<MonoBehaviour>())
            {
                // Check if this component has the specified method
                var componentMethod = component.GetType().GetMethod(methodName, access);
                if (componentMethod != null && componentMethod == methodInfo)
                {
                    // Invoke the method on the component
                    componentMethod.Invoke(component, null); // Pass parameters here if required
                    Debug.Log($"Method '{methodName}' invoked on component '{component.GetType().Name}' attached to '{gameObject.name}'");
                    return;
                }
            }

            Debug.LogWarning($"No component with method '{methodName}' found on GameObject '{gameObject.name}'");
        }

        /// <summary>
        /// Finds all commands which names start with the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix to search for in the command name.</param>
        public List<Command> GetCommandsStartingWith(string prefix)
        {
            var matchingCommands = new List<Command>();

            foreach (var command in commands)
            {
                if (command != null && command.name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    matchingCommands.Add(command);
                }
            }

            return matchingCommands;
        }

        /// <summary>
        /// Format of a query must be as following (objectName.functionName arg1 arg2 arg...)
        /// objectName can be omitted if command is declared as global (.functionName arg1 arg2 arg...)
        /// args are optional in any case.
        /// </summary>
        /// <param name="query">Completed or non-completed input query from the command line</param>
        /// <returns>Deformatted string into a temporary CommandInfo without connection to real commands</returns>
        internal CommandInfo ParseInput(string query)
        {
            if (query == string.Empty) return CommandInfo.Empty;
            if (query.Contains('.') == false)
            {
                Debug.LogWarning($"Command Line could not execute command ({query}) as it has no '.' sign in it");
                return CommandInfo.Empty;
            }

            CommandInfo result = new CommandInfo();

            result.objectName = query.Split('.')[0];
            //Debug.Log("EXECUTE: objectName = " + objectName + " of length = " + objectName.Length);

            result.functionName = result.objectName == string.Empty ? query.Split(" ")[0] : query.Split(".")[1].Split(" ")[0];
            result.functionName = result.functionName.Replace(".", "").Replace(" ", "");
            //Debug.Log("EXECUTE: commandName = " + commandName + " of length = " + commandName.Length);

            if (result.functionName == string.Empty)
            {
                Debug.LogError("Command name is empty");
                return CommandInfo.Empty;
            }

            return result;
        }

        public void Execute(string query)
        {
            CommandInfo commandInfo = ParseInput(query);
            if (commandInfo == CommandInfo.Empty)
            {
                //Debug.LogError("Command Line Query parse failed");
                return;
            }

            int index = commands.FindIndex(0, (Command c) => c.name == commandInfo.functionName);
            
            if (index == -1)
            {
                //Debug.LogError($"Command with name {commandInfo.functionName} could not be found");
                return;
            }
            
            if (commandInfo.objectName == string.Empty)
            {
                // if no name spacified, try find an object on scene which has that command implemented (with global tag on it)
                return;
            }
            var method = methods[index];
            GameObject obj = GameObject.Find(commandInfo.objectName);
            if (obj == null)
            {
                //Debug.LogError($"Object with name {commandInfo.objectName} not found on the current scene");
                return;
            }

            TryInvokeMethodOnGameObject(obj, method);
        }

        [Command("EngineFuncHEHE", ConsoleCommandType.ObjectRelative)]
        public void EngineFunc()
        {
            Debug.Log("Ok this one works noice");
        }
    }
}