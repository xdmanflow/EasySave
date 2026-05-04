namespace EasySave.Services
{
    
    public class FullBackupStrategy : BackupStrategyBase
    {
        protected override string[] SelectFiles(string[] allFiles, string sourceDir, string targetDir)
            => allFiles;
    }
}