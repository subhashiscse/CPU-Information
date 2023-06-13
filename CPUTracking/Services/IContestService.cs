using System;
using CPUTracking.Models.Create;

namespace CPUTracking.Services
{
	public interface IContestService
	{
        List<Contest> GetAllContestList();
    }
}

