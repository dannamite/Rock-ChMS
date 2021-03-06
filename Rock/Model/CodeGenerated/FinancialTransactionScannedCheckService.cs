//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rock.CodeGeneration project
//     Changes to this file will be lost when the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
//
// THIS WORK IS LICENSED UNDER A CREATIVE COMMONS ATTRIBUTION-NONCOMMERCIAL-
// SHAREALIKE 3.0 UNPORTED LICENSE:
// http://creativecommons.org/licenses/by-nc-sa/3.0/
//

using System;
using System.Linq;

using Rock.Data;

namespace Rock.Model
{
    /// <summary>
    /// FinancialTransactionScannedCheck Service class
    /// </summary>
    public partial class FinancialTransactionScannedCheckService : Service<FinancialTransactionScannedCheck>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FinancialTransactionScannedCheckService"/> class
        /// </summary>
        public FinancialTransactionScannedCheckService()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FinancialTransactionScannedCheckService"/> class
        /// </summary>
        /// <param name="repository">The repository.</param>
        public FinancialTransactionScannedCheckService(IRepository<FinancialTransactionScannedCheck> repository) : base(repository)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FinancialTransactionScannedCheckService"/> class
        /// </summary>
        /// <param name="context">The context.</param>
        public FinancialTransactionScannedCheckService(RockContext context) : base(context)
        {
        }
    }

    /// <summary>
    /// Generated Extension Methods
    /// </summary>
    public static partial class FinancialTransactionScannedCheckExtensionMethods
    {
        /// <summary>
        /// Clones this FinancialTransactionScannedCheck object to a new FinancialTransactionScannedCheck object
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="deepCopy">if set to <c>true</c> a deep copy is made. If false, only the basic entity properties are copied.</param>
        /// <returns></returns>
        public static FinancialTransactionScannedCheck Clone( this FinancialTransactionScannedCheck source, bool deepCopy )
        {
            if (deepCopy)
            {
                return source.Clone() as FinancialTransactionScannedCheck;
            }
            else
            {
                var target = new FinancialTransactionScannedCheck();
                target.ScannedCheckMicr = source.ScannedCheckMicr;
                target.AuthorizedPersonId = source.AuthorizedPersonId;
                target.BatchId = source.BatchId;
                target.GatewayId = source.GatewayId;
                target.TransactionDateTime = source.TransactionDateTime;
                target.Amount = source.Amount;
                target.TransactionCode = source.TransactionCode;
                target.Summary = source.Summary;
                target.TransactionTypeValueId = source.TransactionTypeValueId;
                target.CurrencyTypeValueId = source.CurrencyTypeValueId;
                target.CreditCardTypeValueId = source.CreditCardTypeValueId;
                target.SourceTypeValueId = source.SourceTypeValueId;
                target.CheckMicrEncrypted = source.CheckMicrEncrypted;
                target.Id = source.Id;
                target.Guid = source.Guid;

            
                return target;
            }
        }
    }
}
