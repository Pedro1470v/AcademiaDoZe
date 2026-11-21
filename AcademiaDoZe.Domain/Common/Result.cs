// Pedro Vinicius

using System.Collections.Generic;

namespace AcademiaDoZe.Domain.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public T? Value { get; }

        public List<Notification> Notifications { get; }

        private Result(
            bool isSuccess,
            T? value,
            List<Notification> notifications)
        {
            IsSuccess = isSuccess;
            Value = value;
            Notifications = notifications;
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(
                true,
                value,
                new List<Notification>());
        }

        public static Result<T> Failure(
            List<Notification> notifications)
        {
            return new Result<T>(
                false,
                default,
                notifications);
        }

        public static Result<T> Failure(
            string property,
            string mensagem)
        {
            var notifications = new List<Notification>
            {
                new Notification(property, mensagem)
            };

            return new Result<T>(
                false,
                default,
                notifications);
        }
    }
}