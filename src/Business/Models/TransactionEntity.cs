using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Filters;
using Common.Response;

namespace Business.Models
{
    public class TransactionEntity
    {
        public string CorelationId { get; set; }

        #region transaction properties
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public TransactionResponse TransactionResponse { get; set; }
        #endregion

        #region account info

        [DoNotLog]
        public string AccountNumber { get; set; }
        public AccountResponse AccountResponse { get; set; }

        #endregion
    }
}
