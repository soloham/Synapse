using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synapse.Utilities.Extensions
{
    public static class ExtendedMethod
    {
        public static void Rename(this FileInfo fileInfo, string newName)
        {
            fileInfo.MoveTo(fileInfo.Directory.FullName + "\\" + newName);
        }
    }
}
