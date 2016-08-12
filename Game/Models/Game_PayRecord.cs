using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;

namespace Game.Models
{
    public class Game_PayRecord
    {
        #region Model
        private int _id;
        private decimal _bettingamount;
        private int _userid;
        private int _type;
        private int _times;
        private DateTime _adddatetime = DateTime.Now;
        private string _year;
        private string _month;
        private int _isWin;
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
        public decimal BettingAmount
        {
            set { _bettingamount = value; }
            get { return _bettingamount; }
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
        public int TYPE
        {
            set { _type = value; }
            get { return _type; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Times
        {
            set { _times = value; }
            get { return _times; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime AddDatetime
        {
            set { _adddatetime = value; }
            get { return _adddatetime; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Year
        {
            set { _year = value; }
            get { return _year; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Month
        {
            set { _month = value; }
            get { return _month; }
        }

        public int IsWin
        {
            set { _isWin = value; }
            get { return _isWin; }
        }

        #endregion Model
    }

    public class GameTotalEntity
    {
        /// <summary>
        /// 积分
        /// </summary>
        public decimal Point { get; set; }
        /// <summary>
        /// 总充值额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 可用余额
        /// </summary>
        public decimal AmountMoney { get; set; }
        /// <summary>
        /// 月充值总额
        /// </summary>
        public decimal MonthAmount { get; set; }
        /// <summary>
        /// 月统计输赢
        /// </summary>
        public decimal MonthSFTime { get; set; }
    }

    /// <summary>
    /// 用户投注金额信息
    /// </summary>
    public class GameUserAccount
    {
        public int UserId { get; set; }

        public decimal TotalMoney { get; set; }
    }

    public class GameResultImport
    {
        /// <summary>
        /// 投资金额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 局数
        /// </summary>
        public int Times { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int TYPE { get; set; }
    }

    public class GameResultReort
    {
        public string UserName { get; set; }//用户名
        public string Result0 { get; set; }//闲
        public string Result1 { get; set; }//和
        public string Result2 { get; set; }//庄
        public string Result3 { get; set; }//庄对
        public string Result4 { get; set; }//闲对

    }
}