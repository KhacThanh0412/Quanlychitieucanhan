using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Helpers
{
    public static class ReactiveExtensions
    {
        public static T DisposeWith<T>(this T item, CompositeDisposable compositeDisposable)
            where T : IDisposable
        {
            if (compositeDisposable == null)
            {
                throw new ArgumentNullException(nameof(compositeDisposable));
            }

            compositeDisposable.Add(item);
            return item;
        }

        public static void Renew(this CompositeDisposable compositeDisposable)
        {
            if (compositeDisposable == null)
            {
                throw new ArgumentNullException(nameof(compositeDisposable));
            }

            foreach (var dis in compositeDisposable)
            {
                dis.Dispose();
            }

            compositeDisposable.Clear();
        }

        public static IObservable<T> DebounceTime<T>(this IObservable<T> source, TimeSpan timespan)
        {
            return new DebounceOperator<T>(source, timespan);
        }

        /// <summary>
        /// Emit the previous and current values as an array.
        /// https://www.learnrxjs.io/operators/combination/pairwise.html
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IObservable<PairwiseProtype<TSource>> Pairwise<TSource>(this IObservable<TSource> source)
        {
            return source.Scan(
                new PairwiseProtype<TSource>(),
                (acc, current) => new PairwiseProtype<TSource>
                {
                    Current = current,
                    Previous = acc.Current
                });
        }
    }

    public class PairwiseProtype<T>
    {
        public T Current { get; set; }
        public T Previous { get; set; }
    }

    public sealed class DebounceOperator<T> : IObservable<T>
    {
        readonly IObservable<T> _source;
        readonly TimeSpan _timespan;
        internal DebounceOperator(IObservable<T> source, TimeSpan timespan)
        {
            _source = source;
            _timespan = timespan;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var parent = new DebounceHandler(observer, _timespan);
            var d = _source.Subscribe(parent);
            parent.OnSubscribe(d);
            return d;
        }

        sealed class DebounceHandler : IDisposable, IObserver<T>
        {
            readonly IObserver<T> _downstream;
            readonly TimeSpan _timespan;
            IDisposable _upstream;

            private TaskCompletionSource<bool> _tcs;

            internal DebounceHandler(IObserver<T> downstream, TimeSpan timespan)
            {
                _downstream = downstream;
                _timespan = timespan;
            }

            public void OnSubscribe(IDisposable d)
            {
                if (Interlocked.CompareExchange(ref _upstream, d, null) != null)
                {
                    d.Dispose();
                }
            }

            public void Dispose()
            {
                var d = Interlocked.Exchange(ref _upstream, this);
                if (d != null && d != this)
                {
                    d.Dispose();
                }

                if (_tcs != null && !_tcs.Task.IsCompleted)
                {
                    _tcs.SetResult(false);
                }
            }

            public void OnCompleted()
            {
                if (_tcs != null && !_tcs.Task.IsCompleted)
                {
                    _tcs.SetResult(false);
                }

                _downstream.OnCompleted();
            }

            public void OnError(Exception error)
            {
                if (_tcs != null && !_tcs.Task.IsCompleted)
                {
                    _tcs.SetResult(false);
                }

                _downstream.OnError(error);
            }

            public void OnNext(T value)
            {
                if (_tcs != null && !_tcs.Task.IsCompleted)
                {
                    _tcs.SetResult(false);
                }

                AuditAsync(value).ConfigureAwait(false);
            }

            private async Task AuditAsync(T value)
            {
                var localTcs = _tcs = new TaskCompletionSource<bool>();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(async () =>
                {
                    await Task.Delay(_timespan).ConfigureAwait(false);
                    if (!localTcs.Task.IsCompleted)
                    {
                        if (_tcs == localTcs)
                        {
                            localTcs.SetResult(true);
                        }
                        else
                        {
                            localTcs.SetResult(false);
                        }
                    }
                }).ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                try
                {
                    var valid = await localTcs.Task.ConfigureAwait(false);
                    if (valid)
                    {
                        _downstream.OnNext(value);
                    }
                }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
                catch
                {
                }
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
                if (localTcs == _tcs)
                {
                    _tcs = null;
                }
            }
        }
    }
}
