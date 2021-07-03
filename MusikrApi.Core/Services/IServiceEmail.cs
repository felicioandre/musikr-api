namespace MusikrApi.Core.Services
{
    public interface IServiceEmail
    {
        void EnviaEmail(string email, string assunto, string body);
    }
}
