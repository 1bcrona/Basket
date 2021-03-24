using System;

namespace Basket.IoC
{
    public class DIDefinition
    {
        #region Public Enums

        public enum Scope
        {
            Transient,
            Singleton,
            Scoped
        }

        #endregion Public Enums

        #region Public Properties

        public object[] Args { get; set; }
        public Scope DIScope { get; set; }
        public Type ImplementationTypeObj { get; set; }
        public Type ServiceTypeObj { get; set; }

        #endregion Public Properties
    }
}