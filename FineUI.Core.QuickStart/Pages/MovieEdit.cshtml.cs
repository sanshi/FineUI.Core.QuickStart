using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FineUI.Core.QuickStart
{
    public partial class MovieEditModel : BaseModel
    {
        
        #region Fields

        [BindProperty]
        public Movie Movie { get; set; }

        private Movie _storedMovie;

        #endregion

        #region Page_Init

        public async Task Page_InitAsync(object sender, EventArgs e)
        {
            // 本页路由为 /MovieEdit/2，主键在路径段中，不在 ?id 查询参数中。
            int.TryParse(Request.RouteValues["id"]?.ToString(), out var id);
            _storedMovie = await DB.Movies.FirstOrDefaultAsync(m => m.ID == id);

            if (_storedMovie == null)
            {
                FineUI.Core.PageContext.AbortPage("无效参数！", ActiveWindow.HideCommand());
                return;
            }

            if (!IsPostBack)
            {
                Movie = _storedMovie;
            }
        }

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        #endregion


        #region Events

        protected async Task btnSaveClose_ClickAsync(object sender, EventArgs e)
        {
            if (!ModelState.IsValid)
            {
                return;
            }

            // 保存本次按路由主键查到的电影，忽略可修改的表单主键。
            _storedMovie.Title = Movie.Title;
            _storedMovie.ReleaseDate = Movie.ReleaseDate;
            _storedMovie.Genre = Movie.Genre;
            _storedMovie.Price = Movie.Price;

            try
            {
                await DB.SaveChangesAsync();
                // 用户确认修改结果后关闭窗体，再由父页的关闭事件刷新电影列表。
                new Alert
                {
                    Message = "修改成功！",
                    MessageBoxIcon = MessageBoxIcon.Success,
                    OkCommand = ActiveWindow.HidePostBackCommand()
                }.Show();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DB.Movies.Any(e => e.ID == _storedMovie.ID))
                {
                    Alert.Show("指定的电影不存在：" + Movie.Title);
                }
            }
        }

        #endregion


    }
}
