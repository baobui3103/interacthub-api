using InteractHub.Application.Core.Models;

namespace InteractHub.Application.Core.Services
{
    public interface IEmailService
    {
        void SendEmail(Email email);
    }
}
