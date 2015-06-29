using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Ioc
{
    /// <summary>
    /// Providers a default container.
    /// </summary>
    public class VenusContainerLoader
    {
        private static readonly IVenusContainer container = new VenusContainer();

        private VenusContainerLoader()
        { }

        /// <summary>
        /// Gets the default container instance.
        /// </summary>
        public static IVenusContainer Container
        {
            get { return container; }
        }
    }
}
