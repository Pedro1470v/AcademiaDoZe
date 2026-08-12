//Pedro Vinicius
using System.Collections.Generic;
using System.Linq;

namespace AcademiaDoZe.Domain.Common
{
    internal class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;

        public IReadOnlyCollection<Notification> Notifications { get; }

        protected Result(
            bool isSuccess,
            IReadOnlyCollection<Notification> notifications)
        {
            IsSuccess = isSuccess;
            Notifications = notifications;
        }

        public static Result Success()
        {
            return new Result(
                true,
                new List<Notification>());
        }

        public static Result Failure(
            IEnumerable<Notification> notifications)
        {
            return new Result(
                false,
                notifications.ToList());
        }

        public static Result Failure(
            string property,
            string message)
        {
            return new Result(
                false,
                new List<Notification>
                {
                    new Notification(property, message)
                });
        }
    }

    internal class Result<T> : Result
    {
        public T Value { get; }

        private Result(
            bool isSuccess,
            T value,
            IReadOnlyCollection<Notification> notifications)
            : base(isSuccess, notifications)
        {
            Value = value;
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(
                true,
                value,
                new List<Notification>());
        }

        public static Result<T> Failure(
            IEnumerable<Notification> notifications)
        {
            return new Result<T>(
                false,
                default!,
                notifications.ToList());
        }

        public static Result<T> Failure(
            string property,
            string message)
        {
            return new Result<T>(
                false,
                default!,
                new List<Notification>
                {
                    new Notification(property, message)
                });
        }
    }
}