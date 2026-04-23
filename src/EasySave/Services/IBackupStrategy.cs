using EasySave.Models;
using EasyLog;

namespace EasySave.Services
{
    public interface IBackupStrategy
    {
        void Execute(BackupJob job, DailyLogger dailyLogger, StateLogger stateLogger);
    }
}
