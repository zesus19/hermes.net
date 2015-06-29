using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

using Arch.CMessaging.Client.Core.Ioc.LightInject;

namespace Arch.CMessaging.Client.Core.Ioc.Extensions.Annotation
{
    internal class NamedTypeExtractor : ITypeExtractor
    {
        public Type[] Execute(System.Reflection.Assembly assembly)
        {
            var targetNamespaces = new HashSet<string>();
            var resourceNames = assembly.GetManifestResourceNames().Where(n => n.EndsWith("VenusIoc.config"));
            foreach (var resourceName in resourceNames)
            {
                var xmlDoc = new XmlDocument();
                using (var sr = new StreamReader(assembly.GetManifestResourceStream(resourceName)))
                {
                    xmlDoc.Load(sr);
                    foreach (var node in xmlDoc.DocumentElement.SelectNodes("components/assemblyScan/namespace"))
                    {
                        var name = ((XmlElement)node).GetAttribute("name");
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            targetNamespaces.Add(name.Trim());
                        }
                    }
                }
            }

            var types = new List<Type>();
            foreach (var type in assembly.GetTypes())
            {
                if (targetNamespaces.Contains(type.Namespace) && !type.IsAbstract && type.IsDefined(typeof(NamedAttribute), false))
                {
                    types.Add(type);
                }
            }

            return types.ToArray();
        }
    }
}
