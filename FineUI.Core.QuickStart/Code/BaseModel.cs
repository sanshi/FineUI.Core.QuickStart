using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Web;

namespace FineUI.Core.QuickStart
{
    public class BaseModel : PageModel
    {
        #region IsPostBack

        /// <summary>
        /// 是否页面回发
        /// </summary>
        public bool IsPostBack
        {
            get
            {
                return FineUI.Core.PageContext.IsFineUIAjaxPostBack();
            }
        }

        #endregion

        #region RegisterStartupScript

        /// <summary>
        /// 注册客户端脚本
        /// </summary>
        /// <param name="scripts"></param>
        public void RegisterStartupScript(string scripts)
        {
            FineUI.Core.PageContext.RegisterStartupScript(scripts);
        }

        #endregion

        #region ViewBag

        private DynamicViewData _viewBag;

        /// <summary>
        /// Add ViewBag to PageModel
        /// https://forums.asp.net/t/2128012.aspx?Razor+Pages+ViewBag+has+gone+
        /// https://github.com/aspnet/Mvc/issues/6754
        /// </summary>
        public dynamic ViewBag
        {
            get
            {
                if (_viewBag == null)
                {
                    _viewBag = new DynamicViewData(ViewData);
                }
                return _viewBag;
            }
        }
        #endregion

        #region ShowNotify

        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        public virtual void ShowNotify(string message)
        {
            ShowNotify(message, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageIcon"></param>
        public virtual void ShowNotify(string message, MessageBoxIcon messageIcon)
        {
            ShowNotify(message, messageIcon, Target.Top);
        }

        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageIcon"></param>
        /// <param name="target"></param>
        public virtual void ShowNotify(string message, MessageBoxIcon messageIcon, Target target)
        {
            Notify n = new Notify();
            n.Target = target;
            n.Message = message;
            n.MessageBoxIcon = messageIcon;
            n.PositionX = Position.Center;
            n.PositionY = Position.Top;
            n.DisplayMilliseconds = 3000;
            n.ShowHeader = false;

            n.Show();
        }

        #endregion

        #region DB

        private MovieContext _db;

        /// <summary>
        /// 每个请求共享一个数据库连接实例
        /// </summary>
        protected MovieContext DB
        {
            get
            {
                if (_db == null)
                {
                    _db = BaseModel.GetDbConnection();
                }
                return _db;
            }
        }

        /// <summary>
        /// 获取数据库连接实例（静态方法）
        /// </summary>
        /// <returns></returns>
        public static MovieContext GetDbConnection()
        {
            return FineUI.Core.PageContext.GetRequestService<MovieContext>();
        }


        /// <summary>
        /// 获取实例的属性名称列表
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        private string[] GetReflectionProperties(object instance)
        {
            var result = new List<string>();
            foreach (PropertyInfo property in instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var propertyName = property.Name;
                // NotMapped特性
                var notMappedAttr = property.GetCustomAttribute<NotMappedAttribute>(false);
                if (notMappedAttr == null && propertyName != "ID")
                {
                    result.Add(propertyName);
                }
            }
            return result.ToArray();
        }

        protected IQueryable<T> Sort<T>(IQueryable<T> q, Grid grid)
        {
            return q.SortBy(grid.SortField + " " + grid.SortDirection);
        }


        // 排序
        protected IQueryable<T> Sort<T>(IQueryable<T> q, string sortField, string sortDirection)
        {
            return q.SortBy(sortField + " " + sortDirection);
        }

        protected IQueryable<T> SortAndPage<T>(IQueryable<T> q, Grid grid)
        {
            return SortAndPage(q, grid.PageIndex, grid.PageSize, grid.RecordCount, grid.SortField, grid.SortDirection);
        }


        // 排序后分页
        protected IQueryable<T> SortAndPage<T>(IQueryable<T> q, int pageIndex, int pageSize, int recordCount, string sortField, string sortDirection)
        {
            //// 对传入的 pageIndex 进行有效性验证//////////////
            int pageCount = recordCount / pageSize;
            if (recordCount % pageSize != 0)
            {
                pageCount++;
            }
            if (pageIndex > pageCount - 1)
            {
                pageIndex = pageCount - 1;
            }
            if (pageIndex < 0)
            {
                pageIndex = 0;
            }
            ///////////////////////////////////////////////

            return Sort(q, sortField, sortDirection).Skip(pageIndex * pageSize).Take(pageSize);
        }


        //// 附加实体到数据库上下文中（首先在Local中查找实体是否存在，不存在才Attach，否则会报错）
        //// http://patrickdesjardins.com/blog/entity-framework-4-3-an-object-with-the-same-key-already-exists-in-the-objectstatemanager
        //protected T Attach<T>(int keyID) where T : class, IKeyID, new()
        //{
        //    T t = DB.Set<T>().Local.Where(x => x.ID == keyID).FirstOrDefault();
        //    if (t == null)
        //    {
        //        t = new T { ID = keyID };
        //        DB.Set<T>().Attach(t);
        //    }
        //    return t;
        //}

        //// 向现有实体集合中添加新项
        //protected void AddEntities<T>(ICollection<T> existItems, int[] newItemIDs) where T : class, IKeyID, new()
        //{
        //    foreach (int roleID in newItemIDs)
        //    {
        //        T t = Attach<T>(roleID);
        //        existItems.Add(t);
        //    }
        //}

        //// 替换现有实体集合中的所有项
        //// http://stackoverflow.com/questions/2789113/entity-framework-update-entity-along-with-child-entities-add-update-as-necessar
        //protected void ReplaceEntities<T>(ICollection<T> existEntities, int[] newEntityIDs) where T : class, IKeyID, new()
        //{
        //    if (newEntityIDs.Length == 0)
        //    {
        //        existEntities.Clear();
        //    }
        //    else
        //    {
        //        int[] tobeAdded = newEntityIDs.Except(existEntities.Select(x => x.ID)).ToArray();
        //        int[] tobeRemoved = existEntities.Select(x => x.ID).Except(newEntityIDs).ToArray();

        //        AddEntities<T>(existEntities, tobeAdded);

        //        existEntities.Where(x => tobeRemoved.Contains(x.ID)).ToList().ForEach(e => existEntities.Remove(e));
        //        //foreach (int roleID in tobeRemoved)
        //        //{
        //        //    existEntities.Remove(existEntities.Single(r => r.ID == roleID));
        //        //}
        //    }
        //}

        // http://patrickdesjardins.com/blog/validation-failed-for-one-or-more-entities-see-entityvalidationerrors-property-for-more-details-2
        // ((System.Data.Entity.Validation.DbEntityValidationException)$exception).EntityValidationErrors



        #endregion


    }
}