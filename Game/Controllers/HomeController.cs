using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Game.Models;

namespace Game.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            /*
            User_Info userInfo=new User_Info();
            userInfo.UserName = "张三";
            userInfo.AddDateTime=DateTime.Now;

           var obj= DBUtility.DataFactory.Insert(userInfo);
            */



            return View();
        }




    }
}
