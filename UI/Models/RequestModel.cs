using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Models
{
    /// <summary>
    /// httpRequest
    /// Author:张登
    /// CreateTime:2017-04-23
    /// </summary>
    public class RequestModel
    {
        public string code { get; set; }
        public string activityId { get; set; }
        public string giftType { get; set; }
        public string giftId { get; set; }
        public string activity { get; set; }
        public string flag { get; set; }
        public string activityName { get; set; }
        public string url { get; set; }
        public string place { get; set; }

        public string TenPaySender { get; set; }
        public string TenPayWish { get; set; }
        public string TenPayGame { get; set; }
        public string TenPayRemark { get; set; }
        //<add key="TenPayV3_Sender" value="苏宁云商"/>
        //<add key="TenPayV3_Wish" value="  苏宁云商祝您快乐!"/>
        //<add key="TenPayV3_Game" value="苏宁KO618"/>
        //<add key="TenPayV3_Remark" value="感谢参与苏宁KO618活动"/>
    }
}