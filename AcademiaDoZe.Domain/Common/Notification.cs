//Pedro Vinicius
namespace AcademiaDoZe.Domain.Common
{
    internal class Notification
    {
        public string Property { get; }
        public string Message { get; }

        public Notification(
            string property,
            string message)
        {
            Property = property;
            Message = message;
        }

        public override string ToString()
        {
            return $"{Property}: {Message}";
        }
    }
}