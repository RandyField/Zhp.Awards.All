using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Common;
using Interface;
using EFModel;
using Model;


namespace  DAL
{
    //public class HARDWARE_STATE_DAL : BaseDAL<LCDSearchModel, ZhpRedEntities>, I_HARDWARE_STATE_DAL
    //{
    //    /// <summary>
    //    /// 获取城市列表
    //    /// </summary>
    //    /// <returns></returns>
    //    public List<string> GetCityList()
    //    {
    //        try
    //        {
    //            var query = from u in dbcontext.TS_SOCKETLIB_CONFIG
    //                        select u;
    //            List<string> list = query.Select(p => p.cityname).Distinct().ToList();
    //            return list;
    //        }
    //        catch (Exception ex)
    //        {

    //            throw ex;
    //        }
    //    }

    //    /// <summary>
    //    /// 获取线路列表
    //    /// </summary>
    //    /// <param name="cityName"></param>
    //    /// <returns></returns>
    //    public List<string> GetLineList(string cityName)
    //    {
    //        try
    //        {
    //            var query = from u in dbcontext.TS_SOCKETLIB_CONFIG
    //                        select u;
    //            query = query.Where(p => p.cityname == cityName);
    //            List<string> list = query.Select(p => p.linename).Distinct().ToList();
    //            return list;
    //        }
    //        catch (Exception ex)
    //        {

    //            throw ex;
    //        }
    //    }


    //    /// <summary>
    //    /// 获取站点列表
    //    /// </summary>
    //    /// <param name="cityName"></param>
    //    /// <returns></returns>
    //    public List<string> GetStationNameList(string cityName, string lineName)
    //    {
    //        try
    //        {
    //            var query = from u in dbcontext.TS_SOCKETLIB_CONFIG
    //                        select u;
    //            query = query.Where(p => p.cityname == cityName);
    //            query = query.Where(p => p.linename == lineName);
    //            List<string> list = query.Select(p => p.stationname).Distinct().ToList();
    //            return list;
    //        }
    //        catch (Exception ex)
    //        {

    //            throw ex;
    //        }
    //    }

    //    /// <summary>
    //    /// 获取站点列表
    //    /// </summary>
    //    /// <param name="cityName"></param>
    //    /// <returns></returns>
    //    public List<string> GetPositionNameList(string cityName, string lineName, string Station)
    //    {
    //        try
    //        {
    //            var query = from u in dbcontext.TS_SOCKETLIB_CONFIG
    //                        select u;
    //            query = query.Where(p => p.cityname == cityName);
    //            query = query.Where(p => p.linename == lineName);
    //            query = query.Where(p => p.stationname == Station);
    //            List<string> list = query.Select(p => p.position).ToList();
    //            return list;
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //    }


        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="start"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <param name="whLamdba"></param>
        /// <returns></returns>
        //public List<LCDSearchModel> SearchPageing(int pageIndex, int pageSize, out int recordTotal, out int pageCount, Expression<Func<LCDSearchModel, bool>> whLamdba)
        //{
        //    try
        //    {
        //        IQueryable<TH_HARDWARE_STATE> HARDWARE_STATE = dbcontext.TH_HARDWARE_STATE;
        //        IQueryable<TS_SOCKETLIB_CONFIG> SOCKETLIB_CONFIG = dbcontext.TS_SOCKETLIB_CONFIG;
        //        var list = HARDWARE_STATE.Join(SOCKETLIB_CONFIG, a => a.computername, b => b.computername, (a, b) => new LCDSearchModel
        //        {
        //            id = a.recordid,
        //            cityName = b.cityname,
        //            lineName = b.linename,
        //            stationName = b.stationname,
        //            position = b.position,
        //            pcName = b.computername,
        //            brokenTime = a.producetime,
        //            errorCode = a.errornum,
        //            screenType = b.projectname
        //        }).Where(whLamdba);

        //        list = list.Where(p => p.screenType.Trim() == "LCD拼接屏");
        //        recordTotal = list.Count();
        //        var result = list.OrderByDescending(t => t.brokenTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

        //        result.ForEach(p => p.brokenTimeStr = Convert.ToDateTime(p.brokenTime).ToString("yyyy-MM-dd HH:mm:ss"));
        //        result.ForEach(p => p.errorInfo = ParseErrorCode.GetErrorInfo(p.errorCode));

        //        pageCount = Convert.ToInt32(Math.Ceiling((double)recordTotal / (double)pageSize));
        //        return result;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
    //}
}
