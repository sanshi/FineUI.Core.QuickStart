using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FineUI.Core;
using System;
using System.Data;

namespace FineUI.Core.QuickStart
{
    public partial class MovieModel : BaseModel
    {
        #region Fields

        public List<Movie> Movies { get; set; }

        #endregion

        #region Page_Load

        protected async Task Page_LoadAsync(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                await LoadDataAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            IQueryable<Movie> q = DB.Movies;

            // 搜索框
            var searchMessage = ttbSearchMessage.Text?.Trim();
            if (!string.IsNullOrEmpty(searchMessage))
            {
                q = q.Where(s => s.Title.Contains(searchMessage));
            }

            // 获取总记录数（在添加条件之后，排序和分页之前）
            Grid1.RecordCount = await q.CountAsync();

            // 排列和数据库分页
            q = SortAndPage<Movie>(q, Grid1);

            // 绑定表格数据
            Grid1.DataSource = await q.ToListAsync();
            Grid1.DataBind();
        }

        #endregion

        #region Events

        protected async Task Window1_CloseAsync(object sender, WindowCloseEventArgs e)
        {
            await LoadDataAsync();
        }

        protected async Task Grid1_SortAsync(object sender, GridSortEventArgs e)
        {
            await LoadDataAsync();
        }

        protected async Task Grid1_PageIndexChangedAsync(object sender, GridPageEventArgs e)
        {
            await LoadDataAsync();
        }

        protected async Task ttbSearchMessage_Trigger1ClickAsync(object sender, EventArgs e)
        {
            // 清空输入框的内容，并隐藏清空图标
            ttbSearchMessage.Text = String.Empty;
            ttbSearchMessage.ShowTrigger1 = false;

            await LoadDataAsync();
        }

        protected async Task ttbSearchMessage_Trigger2ClickAsync(object sender, EventArgs e)
        {
            // 显示清空图标
            ttbSearchMessage.ShowTrigger1 = true;

            await LoadDataAsync();
        }


        #endregion


        #region Grid1_RowCommand

        protected async Task Grid1_RowCommandAsync(object sender, GridRowCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                // 删除行
                var rowID = Convert.ToInt32(e.RowID);

                var movie = await DB.Movies.FindAsync(rowID);
                if (movie == null)
                {
                    Alert.Show("指定的电影不存在！");
                    return;
                }

                DB.Movies.Remove(movie);
                await DB.SaveChangesAsync();

                await LoadDataAsync();
            }
        }

        #endregion


    }
}
