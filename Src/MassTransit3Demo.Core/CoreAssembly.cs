using System.Reflection;

namespace MassTransit3Demo.Core
{
    public class CoreAssembly
    {
        public static Assembly Assembly => typeof(CoreAssembly).Assembly;
    }
}
