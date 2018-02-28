using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace UI.Controllers
{
    public class PostTestController : Controller
    {
        //
        // GET: /PostTest/

        public ActionResult Index()
        {
            HttpHelper httpHelper = new HttpHelper();
            //string EnterStation = System.Web.HttpUtility.UrlEncode("天府广场", System.Text.Encoding.UTF8);
            //string EnterTime = System.Web.HttpUtility.UrlEncode("2017/06/30 14:40:10.123", System.Text.Encoding.UTF8);
            //string ExitStation = System.Web.HttpUtility.UrlEncode("春熙路", System.Text.Encoding.UTF8);
            //string ExitTime = System.Web.HttpUtility.UrlEncode("2017/06/30 14:45:10.123", System.Text.Encoding.UTF8);


            //string EnterStation = "天府广场";
            //string EnterTime = "2017/06/30 14:40";
            //string ExitStation = "春熙路";
            //string ExitTime = "2017/06/30 14:45";
            ////UID=e9d5bd103ec64bf1abbcd45a1abbf13d&Price=1&EnterStation=天府广场&EnterTime=2017-06-30 14:40:10.123&ExitStation=春熙路&ExitTime=2017-06-30 14:45:10.123&TransactionID=556456
            //string data = string.Format("UID=e9d5bd103ec64bf1abbcd45a1abbf13d&Price=1&EnterStation={0}&EnterTime={1}&ExitStation={0}&ExitTime={3}&TransactionID=556456", EnterStation, EnterTime, ExitStation, ExitTime);
            //string response = httpHelper.HttpPost("http://119.23.231.113:8080/travel/outStation", data);

            ////webapi 单参数
            //string data = string.Format("={0}", "13123");
            //string response = httpHelper.HttpPost("http://localhost:8936/api/Test/UploadTest", data);


        //    {
        //    "Phone": "0X57894asdfjkjkl",
        //    "UID":"5K8264ILTKCH16CQ2502SI8ZNMTM67VS",
        //    "EnterTime": "2017-06-23 10:10:10.123",
        //    "EnterStation":"0101",
        //    "DeviceID":"010101123",
        //    "TransactionID":"01010112320170627101010123",
        //    "OpeMark": "1",
        //    "UploadTime":"2017-06-23 10:10:10.123"
        //}
            //string data = "{Phone: \"5K8264ILTKCH16CQ2502SI8ZNMTM67VS\",UID:\"e89364a286914cc699a4616e1e081673\",EnterTime: \"2017-06-23 10:10:10.123\",EnterStation:\"升仙湖站\" ,DeviceID: \"010101123\",TransactionID:\"01010112320170627101010123\" ,OpeMark:\"1\",UploadTime:\"2017-07-04 15:00:10.123\" }";
            //string response = httpHelper.HttpPost("http://localhost:5078/api/ACC/DataHandler", data);
            //string data = "{Phone: \"13880825220\",UID:\"e89364a286914cc699a4616e1e081673\",EnterTime: \"2017-07-06 10:10:10.123\",EnterStation:\"升仙湖站\" ,DeviceID: \"010101123\",TransactionID:\"01010112320170627101010123\" ,OpeMark:\"1\",UploadTime:\"2017-07-06 13:29:10.123\" }";
            //string enterdata = "{Phone: \"13880825220\",UID:\"e89364a286914cc699a4616e1e081673\",EnterTime: \"2017-07-06 10:10:10.123\",EnterStation:\"0321\" ,DeviceID: \"010101123\",TransactionID:\"01010112320170627101010123\" ,OpeMark:\"1\",UploadTime:\"2017-07-06 13:29:10.123\" }";
            //string exitdata = "{Phone: \"13880825220\",UID:\"e89364a286914cc699a4616e1e081673\",ExitTime: \"2017-07-06 10:10:10.123\",ExitStation:\"0421\" ,DeviceID: \"010101123\",TransactionID:\"01010112320170627101010123\" ,OpeMark:\"2\",UploadTime:\"2017-07-06 17:32:10.123\" }";

            string enterdata = "Phone=13880825220&UID=e89364a286914cc699a4616e1e081673&EnterTime=2017-07-06 10:10:10.123&EnterStation=0321&DeviceID=010101123&TransactionID=01010112320170627101010123&OpeMark=1&UploadTime=2017-07-06 13:29:10.123";

            //string response = httpHelper.HttpPost("http://222.211.86.180:8055/api/ACC/DataHandler", enterdata);
            //string response = httpHelper.HttpPost("http://localhost:5078/api/ACC/DataHandler", data);
            string response = httpHelper.HttpPostForm("http://222.211.86.180:8055/api/ACC/DataHandler", enterdata);
            ViewData["html"] = response;
            return View();
        }

    }
}
