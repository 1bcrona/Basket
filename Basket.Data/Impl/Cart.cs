using Basket.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Basket.Data.Impl
{
    public class Cart : IEntity<string>
    {
        #region Public Properties

        public string Id { get; set; }
        public List<string> ProductIds { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, ProductIds?.Distinct().Aggregate(0, (x, y) => x.GetHashCode() ^ y.GetHashCode()));
        }

        #endregion Public Methods
    }
}