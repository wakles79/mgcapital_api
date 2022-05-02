using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
