using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Game.Models
{
    public class Base_AccountRecord
    {
        #region Model
        private int _id;
        private int _userid;
        private decimal _hamout = 0M;
        private decimal _namout = 0M;
        private string _remark;
        private DateTime _adddatetime = DateTime.Now;
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
        public decimal Hamout
        {
            set { _hamout = value; }
            get { return _hamout; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal Namout
        {
            set { _namout = value; }
            get { return _namout; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Remark
        {
            set { _remark = value; }
            get { return _remark; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime AddDateTime
        {
            set { _adddatetime = value; }
            get { return _adddatetime; }
        }
        #endregion Model
    }
}