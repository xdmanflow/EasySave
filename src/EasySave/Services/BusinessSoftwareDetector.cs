using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace EasySave.Services
{
    public static class BusinessSoftwareDetector
    {
        public static bool IsRunning(IEnumerable<string> softwareList)
        {
            foreach (string name in softwareList)
            {
                string procName = Path.GetFileNameWithoutExtension(name);
                if (Process.GetProcessesByName(procName).Length > 0) return true;
            }
            return false;
        }
    }
}