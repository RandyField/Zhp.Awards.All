using Common.Helper;
using EFModel;
using Interface;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BLL
{
    public class TRF_WeChatUserInfo_BLL
    {
        I_TRF_WeChatUserInfo_DAL idal;

        //构造函数或者类的Setter访问器 ， 参数为实现接口的类的实例
        public TRF_WeChatUserInfo_BLL(I_TRF_WeChatUserInfo_DAL i_param)
        {
            idal = i_param;
        }

        /// <summary>
        /// 是否存在此微信用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool isExist(string openId)
        {
            bool flag = false;
            try
            {
                Expression<Func<TRF_WeChatUserInfo, bool>> exp = a => 1 == 1;
                if (!string.IsNullOrWhiteSpace(openId))
                {
                    exp = a => a.openid == openId;
                    var list = idal.FindBy(exp).ToList();
                    if (list.Count > 0)
                    {
                        flag = true;
                    }
                }          
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("判断是否存在此微信用户信息异常，异常信息：{0}",ex.ToString()));
            }

            return flag;
        }

        public List<TRF_WeChatUserInfo> getAll()
        {
            List<TRF_WeChatUserInfo> list = null;
            try
            {
                Expression<Func<TRF_WeChatUserInfo, bool>> exp = a => 1 == 1;
                exp = a => a.sex == null;
                list = idal.FindBy(exp).ToList();
            }
            catch (Exception ex)
            {

                Logger.Error("获取所有微信用户信息异常，异常信息为:"+ex.ToString());
            }
           
            return list;
        }

        /// <summary>
        /// 保存用户微信信息
        /// </summary>
        /// <param name="model"></param>
        public void Save(wxUserInfoModel model)
        {
            try
            {
                TRF_WeChatUserInfo entity = new TRF_WeChatUserInfo();
                entity.openid = model.Openid;
                entity.nickname = model.Nickname;
                entity.province = model.Province;
                entity.city = model.City;
                entity.country = model.Country;
                entity.headimgurl = model.Headimgurl;
                //entity.ID = 1;
                entity.sex = model.Sex;
                idal.Add(entity);
                idal.Save();
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("保存用户信息异常，异常信息：{0}",ex.ToString()));
            }          
        }


        public void SaveJson(wxUserInfoModel model)
        {
            try
            {
                TRF_WeChatUserInfo entity = new TRF_WeChatUserInfo();
                entity.openid = model.user_openid;
                entity.nickname = model.user_name;
                idal.Add(entity);
                idal.Save();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="model"></param>
        public void updateUserInfo(wxUserInfoModel model)
        {
            try
            {
                TRF_WeChatUserInfo entity = new TRF_WeChatUserInfo();

                Expression<Func<TRF_WeChatUserInfo, bool>> exp = a => 1 == 1;

                exp = a => a.openid == model.Openid;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("nickname", model.Nickname);
                dic.Add("province", model.Province);
                dic.Add("city", model.City);
                dic.Add("country", model.Country);
                dic.Add("headimgurl", model.Headimgurl);
                dic.Add("sex", model.Sex);
                idal.update(exp, dic);
                idal.Save();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }          
        }

    }
}
