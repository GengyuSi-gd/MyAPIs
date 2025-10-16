using System.ServiceModel;

namespace Common.Client
{
    public interface IClientChannelFactory<TChannel>
        where TChannel : IClientChannel
    {
        void Open();

        void Close();

        void Abort();

        TChannel CreateChannel();
    }
}