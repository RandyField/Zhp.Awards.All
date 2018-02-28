using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using EFModel;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Data;

namespace Interface
{
    public interface I_BASE_Interface< U>
        where U : class,new()
    {
        #region Interface
        void GetDbcontext();
        IQueryable<U> GetAll();
        IQueryable<U> FindAll { get; }
        IQueryable<U> IQueryable();
        U Find(params object[] keyValues);
        IQueryable<U> FindBy(Expression<Func<U, bool>> exp);
        IQueryable<U> FindBy(Expression<Func<U, bool>> exp1, Expression<Func<U, bool>> exp2);
        IQueryable<U> FindBy(Expression<Func<U, bool>> exp1, Expression<Func<U, bool>> exp2, Expression<Func<U, bool>> exp3);
        IQueryable<U> FindBy(Expression<Func<U, bool>> exp1, Expression<Func<U, bool>> exp2, Expression<Func<U, bool>> exp3, Expression<Func<U, bool>> exp4);
        void Add(U entity);
        void BulkInsert(List<U> list);
        void Delete(U entity);
        void Edit(U entity);
        void Upsert(U entity, Func<U, bool> insertExpression);
        void Save();
        List<U> PageQuery(int pageIndex, int pageSize, out int recordTotal, out int pageCount, Expression<Func<U, bool>> whLamdba);
        IEnumerable<U> getSearchListByPage<TKey>(Expression<Func<U, bool>> where, Expression<Func<U, TKey>> orderBy, int pageSize, int pageIndex);
        DataTable PageQuery(int page, int pageSize, string sort, string where, out int total);
        List<U> PageQueryList(int page, int pageSize, string sort, string where, out int total);
        void update(Expression<Func<U, bool>> where, Dictionary<string, object> dic);
        void Dispose();

        #endregion
    }
}
