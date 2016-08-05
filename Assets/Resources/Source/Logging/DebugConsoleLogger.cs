using UnityEngine;

namespace Logging
{
    class DebugConsoleLogger : ILogger
    {
        public void Log(string s)
        {
            Debug.Log(s);
        }
    }
}
