<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Status.aspx.cs" Inherits="SPWebCache.Layouts.SPWebCache.Status" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <style type="text/css">
        .tbl {
            width: 100%;
        }
        .tbl th {
            text-align: left;
            background: darkslateblue;
            color: white;
        }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div >
        <div style="float: left; margin: 10px; width: 48%;">
            <fieldset>
                <legend>Sites</legend>
                
                <table class="tbl">
                    <tr>
                        <th>Url</th>
                        <th>Id</th>
                        <th>User</th>
                        <th>LastUsed (sec ago)</th>
                        <th>Locked</th>
                    </tr>
                    <% foreach (Row row in Sites) { %>
                      <tr>
                          <td><%= row.Url %></td>
                          <td><%= row.Id %></td>
                          <td><%= row.User %></td>
                          <td><%= row.LastUsed %></td>
                          <td><%= row.Locked %></td>
                      </tr>     
                    <%   } %>
                </table>

            </fieldset>
        </div>
        <div style="float: right; margin: 10px; width:48%;">
              <fieldset>
                <legend>Webs</legend>
                  
                <table class="tbl">
                    <tr>
                        <th>Url</th>
                        <th>Id</th>
                        <th>User</th>
                        <th>LastUsed (sec ago)</th>
                        <th>Locked</th>
                    </tr>
                    <% foreach (Row row in Webs) { %>
                      <tr>
                          <td><%= row.Url %></td>
                          <td><%= row.Id %></td>
                          <td><%= row.User %></td>
                          <td><%= row.LastUsed %></td>
                          <td><%= row.Locked %></td>
                      </tr>     
                    <%   } %>
                </table>

            </fieldset>
        </div>
    </div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Web cache status
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Web cache status
</asp:Content>
