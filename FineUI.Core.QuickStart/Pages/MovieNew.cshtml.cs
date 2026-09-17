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

            Alert.Show("保存成功！", string.Empty, MessageBoxIcon.Success, ActiveWindow.GetHidePostBackReference());
        }

    }
}