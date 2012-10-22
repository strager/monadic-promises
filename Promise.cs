class Promise<T> {
    private readonly Action<T> m;

    private Promise(Action<T> m) {
        this.m = m;
    }

    public static Promise<T> Pure(T x) {
        return IO(() => x);
    }

    public static Promise<T> IO(Action<T> m) {
        return new Promise(m);
    }

    public Promise<U> Bind<U>(Func<T, Promise<U>> f) {
        return new Promise<U>(() => {
            return f(this.Run()).Run();
        });
    }

    public T Run() {
        return m();
    }
}

class PromiseCore {
    // fmap :: (Functor f) => (a -> b) -> f a -> f b
    public static Promise<U> Fmap<T, U>(Promise<T> p, Func<T, U> f) {
        return Bind(p, (x) => Pure(f(x)));
    }

    // pure :: (Applicative f) => a -> f a
    public static Promise<T> Pure<T>(T x) {
        return Promise<T>.Pure(x);
    }

    // ap :: (Applicative f) => f (a -> b) -> f a -> f b
    public static Promise<U> Ap<T, U>(Promise<Func<T, U>> f, Promise<T> p) {
        return LiftM2((x, y) => x(y), f, p);
    }

    // bind :: (Monad m) => m a -> (a -> m b) -> m b
    public static Promise<U> Bind<T, U>(Promise<T> p, Func<T, Promise<U>> f) {
        return p.Bind(f);
    }

    // io :: IO a -> Promise a
    public static Promise<T> IO<T>(Action<T> m) {
        return Promise<T>.Pure(m);
    }

    // liftM2 :: (Monad m) => (a -> b -> m c) -> m a -> m b -> m c
    public static Promise<V> LiftM2<T, U, V>(Func<T, U, Promise<V>> f, Promise<T> a, Promise<U> b) {
        return a.Bind((x) => {
            return b.Bind((y) => {
                return Pure(f(x, y));
            });
        });
    }

    // bindM2 :: (MonadParallel m) => (a -> b -> m c) -> m a -> m b -> m c
    public static Promise<V> BindM2<T, U, V>(Func<T, U, Promise<V>> f, Promise<T> a, Promise<U> b) {
        // TODO; liftM2 but parallel.
    }
}
