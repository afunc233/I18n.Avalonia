using System;
using System.Collections.Generic;

namespace I18n.Avalonia;

public class I18nUnit
{
    private readonly Observable<string?> _value;

    public string? CurrentValue => _value.Value;
    
    public IObservable<string?> Value => _value;

    public I18nUnit(ITranslatorProvider translatorProvider, string key)
    {
        _value = new Observable<string?>(Next());

        I18nProvider.OnCultureChanged += (_, _) => { _value.OnNext(Next()); };
        return;

        string? Next()
        {
            return translatorProvider.GetString(key);
        }
    }

    private class Observable<T>(T value) : IObservable<T?>, IDisposable
    {
        private T? _value = value;

        private readonly object _gate = new();

        private IList<IObserver<T?>> _observers = [];

        private bool _isDisposed;

        public IDisposable Subscribe(IObserver<T?> observer)
        {
            if (observer is null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            lock (_gate)
            {
                CheckDisposed();
                _observers.Add(observer);
                observer.OnNext(_value);
                return new Subscription(this, observer);
            }
        }

        public T? Value
        {
            get
            {
                lock (_gate)
                {
                    CheckDisposed();
                    return _value;
                }
            }
        }

        public void Dispose()
        {
            lock (_gate)
            {
                _isDisposed = true;
                _observers = [];
                _value = default(T);
            }
        }

        private void CheckDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException($"{nameof(I18nUnit)} already disposed");
            }
        }

        public void OnNext(T? value)
        {
            IList<IObserver<T?>>? observerArray;
            lock (_gate)
            {
                CheckDisposed();
                _value = value;
                observerArray = _observers;
            }

            foreach (var observer in observerArray)
                observer.OnNext(value);
        }


        private void Unsubscribe(IObserver<T?> observer)
        {
            lock (_gate)
            {
                CheckDisposed();
                _observers.Remove(observer);
            }
        }

        private sealed class Subscription(Observable<T> subject, IObserver<T?>? observer) : IDisposable
        {
            public void Dispose()
            {
                if (observer is null) return;

                subject.Unsubscribe(observer);
            }
        }
    }
}
