using EFModel;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace  Interface
{
    /// <summary>
    /// HARDWARE_STATE接口
    /// </summary>
    public interface I_HARDWARE_STATE_DAL : I_BASE_Interface<ZhpRedEntities>
    {
        List<string> GetCityList();
        List<string> GetLineList(string cityName);
        List<string> GetStationNameList(string cityName, string lineName);
        List<string> GetPositionNameList(string cityName, string lineName, string Station);
        //List<LCDSearchModel> SearchPageing(int pageIndex, int pageSize, out int recordTotal, out int pageCount, Expression<Func<LCDSearchModel, bool>> whLamdba);
    }
}
