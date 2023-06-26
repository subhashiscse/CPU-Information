using System;
using CPUTracking.Models.ContestDTO;
using CPUTracking.Models.Create;

namespace CPUTracking.Services
{
	public interface IContestService
	{
        List<Contest> GetAllContestList();
        List<ContestScore> GenerateScoreForAllUser(DateTime FromDate);
        int CalculatePointUsingScore(int percentage);
        string checkContestPlatformName(string inputString);
    }
}

