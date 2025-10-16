namespace Business.Extensions
{
    public static class WcfExtensions
    {
        //        public static TResponse Using<TClient, TResponse>(this TClient client, Func<TResponse> callback)
        //            where TClient : ICommunicationObject
        //        {
        //// ReSharper disable once CompareNonConstrainedGenericWithNull
        ////            if (null == client)
        ////            {
        ////                return default(TResponse);
        ////            }

        //            try
        //            {
        //                var response = callback();

        //                //if (client.State == CommunicationState.Opened)
        //                //{
        //                //    client.Close();
        //                //}

        //                return response;
        //            }
        //            catch (Exception)
        //            {
        //                if (client.State == CommunicationState.Faulted)
        //                {
        //                    client.Abort();
        //                }
        //                throw;
        //            }
        //        }

        //public static TResponse UsingService<TService, TResponse>(this TService service, Func<TResponse> func) where TService : ICommunicationObject
        //{
        //    try
        //    {
        //        var response = func();
        //        if (service != null)
        //        {
        //            service.Close();
        //        }
        //        return response;
        //    }
        //    catch (TimeoutException)
        //    {
        //        if (service != null)
        //        {
        //            service.Abort();
        //        }
        //        throw;
        //    }
        //    catch (FaultException)
        //    {
        //        if (service != null)
        //        {
        //            service.Abort();
        //        }
        //        throw;
        //    }
        //    catch (CommunicationException)
        //    {
        //        if (service != null)
        //        {
        //            service.Abort();
        //        }
        //        throw;
        //    }
        //}
    }
}

