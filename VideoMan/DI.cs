using System;
using System.Collections.Generic;
using System.Linq;

namespace VideoMan
{
    // ReSharper disable once InconsistentNaming
    public class DI
    {
        private static DI _instance;
        public static DI Instance => _instance ?? (_instance = new DI());

        private readonly Dictionary<Type, Type> _dictionary = new Dictionary<Type, Type>();

        public void Register<TContract, TImplementation>() where TImplementation : TContract
        {
            _dictionary.Add(typeof(TContract), typeof(TImplementation));
        }

        public TContract GetInstance<TContract>()
        {
            return (TContract)GetInstance(typeof(TContract));
        }
        private object GetInstance (Type contract)
        {
            var implementation = _dictionary[contract];
            var constructor = implementation.GetConstructors()[0];
            var constructorParameters = constructor.GetParameters();
            if (constructorParameters.Length == 0)
            {
                return Activator.CreateInstance(implementation);
            }
            var parameters = new List<object>(constructorParameters.Length);
            parameters.AddRange(constructorParameters.Select(parameterInfo => GetInstance(parameterInfo.ParameterType)));
            return constructor.Invoke(parameters.ToArray());
        }
    }
}