using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Game.Models
{
    public class Base_Point
    {
        #region Model

        private int _id;
        private int _userid;
        private decimal _point = 0M;
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
        public decimal Point
        {
            set { _point = value; }
            get { return _point; }
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