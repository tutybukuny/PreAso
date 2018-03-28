using System.Reflection;
using log4net;

namespace PreAsso
{
    public class Logger
    {
        public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}