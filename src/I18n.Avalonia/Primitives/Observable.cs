using System;
using System.Collections.Generic;

namespace I18n.Avalonia.Primitives;

internal class Observable<T>(T value) : IObservable<T?>, IDisposable
{
    private readonly object _gate = new();

    private bool _isDisposed;
    private T? _value = value;

    private IList<WeakReference<IObserver<T?>>> _weakReferencesObservers = [];

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
            _weakReferencesObservers = [];
            _value = default;
        }
    }

    public IDisposable Subscribe(IObserver<T?> observer)
    {
        if (observer is null) throw new ArgumentNullException(nameof(observer));

        lock (_gate)
        {
            CheckDisposed();
            // 修改为弱引用
            var weakReference = new WeakReference<IObserver<T?>>(observer);
            _weakReferencesObservers.Add(weakReference);
            observer.OnNext(_value);
            return new Subscription(this, weakReference);
        }
    }

    private void CheckDisposed()
    {
        if (_isDisposed) throw new ObjectDisposedException($"{nameof(I18nUnit)} already disposed");
    }

    public void OnNext(T? value)
    {
        IList<WeakReference<IObserver<T?>>>? observerArray;
        lock (_gate)
        {
            CheckDisposed();
            _value = value;
            observerArray = _weakReferencesObservers;
        }

        foreach (var weakReference in observerArray)
            if (weakReference.TryGetTarget(out var observer))
                observer.OnNext(value);
    }


    private void Unsubscribe(WeakReference<IObserver<T?>> weakReference)
    {
        lock (_gate)
        {
            CheckDisposed();
            _weakReferencesObservers.Remove(weakReference);
        }
    }

    private sealed class Subscription(Observable<T> subject, WeakReference<IObserver<T?>> weakReference) : IDisposable
    {
        public void Dispose()
        {
            subject.Unsubscribe(weakReference);
        }
    }
}
