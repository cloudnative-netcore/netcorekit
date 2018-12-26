using System;
using System.Linq;
using AutoMapper;
using Google.Protobuf;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace NetCoreKit.Infrastructure.Bus
{
    public static class ConvertExtensions
    {
        public static INotification ToNotification<TMessage>(this TMessage msg, IConfiguration config, string key)
            where TMessage : IMessage<TMessage>, new()
        {
            var assemblies = config.LoadFullAssemblies();
            var types = assemblies.SelectMany(t => t.DefinedTypes);
            var notifyType = types.FirstOrDefault(x => x.AssemblyQualifiedName.Contains(key.ToString()));

            var notify = (INotification)Activator.CreateInstance(notifyType);
            var notifyInstance = Mapper.Map(msg, notify);
            return notifyInstance;
        }
    }
}
