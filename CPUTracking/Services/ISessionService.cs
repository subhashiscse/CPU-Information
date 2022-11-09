using CPUTracking.Models.Create;

namespace CPUTracking.Services
{
    public interface ISessionService
    {
        List<Session> GetAllSessionList();
    }
}
