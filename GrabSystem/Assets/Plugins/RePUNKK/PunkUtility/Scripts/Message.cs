using System.Runtime.CompilerServices;
using UnityEngine;
using System.IO;

namespace RePunkk.ReUtility
{
    public static class Message
    {
        private static Object GetCallingContext()
        {
            var stackTrace = new System.Diagnostics.StackTrace(true);

            for (int i = 2; i < stackTrace.FrameCount; i++)
            {
                var frame = stackTrace.GetFrame(i);
                var method = frame.GetMethod();

                if (method.DeclaringType != null && typeof(MonoBehaviour).IsAssignableFrom(method.DeclaringType))
                {
                    var instances = Object.FindObjectsOfType(method.DeclaringType);
                    if (instances.Length > 0) return instances[0] as Object;
                }
            }

            return null;
        }

        private static string GetGameObjectName(Object context)
        {
            if (context is MonoBehaviour mb) return mb.gameObject.name;
            if (context is GameObject go) return go.name;
            return "Unknown";
        }

        public static void Log(string message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            var context = GetCallingContext();
            string objectName = context != null ? GetGameObjectName(context) : Path.GetFileNameWithoutExtension(filePath);

            Debug.LogFormat(context, $"[{objectName}:{lineNumber}] {message}");
        }

        public static void Log(Color color, string message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            var context = GetCallingContext();
            string objectName = context != null ? GetGameObjectName(context) : Path.GetFileNameWithoutExtension(filePath);

            Debug.LogFormat(context, $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>[{objectName}:{lineNumber}] {message}</color>");
        }

        public static void Log(bool debug, string message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (debug)
            {
                var context = GetCallingContext();
                string objectName = context != null ? GetGameObjectName(context) : Path.GetFileNameWithoutExtension(filePath);

                Debug.LogFormat(context, $"[{objectName}:{lineNumber}] {message}");
            }
        }

        public static void Log(bool debug, Color color, string message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (debug)
            {
                var context = GetCallingContext();
                string objectName = context != null ? GetGameObjectName(context) : Path.GetFileNameWithoutExtension(filePath);

                Debug.LogFormat(context, $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>[{objectName}:{lineNumber}] {message}</color>");
            }
        }
    }
}