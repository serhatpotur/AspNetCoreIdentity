namespace AspNetCoreIdentityApp.Business.Services
{
    public interface IEmailService
    {
        Task SendResetPasswordEmail(string resetEmailLink,string toUserEmail);
    }
}
