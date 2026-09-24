// Pedro Vinicius

namespace AcademiaDoZe.Domain.Common
{
    public class Notification
    {
        public string Property { get; }
        public string Mensagem { get; }

        public Notification(string property, string mensagem)
        {
            Property = property;
            Mensagem = mensagem;
        }
    }
}