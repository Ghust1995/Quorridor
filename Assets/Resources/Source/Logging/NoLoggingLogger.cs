using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logging
{
    class NoLoggingLogger : ILogger
    {

        public void Log(string s)
        {
            return;
        }
    }
}
