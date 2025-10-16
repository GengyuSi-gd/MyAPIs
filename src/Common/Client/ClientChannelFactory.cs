using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using System;
using Common.Client;

namespace MMS.Service.CheckDeposit.Repository.Common
{
    public class ClientChannelFactory<TChannel> : IClientChannelFactory<TChannel>
        where TChannel : IClientChannel
    {
        private ChannelFactory<TChannel> _factory;

        public ClientChannelFactory(string endpointConfigurationName)
        {
            _factory = new ChannelFactory<TChannel>(new BasicHttpBinding(BasicHttpSecurityMode.Transport) );
        }
        public ClientChannelFactory(BasicHttpBinding binding, string endpointAddress)
        {
            _factory = new ChannelFactory<TChannel>(binding, new EndpointAddress(new Uri(endpointAddress)));
        }

        public ClientChannelFactory(ChannelFactory<TChannel> factory)
        {
            _factory = factory;
        }



        public void Open()
        {
            this._factory.Open();
        }

        public void Close()
        {
            this._factory.Close();
        }

        public void Abort()
        {
            this._factory.Abort();
        }

        public TChannel CreateChannel()
        {
            return this._factory.CreateChannel();
        }
    }
}
