using System;

namespace Basket.Caching
{
    public class KeyValueStoreEntry
    {
        #region Public Properties

        public DateTime CreationDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public object Item { get; set; }

        public string Key { get; set; }

        #endregion Public Properties
    }
}