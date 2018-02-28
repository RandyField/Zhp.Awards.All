using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Collections;
using System.Reflection;
using Interface;
using Model;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel;
using Common;
using EFModel;


namespace DAL
{
    /// <summary>
    /// 张登
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <typeparam name="U">EFModel</typeparam>
    public  class BaseDAL<U> : IDisposable,I_BASE_Interface<U>
        where U : class,new()
    {
        //实例化数据库上下文
        public ZhpRedEntities dbcontext = null;
        #region Field
        protected string primaryKey;//数据库的主键字段名
        protected string sortField = string.Empty;//排序字段
        private bool isDescending = true;//是否降序排列 
        private bool _disposed;

        #endregion

        #region Constructor
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public BaseDAL()
        {
            GetDbcontext();
        }

        /// <summary>
        /// 有参构造函数
        /// </summary>
        /// <param name="primaryKey">主键</param>
        public BaseDAL(string primaryKey)
        {
            this.primaryKey = primaryKey;
        }
        #endregion

        /// <summary>
        /// 构造函数 数据库上下文
        /// </summary>
        public void GetDbcontext()
        {
            dbcontext = new ZhpRedEntities();
        }


        /// <summary>
        /// 所有
        /// </summary>
        public virtual IQueryable<U> FindAll
        {
            get
            {
                return GetAll();
            }
        }

        /// <summary>
        /// 返回dataset
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<U> GetAll()
        {
            return dbcontext.Set<U>();
        }


        public IQueryable<U> IQueryable()
        {
            return dbcontext.Set<U>();
        }

        /// <summary>
        /// 要查找的实体的主键值
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public virtual U Find(params object[] keyValues)
        {
            return dbcontext.Set<U>().Find(keyValues);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public virtual IQueryable<U> FindBy(Expression<Func<U, bool>> exp)
        {
            return dbcontext.Set<U>().Where(exp);
        }


        public virtual IQueryable<U> FindBy(Expression<Func<U, bool>> exp1,Expression<Func<U, bool>> exp2)
        {
            return dbcontext.Set<U>().Where(exp1).Where(exp2);
        }

        public virtual IQueryable<U> FindBy(Expression<Func<U, bool>> exp1, Expression<Func<U, bool>> exp2, Expression<Func<U, bool>> exp3)
        {
            return dbcontext.Set<U>().Where(exp1).Where(exp2).Where(exp3);
        }

        public virtual IQueryable<U> FindBy(Expression<Func<U, bool>> exp1, Expression<Func<U, bool>> exp2, Expression<Func<U, bool>> exp3, Expression<Func<U, bool>> exp4)
        {
            return dbcontext.Set<U>().Where(exp1).Where(exp2).Where(exp3).Where(exp4);
        }


        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Add(U entity)
        {
            dbcontext.Set<U>().Add(entity);
        }


        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="list"></param>
        public virtual void BulkInsert(List<U> list)
        {
            //表名
            var tblName = typeof(U).Name;
            BulkInsert(dbcontext.Database.Connection.ConnectionString, tblName, list);
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="list"></param>
        public static void BulkInsert(string connection, string tableName, IList<U> list)
        {
            using (var bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.BatchSize = list.Count;
                bulkCopy.DestinationTableName = tableName;

                var table = new DataTable();
                var props = TypeDescriptor.GetProperties(typeof(U))
                    //Dirty hack to make sure we only have system data types
                    //i.e. filter out the relationships/collections
                                           .Cast<PropertyDescriptor>()
                                           .Where(propertyInfo => propertyInfo.PropertyType.Namespace != null
                                               && propertyInfo.PropertyType.Namespace.Equals("System"))
                                           .ToArray();

                foreach (var propertyInfo in props)
                {
                    bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                    table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                }

                var values = new object[props.Length];
                foreach (var item in list)
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item);
                    }

                    table.Rows.Add(values);
                }

                bulkCopy.WriteToServer(table);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Delete(U entity)
        {
            dbcontext.Set<U>().Remove(entity);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Edit(U entity)
        {
            dbcontext.Entry(entity).State = (EntityState.Modified);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="insertExpression"></param>
        public virtual void Upsert(U entity, Func<U, bool> insertExpression)
        {
            if (insertExpression(entity))
            {
                Add(entity);
            }
            else
            {
                Edit(entity);
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        public virtual void Save()
        {
            dbcontext.SaveChanges();
        }

        /// <summary>
        /// EF分页查询
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordTotal"></param>
        /// <param name="pageCount"></param>
        /// <param name="whLamdba"></param>
        /// <returns></returns>
        public virtual List<U> PageQuery(int pageIndex, int pageSize, out int recordTotal, out int pageCount, Expression<Func<U, bool>> whLamdba)
        {
            try
            {
                IQueryable<U> list = dbcontext.Set<U>();                 
                recordTotal = list.Count();
                list = list.Where(whLamdba);
                var result = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                pageCount = Convert.ToInt32(Math.Ceiling((double)recordTotal / (double)pageSize));
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// 实体分页查询
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public IEnumerable<U> getSearchListByPage<TKey>(Expression<Func<U, bool>> where, Expression<Func<U, TKey>> orderBy, int pageSize, int pageIndex)
        {
            return dbcontext.Set<U>().Where(where).OrderByDescending(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }


        /// <summary>
        /// 存储过程分页查询
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort"></param>
        /// <param name="where"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public virtual DataTable PageQuery(int page, int pageSize,
            string sort, string where, out int total)
        {
            var viewName = typeof(U).Name;
            var paras = new List<SqlParameter>
                                {
                                    new SqlParameter("tblName", "dbo."+viewName),
                                    new SqlParameter("fldName", "*"),
                                    new SqlParameter("pageSize", pageSize),
                                    new SqlParameter("page", page),
                                    new SqlParameter("fldSort", sort),
                                    new SqlParameter("strCondition", where),
                                    new SqlParameter("pageCount", SqlDbType.Int){Direction = ParameterDirection.Output},
                                };
            var countParameter = new SqlParameter
            {
                ParameterName = "counts",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };
            var strParameter = new SqlParameter("strSql", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output };

            paras.Add(countParameter);
            paras.Add(strParameter);

            var conn = dbcontext.Database.Connection.ConnectionString;
            var ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure,
                                              "dbo.PagedQuery", paras.ToArray());
            total = countParameter.Value == DBNull.Value ? 0 : Convert.ToInt32(countParameter.Value);
            return ds.Tables[0];
        }


        public virtual List<U> PageQueryList(int page, int pageSize,
            string sort, string where, out int total)
        {
            var viewName = typeof(U).Name;
            var paras = new List<SqlParameter>
                                {
                                    new SqlParameter("tblName", "dbo."+viewName),
                                    new SqlParameter("fldName", "*"),
                                    new SqlParameter("pageSize", pageSize),
                                    new SqlParameter("page", page),
                                    new SqlParameter("fldSort", sort),
                                    new SqlParameter("strCondition", where),
                                    new SqlParameter("pageCount", SqlDbType.Int){Direction = ParameterDirection.Output},
                                };
            var countParameter = new SqlParameter
            {
                ParameterName = "counts",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };
            var strParameter = new SqlParameter("strSql", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output };

            paras.Add(countParameter);
            paras.Add(strParameter);

            //var conn = _entities.Database.Connection.ConnectionString;
            //var ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure,
            //                                  "dbo.PagedQuery", paras.ToArray());
            //total = countParameter.Value == DBNull.Value ? 0 : Convert.ToInt32(countParameter.Value);
            var ret = dbcontext.Database.SqlQuery<U>(
                "dbo.PagedQuery @tblName,@fldName,@pageSize,@page,@fldSort,@strCondition,@pageCount out,@counts out,@strSql out",
                paras.ToArray()).ToList();
            total = countParameter.Value == DBNull.Value ? 0 : Convert.ToInt32(countParameter.Value);
            return ret;
        }


        /// <summary>
        /// 按照条件修改数据
        /// </summary>
        /// <param name="where"></param>
        /// <param name="dic"></param>
        public void update(Expression<Func<U, bool>> where, Dictionary<string, object> dic)
        {
            IEnumerable<U> result = dbcontext.Set<U>().Where(where).ToList();
            Type type = typeof(U);
            List<PropertyInfo> propertyList = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).ToList();
            //遍历结果集
            foreach (U entity in result)
            {
                foreach (PropertyInfo propertyInfo in propertyList)
                {
                    string propertyName = propertyInfo.Name;
                    if (dic.ContainsKey(propertyName))
                    {
                        //设置值
                        propertyInfo.SetValue(entity, dic[propertyName], null);
                    }
                }
            }
            dbcontext.SaveChanges();

        }

        /// <summary>
        /// 释放数据库上下文
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                dbcontext.Dispose();
            }
            _disposed = true;
        }

        /// <summary>
        /// 数据库上下文释放 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
