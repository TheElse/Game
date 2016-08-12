using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Game.Models
{
    public class Base_Account
    {
        #region Model
        private int _id;
        private int _userid;
        private decimal _amountmoney = 0M;
        private decimal _totalmoney = 0M;
        private DateTime _adddatetime = DateTime.Now;
        private DateTime? _updateDateTime;
        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int UserId
        {
            set { _userid = value; }
            get { return _userid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal AmountMoney
        {
            set { _amountmoney = value; }
            get { return _amountmoney; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal TotalMoney
        {
            set { _totalmoney = value; }
            get { return _totalmoney; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime AddDateTime
        {
            set { _adddatetime = value; }
            get { return _adddatetime; }
        }


        /// <summary>
        /// 
        /// </summary>
        public DateTime? UpdateDateTime
        {
            set { _updateDateTime = value; }
            get { return _updateDateTime; }
        }

        #endregion Model

    }

    public class WithdrawalEntity
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        /// <summary>
        /// 可提现金额
        /// </summary>
        public decimal AmountMoney { get; set; }

        /// <summary>
        /// 充值总额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 记录ID
        /// </summary>
        public int Id { get; set; }
    }

    
}