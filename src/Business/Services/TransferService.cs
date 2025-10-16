using Business.Models;
using Common.Request;
using Common.Response;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Helper;

namespace Business.Services
{
    public class TransferService: ITransferService
    {
        private readonly IMethodExecutionHelper<TransactionEntity> _methodExecutionHelper;

        public TransferService(IMethodExecutionHelper<TransactionEntity> methodExecutionHelper)
        {
            _methodExecutionHelper = methodExecutionHelper;
        }

        public Task<BaseResponse> Transfer(BaseRequest request)
        {
            TransactionEntity messageEntity = new TransactionEntity()
            {
                CorelationId =request.CorrelationId,
                
            } ;
            var methods = new List<MethodItem<TransactionEntity>>()
            {
                new MethodItem<TransactionEntity> {Method = GetEnrollment,FailAsGroup = true},
                new MethodItem<TransactionEntity> {Method = ValidateTransaction,FailAsGroup = true}
            };
            return Task.FromResult(_methodExecutionHelper.ExecuteMethods(messageEntity, methods));
        }

        #region private methods
        private AccountResponse GetEnrollment(TransactionEntity messageEntity)
        {
            var response = new AccountResponse
            {
                ResponseCode = "Success",
            };

            //get account info from db

            return response;
        }


        private BaseResponse ValidateTransaction(TransactionEntity messageEntity)
        {
            var methods = new List<MethodItem<TransactionEntity>>()
            {
                new MethodItem<TransactionEntity> {Method = ValidateCurrentTransaction,FailAsGroup = true},
            };

            var methods2 = new List<MethodItem<TransactionEntity>>()
            {
                new MethodItem<TransactionEntity> {Method = StartTransaction,FailAsGroup = true},
                new MethodItem<TransactionEntity> {Method = InsertTransaction,FailAsGroup = true},
                new MethodItem<TransactionEntity> {Method = RiskEvaluate,FailAsGroup = true},
                new MethodItem<TransactionEntity> {Method = ProcessSubmit,FailAsGroup = true},
            };

            var response = _methodExecutionHelper.ExecuteMethods(messageEntity, methods);


            if (response.ResponseCode == "FailedToStartSessionAtMMS" ||
                response.ResponseCode == "InvalidClientCheckReferenceId")
            {
                return response;
            }
            else if (response.ResponseCode != "Success")
            {
                return response;
            }
            else
            {
                //pass the validation
                Console.WriteLine("pass the validation");
                response = _methodExecutionHelper.ExecuteMethods(messageEntity, methods2);
            }

            return response;
        }
        private TransactionResponse ValidateCurrentTransaction(TransactionEntity messageEntity)
        {
            var response = new TransactionResponse()
            {
                ResponseCode = "Success",
            };

            //get account info from db

            return response;
        }

        public TransactionResponse StartTransaction(TransactionEntity messageEntity)
        {
            //call downstream service to start transaction
            Console.WriteLine("StartTransaction");

            var response = new TransactionResponse
            {
                ResponseCode = "Success",
            };
            return response;
        }

        public TransactionResponse InsertTransaction(TransactionEntity messageEntity)
        {
            var response = new TransactionResponse
            {
                ResponseCode = "Success"
            };

            // create transaction in db

            // set messageEntity.TransactionId = newly created transaction id
            messageEntity.TransactionId = Guid.NewGuid().ToString();
            Console.WriteLine("InsertTransaction");
            return response;
        }
        public TransactionResponse RiskEvaluate(TransactionEntity messageEntity)
        {
            var response = new TransactionResponse
            {
                ResponseCode = "Success"
            };

            // create transaction in db
            Console.WriteLine("RiskEvaluate");
            // set messageEntity.TransactionId = newly created transaction id

            return response;
        }


        private BaseResponse ProcessSubmit(TransactionEntity messageEntity)
        {
            var methods = new List<MethodItem<TransactionEntity>>()
            {
                new MethodItem<TransactionEntity> {Method = ProcessTransaction,FailAsGroup = true},
                new MethodItem<TransactionEntity> {Method = UpdateTransactionStatus,FailAsGroup = false}
            };

            var methods2 = new List<MethodItem<TransactionEntity>>()
            {
                new MethodItem<TransactionEntity> {Method = UpdateTransactionStatus,FailAsGroup = false},
            };

            var response = _methodExecutionHelper.ExecuteMethods(messageEntity, methods);
            if (response.ResponseCode != "Success")
            {
                _methodExecutionHelper.ExecuteMethods(messageEntity, methods2);
            }
            return response;
        }

        public TransactionResponse ProcessTransaction(TransactionEntity messageEntity)
        {
            var response = new TransactionResponse();

            try
            {
                //call downstream service to process
                response.TransactionId="12345";
                response.Amount=100.00M;

            }
            catch (TimeoutException)
            {
                messageEntity.TransactionResponse.ResponseCode = "Timeout";
                messageEntity.TransactionResponse.SubResponseCode = "DownService TimeOut";
                messageEntity.TransactionResponse.ResponseMessage = "Client ProcessV2 Call Timeout";
            }
            catch (Exception)
            {
                messageEntity.TransactionResponse.ResponseCode = "InternalException";
                messageEntity.TransactionResponse.SubResponseCode = "None";
                messageEntity.TransactionResponse.ResponseMessage = "Client ProcessV2 Call Exception";
            }


            //var response = messageEntity.TransactionResponse;
            messageEntity.TransactionResponse = messageEntity.TransactionResponse ?? new TransactionResponse();
            messageEntity.TransactionResponse.ResponseCode = "Success";
            messageEntity.TransactionResponse.TransactionId = response.TransactionId;
            messageEntity.TransactionResponse.Amount = response.Amount;
            messageEntity.TransactionResponse.ResponseDateTime = DateTime.Now;
                
          
            return messageEntity.TransactionResponse;
        }


        private BaseResponse UpdateTransactionStatus(TransactionEntity messageEntity)
        {
            var response = new TransactionResponse
            {
                ResponseCode = "Success"
            };

            //update the status in db
            

            return response;
        }


        #endregion
    }
}
