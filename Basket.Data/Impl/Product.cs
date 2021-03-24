using Basket.Data.Interface;
using System;

namespace Basket.Data.Impl
{
    public class Product : IEntity<string>
    {
        #region Public Properties

        public string Description { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Description, Quantity, Id);
        }

        #endregion Public Methods
    }
}