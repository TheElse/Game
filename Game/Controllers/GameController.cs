using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DBUtility;
using Game.Common;
using Game.Models;

namespace Game.Controllers
{
    public class GameController : Controller
    {
        //
        // GET: /Game/

        public ActionResult Index()
        {
            //int times = 0;
            //string sql = string.Format("select * from Game_Times");
            //Game_Times gameTimes = DBUtility.DataFactory.Query<Game_Times>(sql, "").SingleOrDefault();
            //if (gameTimes == null)
            //{
            //    times = 1;
            //}
            //else
            //{
            //    times = gameTimes.Times + 1;
            //}
            ViewBag.Times = GetTimes();
           // ViewBag.PerCentList= DBUtility.DataFactory.GetList<Base_PayGameLossPerCent>();//获取赔率

            return View();
        }

        [HttpPost]
        /// <summary>
        /// 获取用户统计信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public JsonResult UserTotalRecord(string id)
        {
            try
            {
               // string sql2 = string.Format("select * from User_Info where UserName like '%{0}%'", userName);
                string sql2 = string.Format("select * from User_Info where userid={0}", id);
                User_Info userInfo = DataFactory.Query<User_Info>(sql2, "").FirstOrDefault();
                GameTotalEntity entity = new GameTotalEntity();
                /*没有找到相关的用户信息,返回json*/
                if (userInfo == null)
                {
                    entity.AmountMoney = 0;
                    entity.MonthAmount = 0;
                    entity.MonthSFTime = 0;
                    entity.Point = 0;
                    entity.TotalMoney = 0;
                    return Json(new { totalData = entity, UserId = 0, sucess = 1 });
                }


                int userId = userInfo.UserId;
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month;

                #region SQl语句

                string sql = string.Format(@"declare @Point		decimal(18,2);   --积分
declare @TotalMoney decimal(18,2);   --总充值额
declare @AmountMoney decimal(18,2);  --可用余额
declare @MonthAmount decimal(18,2);   --月充值总额
declare @MonthSFTime decimal(18,2);   --月统计输赢
set @Point=0;
set @TotalMoney=0;
set  @AmountMoney=0;
set @MonthAmount=0;
set @MonthSFTime=0;


select @Point=a.Point  --积分
       ,@TotalMoney=b.TotalMoney  --总充值额
	   ,@AmountMoney=b.AmountMoney --可用余额
 from Base_Point as a inner join Base_Account as b on a.UserId=b.UserId
 and b.UserId={0}

 select @MonthAmount=sum(Amount) from Base_AccountMonthRecord where userid={0} and [YEAR]='{1}' and [MONTH]='{2}'

 select @MonthSFTime=sum(CASE WHEN IsWin=0 THEN BettingAmount*-1 ELSE BettingAmount END) from Game_PayRecord where userid={0} and [YEAR]='{1}' and [month]='{2}'

 select isnull(@Point,0) as Point
        ,isnull(@TotalMoney,0) as TotalMoney
		,isnull(@AmountMoney,0) as AmountMoney
		,isnull(@MonthAmount,0) as MonthAmount
		,isnull(@MonthSFTime,0) as MonthSFTime", userId, year, month);

                #endregion

                entity = DBUtility.DataFactory.Query<GameTotalEntity>(sql, "").FirstOrDefault();
                if (entity != null)
                {
                    return Json(new { totalData = entity,UserId=userId, sucess = 1 });
                }
                else
                {
                    entity=new GameTotalEntity();
                    entity.AmountMoney = 0;
                    entity.MonthAmount = 0;
                    entity.MonthSFTime = 0;
                    entity.Point = 0;
                    entity.TotalMoney = 0;
                    return Json(new { totalData = entity, UserId = userId, sucess = 1 });
                }
            }
            catch (Exception ex)
            {
                return Json(new { sucess = -99,msg=ex.Message });
            }
           
        }

        [HttpPost]
        public JsonResult GamePay(string gameParamentItem, string typeWin, int times)
        {
            try
            {
                /*
           * gameParamentItem数据格式：
           * 用户ID|类型|下注金额|购买类型,
           * 5|0|1000|,liuhao|0|1000,
           */
                DateTime nowDateTime = DateTime.Now; //当前时间
                decimal totalMoney = 0; //本次下注金额

                //string paramentItem = "5|0|1000|,4|1|2000";
                string[] userItemArray = gameParamentItem.Split(',');
                if (userItemArray.Length > 0)
                {
                    foreach (string uRecodItem in userItemArray)
                    {
                        /*每个玩家的投注记录情况*/
                        if (!string.IsNullOrWhiteSpace(uRecodItem))
                        {
                            string[] recordInfo = uRecodItem.Split('|');
                            Game_PayRecord gamePayRecord = new Game_PayRecord();
                            gamePayRecord.UserId = Convert.ToInt32(recordInfo[0]); //用户ID
                            gamePayRecord.TYPE = Convert.ToInt32(recordInfo[1]); //下注类型
                            gamePayRecord.Times = times; //局数
                            gamePayRecord.BettingAmount = Convert.ToDecimal(recordInfo[2]); //下注金额
                            gamePayRecord.AddDatetime = nowDateTime;
                          //  gamePayRecord.IsWin = gamePayRecord.TYPE == typeWin ? 1 : 0; //1是赢  0是输
                            gamePayRecord.IsWin = typeWin.Contains(gamePayRecord.TYPE.ToString()) ? 1 : 0; //1是赢  0是输
                            gamePayRecord.Year = nowDateTime.Year.ToString();
                            gamePayRecord.Month = nowDateTime.Month.ToString();

                            totalMoney += gamePayRecord.BettingAmount;
                            DataFactory.Insert(gamePayRecord);
                          
                            //执行存储过程做操作
                            DataFactory.ExecuteNonQueryProc("proc_PayRecord_Update", new { UserId = gamePayRecord.UserId, IsWin = gamePayRecord.IsWin, BettingAmount = gamePayRecord.BettingAmount, Times = gamePayRecord.Times, Type = typeWin });//指定了过多的参数
                        }
                    }

                    # region 记录本次的投注总额

                    /*本局的下注总额、什么类型赢  */
                    Game_TimeResult gameTimeResult = new Game_TimeResult();
                    gameTimeResult.Times = times;
                    gameTimeResult.TotalMoney = totalMoney; //下注总额
                    gameTimeResult.Type = typeWin; //本次输赢的记录
                    gameTimeResult.AddDateTime = nowDateTime;
                    DataFactory.Insert(gameTimeResult);

                    #endregion
                }
                return Json(new {sucess = 1, msg = "操作成功!"});
            }
            catch (Exception ex)
            {
                return Json(new {sucess = -99, msg = ex.Message});
            }
        }

        /// <summary>
        /// 第二种游戏规则算法
        /// </summary>
        /// <param name="gameParamentItem"></param>
        /// <param name="typeWin"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GamePayTotal(string gameParamentItem, string typeWin, int times)
        {
            try
            {
                /*
           * gameParamentItem数据格式：
           * 用户ID|类型|下注金额|购买类型,
           * 5|0|1000|,liuhao|0|1000,
           */
                DateTime nowDateTime = DateTime.Now; //当前时间
                decimal totalMoney = 0; //本次下注金额

                //string paramentItem = "5|0|1000|,4|1|2000";
                string[] userItemArray = gameParamentItem.Split(',');
                IEnumerable<Base_PayGameLossPerCent> basePayGameLossPerCents = GetPayGameLossPerCent(typeWin);
             
                if (userItemArray.Length > 0)
                {
                    foreach (string uRecodItem in userItemArray)
                    {
                        /*每个玩家的投注记录情况*/
                        if (!string.IsNullOrWhiteSpace(uRecodItem))
                        {
                            string[] recordInfo = uRecodItem.Split('|');
                            Game_PayRecord gamePayRecord = new Game_PayRecord();
                            gamePayRecord.UserId = Convert.ToInt32(recordInfo[0]); //用户ID
                            gamePayRecord.TYPE = Convert.ToInt32(recordInfo[1]); //下注类型
                            gamePayRecord.Times = times; //局数
                            gamePayRecord.BettingAmount = Convert.ToDecimal(recordInfo[2]); //下注金额
                            gamePayRecord.AddDatetime = nowDateTime;
                            gamePayRecord.IsWin = typeWin.Contains(gamePayRecord.TYPE.ToString()) ? 1 : 0; //1是赢  0是输
                            gamePayRecord.Year = nowDateTime.Year.ToString();
                            gamePayRecord.Month = nowDateTime.Month.ToString();

                            totalMoney += gamePayRecord.BettingAmount;
                            DataFactory.Insert(gamePayRecord);

                            decimal lossPerCent = 1;
                           var lossPerCentsItem=    basePayGameLossPerCents.Where(m => m.type == gamePayRecord.TYPE).SingleOrDefault();
                            if (lossPerCentsItem != null)
                            {
                                lossPerCent = lossPerCentsItem.LossPerCent;
                                if (gamePayRecord.IsWin == 1)
                                {
                                    gamePayRecord.BettingAmount = gamePayRecord.BettingAmount*lossPerCent;
                                }
                            }

                            bool isCenter = false;
                            if (typeWin.Contains("1") && (gamePayRecord.TYPE == 0 || gamePayRecord.TYPE == 2)) //开和
                            {
                                isCenter = true;
                            }

                            //执行存储过程做操作
                            if (!isCenter)
                            {
                                DataFactory.ExecuteNonQueryProc("proc_PayGameRecord_Update",
                                    new { UserId = gamePayRecord.UserId, 
                                          IsWin = gamePayRecord.IsWin, 
                                          BettingAmount = gamePayRecord.BettingAmount, 
                                          Times = gamePayRecord.Times, 
                                         
                                          LossPerCent = lossPerCent
                                    });
                            }
                           
                        }
                    }

                    # region 记录本次的投注总额

                    /*本局的下注总额、什么类型赢  */
                    Game_TimeResult gameTimeResult = new Game_TimeResult();
                    gameTimeResult.Times = times;
                    gameTimeResult.TotalMoney = totalMoney; //下注总额
                    gameTimeResult.Type = typeWin; //本次输赢的记录
                    gameTimeResult.AddDateTime = nowDateTime;
                    DataFactory.Insert(gameTimeResult);

                    #endregion
                }
                return Json(new { sucess = 1, msg = "操作成功!" });
            }
            catch (Exception ex)
            {
                return Json(new { sucess = -99, msg = ex.Message });
            }
        }



        /// <summary>
        /// 获取赔率
        /// </summary>
        /// <param name="typeWin"></param>
        /// <returns></returns>
        public IEnumerable<Base_PayGameLossPerCent> GetPayGameLossPerCent(string typeWin)
        {
            string[] typeArrary = typeWin.Trim(',').Split(',');
            string sqlWhere = string.Empty;
            if (typeArrary.Length > 0)
            {
                for (int i = 0; i < typeArrary.Length; i++)
                {
                    if (i + 1 == typeArrary.Length)
                    {
                        sqlWhere += string.Format("{0}", typeArrary[i]);
                    }
                    else
                    {
                        sqlWhere += string.Format("{0},", typeArrary[i]);
                    }
                }
            }
            string sql = string.Format(" SELECT * FROM Base_PayGameLossPerCent WHERE [type] IN({0})", sqlWhere);
            var list = DataFactory.Query<Base_PayGameLossPerCent>(sql, "");

            return list;
        }


        private string GetTimes()
        {
            int times = 0;
            string sql = string.Format("select * from Game_Times");
            Game_Times gameTimes = DBUtility.DataFactory.Query<Game_Times>(sql, "").SingleOrDefault();
            if (gameTimes == null)
            {
                times = 1;
            }
            else
            {
                times = gameTimes.Times;
                // gameTimes.Times = times;
                // DataFactory.Update(gameTimes);//更新局数
            }
            return times.ToString();
        }

        public ActionResult AddTimes()
        {
            int times = 0;
            string sql = string.Format("select * from Game_Times");
            Game_Times gameTimes = DBUtility.DataFactory.Query<Game_Times>(sql, "").FirstOrDefault();

            if (gameTimes == null)
            {
                gameTimes = new Game_Times();
                times = 1;
                gameTimes.Times = times;
                gameTimes.AddDateTime=DateTime.Now;
                DataFactory.Insert(gameTimes);//更新局数

            }
            else
            {
                times = gameTimes.Times+1;
                gameTimes.Times = times;
                DataFactory.Update(gameTimes);//更新局数
            }
            return RedirectToAction("Index"); //跳转到index页面

        }

        public void ImportGameResult(int times)
        {
                #region  SQL语句
                        /*查询参与游戏的用户--sql*/
                        string sql = string.Format(@"

            SELECT b.* FROM 
            ( SELECT DISTINCT UserId FROM Game_PayRecord 
            WHERE Times={0}
            ) AS a INNER JOIN User_Info AS b ON a.UserId=b.UserId
            ", times);
                        /*统计用户投注的金额问题--sql*/
                        string sql2 = string.Format(@"
            SELECT SUM(BettingAmount) AS TotalMoney
	               ,a.UserId
	               ,a.Times
	               ,a.[TYPE]
	  
            FROM Game_PayRecord AS a
            WHERE Times={0}
            GROUP BY a.UserId
	               ,a.Times
	               ,a.[TYPE]  
            ", times);
            #endregion

            var userInfoList = DataFactory.Query<UserInfo>(sql,"");
            var gameResultList = DataFactory.Query<GameResultImport>(sql2,"");

            List<GameResultReort> gameResultReorts = new List<GameResultReort>();


            string endRowUserName = "投注汇总";
            decimal endResult0 = 0;
            decimal endResult1 = 0;
            decimal endResult2 = 0;
            decimal endResult3 = 0;
            decimal endResult4 = 0;


            if (userInfoList != null && gameResultList != null)
            {
                foreach (UserInfo userInfo in userInfoList)
                {
                    GameResultReort gameResult = new GameResultReort();
                    gameResult.UserName = userInfo.UserName; //用户名
                    var result0 = gameResultList.Where(m => m.UserId == userInfo.UserId && m.TYPE == 0).FirstOrDefault();
                    if (result0 != null)
                    {
                        gameResult.Result0 = result0.TotalMoney.ToString();
                        endResult0 += result0.TotalMoney;
                    }
                    else
                    {
                        gameResult.Result0 = "";
                    }

                    var result1 = gameResultList.Where(m => m.UserId == userInfo.UserId && m.TYPE == 1).FirstOrDefault();
                    if (result1 != null)
                    {
                        gameResult.Result1 = result1.TotalMoney.ToString();
                        endResult1 += result1.TotalMoney;
                    }
                    else
                    {
                        gameResult.Result1 = "";
                    }

                    var result2 = gameResultList.Where(m => m.UserId == userInfo.UserId && m.TYPE == 2).FirstOrDefault();
                    if (result2 != null)
                    {
                        gameResult.Result2 = result2.TotalMoney.ToString();
                        endResult2 += result2.TotalMoney;
                    }
                    else
                    {
                        gameResult.Result2 = "";
                    }

                    var result3 = gameResultList.Where(m => m.UserId == userInfo.UserId && m.TYPE == 3).FirstOrDefault();
                    if (result3 != null)
                    {
                        gameResult.Result3 = result3.TotalMoney.ToString();
                        endResult3 += result3.TotalMoney;
                    }
                    else
                    {
                        gameResult.Result3 = "";
                    }

                    var result4 = gameResultList.Where(m => m.UserId == userInfo.UserId && m.TYPE == 4).FirstOrDefault();
                    if (result4 != null)
                    {
                        gameResult.Result4 = result4.TotalMoney.ToString();
                        endResult4 += result4.TotalMoney;
                    }
                    else
                    {
                        gameResult.Result4 = "";
                    }

                    gameResultReorts.Add(gameResult);
                }

                //加入一行空
                gameResultReorts.Add(new GameResultReort()
                {
                    UserName = "",
                    Result0 = "",
                    Result1 = "",
                    Result2 = "",
                    Result3 = "",
                    Result4 = ""
                });

                gameResultReorts.Add(new GameResultReort()
                {
                    UserName = endRowUserName,
                    Result0 = endResult0.ToString(),
                    Result1 = endResult1.ToString(),
                    Result2 = endResult2.ToString(),
                    Result3 = endResult3.ToString(),
                    Result4 = endResult4.ToString()
                });
            }


            Dictionary<string, string> userdics = new Dictionary<string, string>();
            userdics.Add("UserName", "选手");
            userdics.Add("Result0", "闲");
            userdics.Add("Result2", "庄");
            userdics.Add("Result4", "闲对");
            userdics.Add("Result3", "庄对");
            userdics.Add("Result1", "和");
            

            string[] width = new string[6];
            width[0] = "30";
            width[1] = "30";
            width[2] = "20";
            width[3] = "50";
            width[4] = "50";
            width[5] = "50";
            string headTitle = string.Format("第{0}局", times);
            AsposeExcel.ExportData(width, userdics, gameResultReorts, headTitle);



        }
    }
}
