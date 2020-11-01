using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Slot.BackOffice.Data.History
{
    public class BaseRepository
    {
        protected readonly static Assembly executingAssembly;
        protected readonly static string[] manifestResourceNames;

        static BaseRepository()
        {
            executingAssembly = Assembly.GetExecutingAssembly();
            manifestResourceNames = executingAssembly.GetManifestResourceNames();
        }
    }
}
