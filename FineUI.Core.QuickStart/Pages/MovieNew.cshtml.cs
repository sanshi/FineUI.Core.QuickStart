using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FineUI.Core.QuickStart
{
    public partial class MovieNewModel : BaseModel
    {
        #region Fields

        [BindProperty]
        public Movie Movie { get; set; }

        #endregion

        public void OnGet()
        {

        }

        protected async Task btnSaveClose_ClickAsync(object sender, EventArgs e)
        {
            if (!ModelState.IsValid)
            {
                return;
            }

            DB.Movies.Add(Movie);
            await DB.SaveChangesAsync();

            // 用户确认保存结果后关闭窗体，再由父页的关闭事件刷新电影列表。
            new Alert
            {
                Message = "保存成功！",
                MessageBoxIcon = MessageBoxIcon.Success,
                OkCommand = ClientCommand.HideActiveWindowPostBack()
            }.Show();
        }

    }
}
