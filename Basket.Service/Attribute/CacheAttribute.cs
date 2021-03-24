using Basket.Service.Exception.Impl;
using System;

namespace Basket.Service.Attribute
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheAttribute : System.Attribute
    {
        #region Public Constructors

        public CacheAttribute(int timeSpan)
        {
            if (timeSpan < 0)
            {
                throw new InsufficientException("TimeSpan.TotalSeconds");
            }

            TimeSpan = TimeSpan.FromSeconds(timeSpan);
        }

        #endregion Public Constructors

        #region Public Properties

        public TimeSpan TimeSpan { get; }

        #endregion Public Properties
    }
}