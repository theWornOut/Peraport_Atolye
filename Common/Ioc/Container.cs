using Common.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Common.Ioc
{
    public class Container : IContainer
    {
        public Container()
        {
            RegisteredObjects = new ConcurrentDictionary<string, object>();
        }

        public T Resolve<T>() where T : class
        {
            var targetType = typeof(T);
            return Resolve(targetType) as T;
        }

        public IDictionary<string, object> RegisteredObjects { get; }
        public T Resolve<T>(string name) where T : class
        {
            var targetType = typeof(T);
            return Resolve(name, targetType) as T;
        }

        public object Resolve(Type type)
        {
            return Resolve(string.Empty, type);
        }

        public object Resolve(string name, Type type)
        {
            if (!string.IsNullOrWhiteSpace(name)) return RegisteredObjects[name];
            var types = RegisteredObjects.Values.Where(s => s.GetType() == type).ToArray();
            if (!types.Any()) return RegisteredObjects[name];
            if (types.Count() > 1)
            {
                var peraportException = new PeraportException("Değer 1'den büyük", new ArgumentOutOfRangeException());
            }
            return types.FirstOrDefault();
        }

        public void Register<T>(string name, object[] parameterValue)
        {
            var targetType = typeof(T);
            Register(name, targetType, parameterValue);
        }

        public void Register(string name, Type type, params object[] parameterValue)
        {
            if (parameterValue != null && parameterValue.Any())
            {
                var cnstrMethod = type.GetConstructor(Type.EmptyTypes);
                if (cnstrMethod == null)
                {
                    var peraportException = new PeraportException("Null değer", new ArgumentNullException());
                }
                if (cnstrMethod == null) return;
                var instance = cnstrMethod.Invoke(null);
                RegisteredObjects.Add(name, instance);
                return;
            }

            if (parameterValue != null)
            {
                var construtorTypes = parameterValue.Select(w => w.GetType()).ToArray();
                var targetConstructor = type.GetConstructor(construtorTypes);
                //todo:Yeni bir exception yazalım.Ve ona uygun mesajlarla atsın.
                if (targetConstructor == null)
                {
                    var peraportException = new PeraportException("Constructor Null", new ArgumentNullException());
                }
                if (targetConstructor == null) return;
                var targetInstance = targetConstructor.Invoke(parameterValue);
                RegisteredObjects.Add(name, targetInstance);
            }
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            var targetType = typeof(T);
            var types = ResolveAll(targetType).ToArray();
            return types.Select(s => (T)s);

        }
        public IEnumerable<object> ResolveAll(Type targetType)
        {
            var types = RegisteredObjects.Values.Where(s => s.GetType() == targetType).ToArray();
            return types.Select(s => s);
        }
    }
}