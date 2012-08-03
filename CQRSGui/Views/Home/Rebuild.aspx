<%@ Page Title="Title" Language="C#" Inherits="System.Web.Mvc.ViewPage<CQRSGui.Controllers.RebuildReadModelModel>" MasterPageFile="~/Views/Shared/Site.Master" %>
<asp:Content runat="server" ID="Title" ContentPlaceHolderID="TitleContent"></asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">
    Rebuild the read model? This might take some time!
        <% using (Html.BeginForm())
       {%>
        <%: Html.TextBox("ToVersion", Model.ToVersion)%>

        <button name="submit">Yes, please!</button>
    <%
       }%>
</asp:Content>


