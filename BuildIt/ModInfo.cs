using ICities;
using System.Reflection;

namespace BuildIt
{
    public class ModInfo : IUserMod
    {
        public string Name => "Build It! " + Version;
        public string Description => "Generate a city from a openstreet map";

        public static string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
    }
}
