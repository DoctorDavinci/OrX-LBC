using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OrX
{
    internal static class BDArmoryExtensions
    {
        internal static Type PartExtensions;
        private static bool isInstalled;

        static BDArmoryExtensions()
        {
            try
            {
                PartExtensions = AssemblyLoader.loadedAssemblies
                     .Where(a => a.name.Contains("BDArmory.Core")).SelectMany(a => a.assembly.GetExportedTypes())
                     .SingleOrDefault(t => t.FullName == "BDArmory.Core.Extension.PartExtensions");
                isInstalled = true;
            }
            catch (Exception e)
            {
                isInstalled = false;
            }
        }


        internal static bool BDArmoryIsInstalled()
        {
            return isInstalled;
        }
    }
}
