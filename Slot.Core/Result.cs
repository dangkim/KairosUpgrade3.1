using System;
using System.Collections.Generic;
using Unit = System.ValueTuple;


namespace Slot.Core
{
    using Slot.Core.Extensions;

    public struct Result<TResult, TError>
    {
        public TResult Value { get; }

        public TError Error { get; }

        public bool IsError { get; }

        internal Result(TResult value)
        {
            Value = value;
            Error = default(TError);
            IsError = false;
        }

        internal Result(TError error)
        {
            Value = default(TResult);
            Error = error;
            IsError = true;
        }

        public U Match<U>(Func<TResult, U> onSuccess, Func<TError, U> onError) => IsError ? onError(Error) : onSuccess(Value);

        public Unit Match(Action<TResult> onSuccess, Action<TError> onError) => Match(onSuccess.ToFunc(), onError.ToFunc());

        public IEnumerator<TResult> AsEnumerable()
        {
            if (!IsError) yield return Value;
        }

        public static implicit operator Result<TResult, TError>(TResult value) => new Result<TResult, TError>(value);
        public static implicit operator Result<TResult, TError>(TError error) => new Result<TResult, TError>(error);

        //public static implicit operator Result<TResult, TError>(Result.Ok<TResult> ok) => new Result<TResult, TError>(ok.Value);
        //public static implicit operator Result<TResult, TError>(Result.Error<TError> error) => new Result<TResult, TError>(error.Value);

    }

    //public static class Result
    //{
    //    public struct Ok<T>
    //    {
    //        internal T Value { get; }
    //        internal Ok(T value) { Value = value; }
    //    }

    //    public struct Error<T>
    //    {
    //        internal T Value { get; }
    //        internal Error(T value) { Value = value; }
    //    }
    //}

    //public static partial class ResultHelper
    //{
    //    public static Result.Ok<T> Ok<T>(T value) => new Result.Ok<T>(value);
    //    public static Result.Error<T> Error<T>(T value) => new Result.Error<T>(value);
    //}

    public static class ResultExtensions
    {
        public static Result<U, E> Bind<T, U, E>(this Result<T, E> m, Func<T, Result<U, E>> f)
            => m.Match(x => f(x), e => e);

        public static Result<U, E> Select<T, U, E>(this Result<T, E> m, Func<T, U> f)
            => m.Match(x => f(x), e => new Result<U, E>(e));

        public static Result<Unit, E> ForEach<T, E>(this Result<T, E> m, Action<T> f)
            => Select(m, f.ToFunc());

        public static Result<I, E> SelectMany<T, U, I, E>(this Result<T, E> m, Func<T, Result<U, E>> f, Func<T, U, I> project) =>
            m.Match(x => f(x).Match(x1 => project(x, x1),
                                    e1 => new Result<I, E>(e1)),
                    e => e);

        public static T OnError<T, E>(this Result<T, E> m, Func<E, T> onError) =>
            m.Match(x => x, e => onError(e));
    }
}
