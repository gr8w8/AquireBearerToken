using DotNetEnv;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AquireBearerToken
{
    public class Utilities
    {
        public static void ReadEnvFile()
        {
            var envFile = Env.Load();
            if (!envFile.Any())
            {
                envFile = Env.TraversePath().Load();
            }
            return;
        }
    }
}
