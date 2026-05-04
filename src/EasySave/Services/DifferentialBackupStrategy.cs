using System.IO;
using System.Linq;

namespace EasySave.Services
{
    public class DifferentialBackupStrategy : BackupStrategyBase
    {
        protected override string[] SelectFiles(string[] allFiles, string sourceDir, string targetDir)
        {
            return allFiles.Where(src =>
            {
                string tgt = Path.Combine(targetDir, Path.GetRelativePath(sourceDir, src));
                return !File.Exists(tgt) || File.GetLastWriteTimeUtc(src) > File.GetLastWriteTimeUtc(tgt);
            }).ToArray();
        }
    }
}