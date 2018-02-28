using Ninject;
using BLL;
using DAL;
using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  NinjectDI
{
    public class HARDWARE_STATE_NinjectDI
    {
        //public HARDWARE_STATE_BLL getBll()
        //{
            //#region Ninject DI依赖注入
             
            ////创建Ninject内核实例  前者为Ikernel接口 ，再用StandardKernel类作为接口的实例化
            //IKernel ninjectKernel = new StandardKernel();

            ////接口绑定实现接口的实例
            //ninjectKernel.Bind<I_HARDWARE_STATE_DAL>().To<HARDWARE_STATE_DAL>();

            ////获取接口实现
            //I_HARDWARE_STATE_DAL idal = ninjectKernel.Get<I_HARDWARE_STATE_DAL>();

            ////依赖注入-实现接口的实例传给构造函数
            //HARDWARE_STATE_BLL bll = new HARDWARE_STATE_BLL(idal);

            //return bll;
            //#endregion
        //}

    }
}
