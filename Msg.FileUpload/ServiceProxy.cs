using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Msg.FileUpload
{
    internal abstract class ServiceProxy<TChannel>
    {
        private Binding binding;
        private string remoteAddress;
        private ChannelFactory<TChannel> factory;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="binding"></param>
        protected ServiceProxy(Binding binding, string remoteAddress)
        {
            this.binding = binding;
            this.remoteAddress = remoteAddress;
        }

        /// <summary>
        /// 提供客户端用来与服务终结点进行通信的唯一网络地址
        /// </summary>
        /// <returns></returns>
        protected virtual EndpointAddress GetEndpointAddress()
        {
            EndpointAddress endpointAddress = new EndpointAddress(remoteAddress);
            return endpointAddress;
        }

        /// <summary>
        /// 构建通讯管道
        /// </summary>
        /// <returns></returns>
        internal TChannel CreateChannel()
        {
            EndpointAddress endpointAddress = GetEndpointAddress();
            factory = new ChannelFactory<TChannel>(binding);
            ClientViaBehavior clientViaBehavior = new ClientViaBehavior(new Uri(remoteAddress));
            factory.Endpoint.Behaviors.Add(clientViaBehavior);
            factory.Endpoint.Binding.CloseTimeout = TimeSpan.FromMinutes(10);
            factory.Endpoint.Binding.OpenTimeout = TimeSpan.FromMinutes(10);
            factory.Endpoint.Binding.SendTimeout = TimeSpan.FromMinutes(10);
            factory.Endpoint.Binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
            TChannel gc = factory.CreateChannel(endpointAddress);
            
            return gc;
        }

        internal void Dispose()
        {
            if (factory != null)
            {
                factory.Close();
                factory.Abort();
                factory = null;
            }
        }

    }
}
