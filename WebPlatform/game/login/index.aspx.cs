using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class game_login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        HttpCookie cookie = new HttpCookie("GAMEGPVLU", "CE5C19C174CF1C8700F2B3669C9429FE4E29C0E366EDE0C76873E5CDE50D124608F3125CE54B58FCDCC9CD4623B9D91F396921B0660BFEACE5FAC3C06BDCCDE8");
        cookie.Domain = "mail.moeangel.com";
        cookie.Path = "/";
        cookie.HttpOnly = true;
        Response.SetCookie(cookie);
    }
}