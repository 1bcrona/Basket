using Basket.IoC.Interface;
using System;

namespace Basket.IoC
{
    public class GeneratorGeneratorFactory<T> : IGeneratorFactory<T> where T : class
    {
        #region Private Fields

        private readonly Func<T> _GeneratorFunc;

        #endregion Private Fields

        #region Public Constructors

        public GeneratorGeneratorFactory(Func<T> generatorFunc)
        {
            _GeneratorFunc = generatorFunc;
        }

        #endregion Public Constructors

        #region Public Methods

        public T Create()
        {
            return _GeneratorFunc();
        }

        #endregion Public Methods
    }
}