using System.Runtime.CompilerServices;

namespace CargoEngine.Exception {
    public class CargoEngineException : System.Exception {
        protected CargoEngineException(string message) : base(message)
        {
        }

        protected CargoEngineException(string message, System.Exception baseException) : base(message,baseException)
        {
        }

        protected CargoEngineException() : base()
        {
        }

        public static CargoEngineException Create(string message,
            System.Exception baseException,
            [CallerMemberName] string methodName = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string file = "") {
            file = file.Substring(file.LastIndexOf("\\") + 1);
            return new CargoEngineException(message + "\n" + file + "(" + lineNumber + "):" + methodName + "\n" + baseException.Message, baseException);

        }
        public static CargoEngineException Create(string message,
            [CallerMemberName] string methodName = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string file = "") {
            return CargoEngineException.Create(message, null, methodName, lineNumber, file);
        }
    }
}
