using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DBUtility;
using Game.Models;

namespace Game.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 充值页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Recharge()
        {
            return View();
        }


        [HttpPost]
        public JsonResult Recharge(string userName, decimal amountMoney, int? userId)
        {

            DateTime nowDateTime = DateTime.Now;

            if (userId.HasValue)
            {
                if (userId > 0)
                {
                    string sql = string.Format(@"SELECT  * FROM Base_Account WHERE UserId={0} ", userId);
                    string sql2 = string.Format("SELECT * FROM Base_Point WHERE UserId={0}", userId);
                    var _baseAccount = DataFactory.Query<Base_Account>(sql, "").FirstOrDefault(); //用户账户信息
                    var _basePoint = DataFactory.Query<Base_Point>(sql2, "").FirstOrDefault(); //用户积分信息
                    if (_baseAccount != null)
                    {
                        /*更新余额信息*/
                        Base_Account uBaseAccount = new Base_Account();
                        uBaseAccount.AmountMoney = _baseAccount.AmountMoney + amountMoney;
                        uBaseAccount.TotalMoney = _baseAccount.TotalMoney + amountMoney;
                        uBaseAccount.Id = _baseAccount.Id;
                        uBaseAccount.UserId = _baseAccount.UserId;
                        uBaseAccount.UpdateDateTime = nowDateTime;
                        DataFactory.Update(uBaseAccount);
                        /*更新余额信息*/

                        /*记录余额变化*/
                        Base_AccountRecord uBaseAccountRecord = new Base_AccountRecord();
                        uBaseAccountRecord.UserId = _baseAccount.UserId;
                        uBaseAccountRecord.Hamout = _baseAccount.AmountMoney; //历史余额
                        uBaseAccountRecord.Namout = uBaseAccount.AmountMoney; //现余额
                        uBaseAccountRecord.Remark = string.Format("{1},用户充值金额:{0}", amountMoney, nowDateTime);
                        uBaseAccountRecord.AddDateTime = nowDateTime;
                        DataFactory.Insert(uBaseAccountRecord);
                        /*记录余额变化*/

                        /*记录充值记录*/
                        Base_AccountMonthRecord uBaseAccountMonthRecord = new Base_AccountMonthRecord();
                        uBaseAccountMonthRecord.UserId = _baseAccount.UserId;
                        uBaseAccountMonthRecord.YEAR = nowDateTime.Year.ToString();
                        uBaseAccountMonthRecord.MONTH = nowDateTime.Month.ToString();
                        uBaseAccountMonthRecord.Amount = amountMoney;
                        uBaseAccountMonthRecord.AddDateTime = nowDateTime;
                        DataFactory.Insert(uBaseAccountMonthRecord);
                        /*记录充值记录*/

                        /*更新积分*/
                        Base_Point uBasePoint = new Base_Point();
                        uBasePoint.UserId = _baseAccount.UserId;
                        uBasePoint.Point = _basePoint.Point + amountMoney; //原积分+充值金额
                        uBasePoint.Id = _basePoint.Id;
                        DataFactory.Update(uBasePoint);
                        /*更新积分*/

                        /*添加积分历史明细*/
                        Base_PointRecord uBasePointRecord = new Base_PointRecord();
                        uBasePointRecord.UserId = _baseAccount.UserId;
                        uBasePointRecord.AddDateTime = nowDateTime;
                        uBasePointRecord.HPoint = _basePoint.Point; //历史积分
                        uBasePointRecord.NPoint = uBasePoint.Point; //充值后的积分
                        uBasePointRecord.Remark = string.Format("{0} 新增{1}积分", nowDateTime, amountMoney);
                        DataFactory.Insert(uBasePointRecord);
                        /*添加积分历史明细*/

                        return Json(new {sucess = 1});
                    }
                    else
                    {
                        //返回空提示
                        return Json(new {sucess = -98, msg = "添加失败"});
                    }
                }
                else
                {
                    int uid = AddNewUser(userName, amountMoney);
                    if (uid > 0)
                    {
                        return Json(new { sucess = 1 });
                    }
                }
            }

            //if (!string.IsNullOrWhiteSpace(userName))
            //{
            //    string sql = string.Format("SELECT * FROM User_Info WHERE userName='{0}'", userName);
            //    var userInfo = DBUtility.DataFactory.Query<User_Info>(sql, "");

            //    if (userInfo != null && userInfo.Count() > 0)
            //    {
            //        return Json(new {sucess = -98,msg="该用户已经存在"});
            //    }
            //    else
            //    {


            //        /*添加用户信息*/
            //        User_Info uInfo = new User_Info();
            //        uInfo.UserName = userName;
            //        uInfo.AddDateTime = nowDateTime;
            //        int uId = Convert.ToInt32(DataFactory.Insert(uInfo));
            //        /*添加用户信息*/

            //        /*添加账户余额*/
            //        Base_Account baseAccount = new Base_Account();
            //        baseAccount.UserId = uId;
            //        baseAccount.AmountMoney = amountMoney;
            //        baseAccount.TotalMoney = amountMoney;
            //        baseAccount.AddDateTime = nowDateTime;
            //        baseAccount.UpdateDateTime = null;
            //        DBUtility.DataFactory.Insert(baseAccount);
            //        /*添加账户余额*/

            //        /*充值记录*/
            //        Base_AccountRecord baseAccountRecord = new Base_AccountRecord();
            //        baseAccountRecord.UserId = uId;
            //        baseAccountRecord.Hamout = 0; //原金额
            //        baseAccountRecord.Namout = amountMoney;
            //        baseAccountRecord.AddDateTime = nowDateTime;
            //        baseAccountRecord.Remark = "新用户充值，充值金额" + amountMoney.ToString();
            //        DBUtility.DataFactory.Insert(baseAccountRecord);
            //        /*充值记录*/

            //        /*每月的充值流水记录*/
            //        Base_AccountMonthRecord baseAccountMonthRecord = new Base_AccountMonthRecord();
            //        baseAccountMonthRecord.AddDateTime = nowDateTime;
            //        baseAccountMonthRecord.YEAR = nowDateTime.Year.ToString();
            //        baseAccountMonthRecord.MONTH = nowDateTime.Month.ToString();
            //        baseAccountMonthRecord.UserId = uId;
            //        baseAccountMonthRecord.Amount = amountMoney;
            //        DBUtility.DataFactory.Insert(baseAccountMonthRecord);
            //        /*每月的充值流水记录*/

            //        /*用户积分*/
            //        Base_Point basePoint = new Base_Point();
            //        basePoint.Point = amountMoney;
            //        basePoint.UserId = uId;
            //        basePoint.AddDateTime = nowDateTime;
            //        DBUtility.DataFactory.Insert(basePoint);
            //        /*用户积分*/

            //        /*积分历史记录*/
            //        Base_PointRecord basePointRecord = new Base_PointRecord();
            //        basePointRecord.UserId = uId;
            //        basePointRecord.AddDateTime = nowDateTime;
            //        basePointRecord.HPoint = 0; //历史积分
            //        basePointRecord.NPoint = amountMoney; //现积分
            //        basePointRecord.Remark = string.Format("新用户充值初始化{0}积分", amountMoney);
            //        DBUtility.DataFactory.Insert(basePointRecord);
            //        /*积分历史记录*/

            //        return Json(new {sucess = 1});
            //    }
            //}

            return Json(new {sucess = -99});
        }

        /// <summary>
        /// 用户账户
        /// </summary>
        /// <returns></returns>

        public ActionResult UserAccount()
        {
            return View();
        }


        /// <summary>
        /// 用户提现
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Withdrawal()
        {
            return View();
        }

        public JsonResult UserAccountInfo(string userName)
        {
            string sql =
                string.Format(
                    "SELECT UserName,b.AmountMoney,TotalMoney,b.Id FROM User_Info AS a INNER JOIN Base_Account AS b ON a.userid=b.UserId AND a.UserName LIKE '%{0}%'",
                    userName);

            var withdrawalEntity = DBUtility.DataFactory.Query<WithdrawalEntity>(sql, "");

            if (withdrawalEntity != null)
            {
                return Json(new {userInfoItem = withdrawalEntity, sucess = 1});
            }
            else
            {
                return Json(new {sucess = 0});
            }
        }

        /// <summary>
        /// 提现
        /// </summary>
        /// <param name="amountMoney"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Withdrawal(decimal amountMoney, int id)
        {
            Base_Account _baseAccount = DBUtility.DataFactory.Get<Base_Account>(id);
            if (_baseAccount == null)
            {
                return Json(new {sucess = 0, msg = "提现失败,没有相关信息"});
            }

            if (_baseAccount.AmountMoney < amountMoney)
            {
                return Json(new {sucess = 0, msg = "提现失败,超出提现余额"});
            }

            DateTime nowDateTime = DateTime.Now;
            decimal hAmount = _baseAccount.AmountMoney;

            #region  更新账户余额

            _baseAccount.AmountMoney = _baseAccount.AmountMoney - amountMoney; //余额
            _baseAccount.UpdateDateTime = nowDateTime;
            _baseAccount.Id = id;
            DataFactory.Update(_baseAccount);

            #endregion

            Base_AccountRecord _baseAccountRecord = new Base_AccountRecord();
            _baseAccountRecord.Hamout = hAmount;
            _baseAccountRecord.Namout = _baseAccount.AmountMoney;
            _baseAccountRecord.Remark = string.Format("{0}提现{1}", nowDateTime, amountMoney);
            _baseAccountRecord.UserId = _baseAccount.UserId;
            DataFactory.Update(_baseAccountRecord);

            string sql = string.Format("select * from Base_Point where userId={0}", _baseAccount.UserId);

            Base_Point basePoint = DBUtility.DataFactory.Query<Base_Point>(sql, "").FirstOrDefault();
            if (basePoint != null)
            {
                decimal point = basePoint.Point;
                /*更新积分--(s)*/
                basePoint.Point = basePoint.Point - amountMoney;
                DBUtility.DataFactory.Update(basePoint);
                /*更新积分--(s)*/

                #region 积分变更记录

                Base_PointRecord basePointRecord = new Base_PointRecord();
                basePointRecord.HPoint = point; //历史积分
                basePointRecord.NPoint = basePoint.Point;
                basePointRecord.UserId = basePoint.UserId;
                basePointRecord.Remark = string.Format("{0}提现，积分变更为{1}", nowDateTime, basePointRecord.NPoint);
                DataFactory.Insert(basePointRecord);

                #endregion
            }

            return Json(new {sucess = 1, msg = "提现成功"});

        }

       
        [HttpGet]
        public ActionResult AccountDetail()
        {
//            #region SQL语句

//            string sql = string.Format(@"select a.UserId
//	  ,a.UserName   --用户名
//      ,a.AddDateTime  --注册时间
//	  ,isnull(b.TotalMoney,0) as TotalMoney  --充值额
//	  ,isnull(c.Point,0)  as Point       --当前积分
//	  ,isnull(d.countTimes,0) as CountTimes --参与数量
//	  ,ISNULL(d.winMoney,0) as  WinMoney--输赢情况
//
//from User_Info as a left join Base_Account  as  b on a.UserId=b.UserId
//					left join Base_Point  as c on a.UserId=c.UserId
//					left join (select  isnull(sum( case when IsWin=1 then BettingAmount  --1赢钱为正
//											else BettingAmount*-1    end     --0输数为负
//										   ),0 )as winMoney
//									   ,count(Times) as countTimes --参与数量
//									   ,UserId
//								from Game_PayRecord
//								group by UserId
//								) as d on a.UserId=d.UserId
//");

//            #endregion

            List<UserAccountInfo> list = GetUserAccountInfoList("",null,"","");
           // list = DataFactory.Query<UserAccountInfo>(sql, "").ToList();
            if (list != null && list.Count > 0)
            {
                ViewBag.UserAccountList = list;
            }

            return View();
        }

        [HttpPost]
        public ActionResult AccountDetail(string userName)
        {
            string startTime = Request["statTime"];
            string endTime = Request["endTime"];

            List<UserAccountInfo> list = GetUserAccountInfoList(userName, null, startTime, endTime);

            if (list != null && list.Count > 0)
            {
                ViewBag.UserAccountList = list;
            }
            ViewBag.UserName = userName;
            ViewBag.StartTime = startTime;
            ViewBag.EndTime = endTime;

            return View();
        }

        /// <summary>
        /// 获取用户的信息
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public JsonResult GetUserPayGameAccountInfoDetail(string userIds)
        {
            List<UserAccountInfo> list = GetUserAccountInfoList("", userIds.TrimEnd(','),"","");
            if (null != list && list.Count > 0)
            {
                return Json(new {dateItem = list, sucess = 1});
            }
            else
            {
                return Json(new { dateItem = list, sucess = 0 });
            }
        }

        /// <summary>
        /// 获取账户详情
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private List<UserAccountInfo> GetUserAccountInfoList(string userName,string userIds,string statTime,string endTime)
        {
            #region

            string sqlWhere = string.Empty;
            string sqlWhere2 = string.Empty;

            if (!string.IsNullOrWhiteSpace(userName))
            {
                sqlWhere2 += string.Format(@" and a.UserName LIKE '%{0}%'", userName);
                sqlWhere += string.Format(@" and t2.UserName LIKE '%{0}%'", userName);
            }

            if (!string.IsNullOrWhiteSpace(userIds))
            {
                sqlWhere2 += string.Format(@" and a.UserId in({0})", userIds);
                sqlWhere += string.Format(@" and t1.UserId in({0})", userIds);
            }

            if (!string.IsNullOrWhiteSpace(statTime))
            {
                //sqlWhere += string.Format(@" and a.AddDateTime >='{0} 00:00:00' ", statTime);
                sqlWhere += string.Format(@" and t1.AddDateTime >='{0} 00:00:00' ", statTime);
            }

            if (!string.IsNullOrWhiteSpace(endTime))
            {
              //  sqlWhere += string.Format(@" and a.AddDateTime <='{0} 23:59:59' ", endTime);
                sqlWhere += string.Format(@" and t1.AddDateTime <='{0} 23:59:59' ", endTime);
            }


//            string sql = string.Format(@"select a.UserId
//	  ,a.UserName   --用户名
//      ,a.AddDateTime  --注册时间
//	  ,isnull(b.TotalMoney,0) as TotalMoney  --充值额
//	  ,isnull(c.Point,0)  as Point       --当前积分
//	  ,isnull(d.countTimes,0) as CountTimes --参与数量
//	  ,ISNULL(d.winMoney,0) as  WinMoney--输赢情况
//
//from User_Info as a left join Base_Account  as  b on a.UserId=b.UserId
//					left join Base_Point  as c on a.UserId=c.UserId
//					left join (select  isnull(sum( case when IsWin=1 then BettingAmount  --1赢钱为正
//											else BettingAmount*-1    end     --0输数为负
//										   ),0 )as winMoney
//									   ,count(Times) as countTimes --参与数量
//									   ,UserId
//								from Game_PayRecord
//								group by UserId
//								) as d on a.UserId=d.UserId
//    where 1=1 {0}
//", sqlWhere);



            string sql = string.Format(@"select a.UserId
	  ,a.UserName   --用户名
      ,a.AddDateTime  --注册时间
	  ,ISNULL( b.TotalAmountMoney,0)as TotalMoney --充值金额
	  ,isnull(c.Point,0)  as Point       --当前积分
	 ,isnull(d.countTimes,0) as CountTimes --参与数量
	  ,ISNULL(d.winMoney,0) as  WinMoney--输赢情况

from User_Info as a LEFT JOIN (
								 SELECT SUM(Amount)AS TotalAmountMoney,t1.UserId FROM Base_AccountMonthRecord as t1 inner join User_Info as t2 on t1.userid=t2.userid
								WHERE 1=1  {0}
								GROUP BY t1.UserId
							  ) AS b ON a.UserId = b.UserId
					left join Base_Point  as c on a.UserId=c.UserId
					LEFT JOIN (
								/*输赢情况、参与数量--s*/
								select  isnull(sum( case when IsWin=1 then BettingAmount  --1赢钱为正
																			else BettingAmount*-1    end     --0输数为负
																		   ),0 )as winMoney
																	   ,count(Times) as countTimes --参与数量
																	   ,t1.UserId
																from Game_PayRecord as t1 INNER JOIN dbo.User_Info AS t2 ON t1.UserId=t2.UserId
																WHERE 1=1  {0}
																group by t1.UserId
								/*输赢情况、参与数量--e*/
							  ) AS d ON a.UserId=d.UserId
    where 1=1 {1} ", sqlWhere,sqlWhere2);


            #endregion

            List<UserAccountInfo> list = new List<UserAccountInfo>();
            list = DataFactory.Query<UserAccountInfo>(sql, "").ToList();
            return list;
        }





        ///// <summary>
        ///// 获取搜索账户的数据
        ///// </summary>
        ///// <param name="userName">用户名</param>
        ///// <returns></returns>
        //public JsonResult AccountDetail(string userName)
        //{
        //    List<UserAccountInfo> list = GetUserAccountInfoList(userName);
        //    if (list != null && list.Count > 0)
        //    {
        //        return Json(new {sucess = 1, dataEntity = list});
        //    }
        //    else
        //    {
        //        return Json(new { sucess = 0 });
        //    }
        //}

        public string GetUserInfoByUserName(string q)
        {
            string sql = string.Format("select UserId,UserName from User_Info where UserName like '%{0}%'", HttpUtility.UrlDecode(q));

            var userInfo = DataFactory.Query<UserInfo>(sql, "");

            Jayrock.Json.JsonArray jsonList = new Jayrock.Json.JsonArray();
            Jayrock.Json.JsonObject json;
            if (userInfo != null)
            {
                string str = "";
                foreach (var itemUser in userInfo)
                {
                    json = new Jayrock.Json.JsonObject();
                    json.Accumulate("UserId", itemUser.UserId);
                    json.Accumulate("UserName", itemUser.UserName);
                    jsonList.Add(json);
                }
                str = jsonList.ToString();
                return str;
            }

            else
            {
                return "[]";
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        public JsonResult GetUserInfoByName(string userName)
        {
            string sql = string.Format("SELECT UserId,UserName FROM User_Info WHERE UserName='{0}'", userName.Trim());
            var userInfo = DataFactory.Query<UserInfo>(sql, "").FirstOrDefault();

            if (userInfo != null)
            {
                return Json(new {sucess = 1, UserId = userInfo.UserId});
            }
            else
            {
                return Json(new {sucess = 0});
            }
        }

        /// <summary>
        /// 充值添加用户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="amountMoney"></param>
        private int AddNewUser(string userName, decimal amountMoney)
        {

            DateTime nowDateTime = DateTime.Now;

            /*添加用户信息*/
            User_Info uInfo = new User_Info();
            uInfo.UserName = userName;
            uInfo.AddDateTime = nowDateTime;
            int uId = Convert.ToInt32(DataFactory.Insert(uInfo));
            /*添加用户信息*/

            /*添加账户余额*/
            Base_Account baseAccount = new Base_Account();
            baseAccount.UserId = uId;
            baseAccount.AmountMoney = amountMoney;
            baseAccount.TotalMoney = amountMoney;
            baseAccount.AddDateTime = nowDateTime;
            baseAccount.UpdateDateTime = null;
            DBUtility.DataFactory.Insert(baseAccount);
            /*添加账户余额*/

            /*充值记录*/
            Base_AccountRecord baseAccountRecord = new Base_AccountRecord();
            baseAccountRecord.UserId = uId;
            baseAccountRecord.Hamout = 0; //原金额
            baseAccountRecord.Namout = amountMoney;
            baseAccountRecord.AddDateTime = nowDateTime;
            baseAccountRecord.Remark = "新用户充值，充值金额" + amountMoney.ToString();
            DBUtility.DataFactory.Insert(baseAccountRecord);
            /*充值记录*/

            /*每月的充值流水记录*/
            Base_AccountMonthRecord baseAccountMonthRecord = new Base_AccountMonthRecord();
            baseAccountMonthRecord.AddDateTime = nowDateTime;
            baseAccountMonthRecord.YEAR = nowDateTime.Year.ToString();
            baseAccountMonthRecord.MONTH = nowDateTime.Month.ToString();
            baseAccountMonthRecord.UserId = uId;
            baseAccountMonthRecord.Amount = amountMoney;
            DBUtility.DataFactory.Insert(baseAccountMonthRecord);
            /*每月的充值流水记录*/

            /*用户积分*/
            Base_Point basePoint = new Base_Point();
            basePoint.Point = amountMoney;
            basePoint.UserId = uId;
            basePoint.AddDateTime = nowDateTime;
            DBUtility.DataFactory.Insert(basePoint);
            /*用户积分*/

            /*积分历史记录*/
            Base_PointRecord basePointRecord = new Base_PointRecord();
            basePointRecord.UserId = uId;
            basePointRecord.AddDateTime = nowDateTime;
            basePointRecord.HPoint = 0; //历史积分
            basePointRecord.NPoint = amountMoney; //现积分
            basePointRecord.Remark = string.Format("新用户充值初始化{0}积分", amountMoney);
            DBUtility.DataFactory.Insert(basePointRecord);
            /*积分历史记录*/

            return uId;
        }

        /// <summary>
        /// 添加用户  返回用户ID
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public JsonResult AddUser(string userName)
        {
            int userId = AddNewUser(userName.Trim(), 0);

            if (userId > 0)
            {
                return Json(new {sucess = 1, UserId = userId});
            }
            return Json(new { sucess = 0, UserId = userId });
        }

        /// <summary>
        /// 获取用户的账户信
        /// </summary>
        /// <param name="userIds">用户ID数组</param>
        /// <returns></returns>
        public JsonResult GetUserAccountByUserIds(string userIds,string bettingAmountItems)
        {
          List<GameUserAccount> list=new List<GameUserAccount>();
            string[] userAccountArrary = bettingAmountItems.TrimEnd(',').Split(',');
            if (userAccountArrary.Length > 0)
            {
                for (int i = 0; i < userAccountArrary.Length; i++)
                {
                    int userId = Convert.ToInt32(userAccountArrary[i].Split('|')[0]);//用户ID
                    decimal amount = Convert.ToDecimal(userAccountArrary[i].Split('|')[1]);//投注金额
                    bool isResult = false;
                    if (!string.IsNullOrWhiteSpace(userAccountArrary[i]))
                    {
                        for (int j = 0; j < list.Count; j++)
                        {
                            if (list[j].UserId == userId)
                            {
                                list[j].TotalMoney += amount;
                                isResult = true;
                            }
                        }

                        if (!isResult)
                        {
                            GameUserAccount item = new GameUserAccount();
                            item.UserId = userId;
                            item.TotalMoney = amount;
                            list.Add(item);
                        }
                    }
                }
            }
            StringBuilder errorText = new StringBuilder();
              string sql =string.Format(
                    "SELECT a.UserId, UserName,b.AmountMoney,TotalMoney,b.Id FROM User_Info AS a INNER JOIN Base_Account AS b ON a.userid=b.UserId AND a.UserId in({0})",
                    userIds);

           // string sql = string.Format(@"select * from Base_Account where UserId in({0})", userIds);
            var userAccountInfo = DataFactory.Query<WithdrawalEntity>(sql, "");
            bool hasError = false;
            if ((userAccountInfo != null && userAccountInfo.Count() > 0))
            {

                foreach (var item in list)
                {
                    var amountEntity = userAccountInfo.Where(m => m.UserId == item.UserId).FirstOrDefault();

                    if (item.TotalMoney > amountEntity.AmountMoney)
                    {
                        errorText.AppendFormat("账号：{0}投注的金额为:{1},超过账户上余额，不能投注!\n", amountEntity.UserName,
                            item.TotalMoney);
                        hasError = true;
                    }
                }
            }

            if (hasError)
            {
                return Json(new { sucess = 0, msg = errorText.ToString() });
            }
            else
            {
                return Json(new {sucess = 1});
            }
           
        }
    }
}
