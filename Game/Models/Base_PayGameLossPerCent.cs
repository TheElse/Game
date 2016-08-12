using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Game.Models
{
    public class Base_PayGameLossPerCent
    {
        #region Model
        private int _id;
        private int _type;
        private decimal _losspercent;
        private DateTime? _adddatetime = DateTime.Now;
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
        public int type
        {
            set { _type = value; }
            get { return _type; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal LossPerCent
        {
            set { _losspercent = value; }
            get { return _losspercent; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? AddDateTime
        {
            set { _adddatetime = value; }
            get { return _adddatetime; }
        }
        #endregion Model
    }
}