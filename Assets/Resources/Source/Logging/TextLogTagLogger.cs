using UnityEngine;
using UnityEngine.UI;

namespace Logging
{
    class TextLogTagLogger : ILogger
    {
        private readonly Text _loggerObject;

        public TextLogTagLogger()
        {
            _loggerObject = GameObject.Find("Logger").GetComponent<Text>();
        }

        public void Log(string s)
        {
            _loggerObject.text = s;
        }
    }
}
