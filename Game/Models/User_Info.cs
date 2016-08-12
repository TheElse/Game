using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Game.Models
{
    public class User_Info
    {
        #region Model
        private int _userid;
        private string _username;
        private DateTime? _adddatetime = DateTime.Now;
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
        public string UserName
        {
            set { _username = value; }
            get { return _username; }
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

    public class UserAccountInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime AddDateTime { get; set; }
        /// <summary>
        /// 充值额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 当前积分
        /// </summary>
        public decimal Point { get; set; }
        /// <summary>
        /// 参与数量
        /// </summary>
        public int CountTimes { get; set; }
        /// <summary>
        /// 输赢情况
        /// </summary>
        public decimal WinMoney { get; set; }


    }

    public class UserInfo
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}