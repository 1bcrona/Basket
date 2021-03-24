using System;

namespace Basket.Repository.Attribute
{
    public class CollectionName : System.Attribute
    {
        #region Public Constructors

        public CollectionName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Empty CollectionName not allowed", nameof(value));
            Name = value;
        }

        #endregion Public Constructors

        #region Public Properties

        public virtual string Name { get; }

        #endregion Public Properties
    }
}