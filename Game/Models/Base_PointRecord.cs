using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Game.Models
{
    public class Base_PointRecord
    {
        #region Model
        private int _id;
        private int _userid;
        private DateTime _adddatetime = DateTime.Now;
        private decimal _hpoint = 0M;
        private decimal _npoint = 0M;
        private string _remark;
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
        public DateTime AddDateTime
        {
            set { _adddatetime = value; }
            get { return _adddatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal HPoint
        {
            set { _hpoint = value; }
            get { return _hpoint; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal NPoint
        {
            set { _npoint = value; }
            get { return _npoint; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Remark
        {
            set { _remark = value; }
            get { return _remark; }
        }
        #endregion Model
    }
}