﻿<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CorrMgr._Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1><%: Title %>Correspondence Game Editor</h1>
            </hgroup>
            <br />
            <asp:TreeView ID="gamesList" runat="server" ImageSet="BulletedList3" ShowExpandCollapse="False" ShowLines="True">
                <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                <NodeStyle Font-Names="Verdana" Font-Size="8pt" ForeColor="Black" HorizontalPadding="0px" NodeSpacing="0px" VerticalPadding="0px" />
                <ParentNodeStyle Font-Bold="False" />
                <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px" VerticalPadding="0px" />
            </asp:TreeView>
            <asp:TextBox ID="boardDisplay" runat="server" Font-Names="Chess-7" Height="191px" Rows="10" TextMode="MultiLine" Width="195px">some textb</asp:TextBox>
            <asp:GridView ID="corrGridView" runat="server" style="float:right" Width="651px" >
                <Columns>
                    <asp:BoundField DataField="MoveNbr" HeaderText="#" />
                    <asp:BoundField DataField="WhiteMove" HeaderText="White" />
                    <asp:BoundField DataField="WMoveTime" HeaderText="Time" />
                    <asp:BoundField DataField="WReflTime" HeaderText="Refl" />
                    <asp:BoundField DataField="BlackMove" HeaderText="Black" />
                    <asp:BoundField DataField="BMoveTime" HeaderText="Time" />
                    <asp:BoundField DataField="BReflTime" HeaderText="Refl" />
                </Columns>
            </asp:GridView>
        </div>
    </section>
    <br />
    <br />
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h3>We suggest the following:</h3>
    <ol class="round">
        <li class="one">
            <h5>Getting Started</h5>
            ASP.NET Web Forms lets you build dynamic websites using a familiar drag-and-drop, event-driven model.
            A design surface and hundreds of controls and components let you rapidly build sophisticated, powerful UI-driven sites with data access.
            <a href="http://go.microsoft.com/fwlink/?LinkId=245146">Learn more…</a>
        </li>
        <li class="two">
            <h5>Add NuGet packages and jump-start your coding</h5>
            NuGet makes it easy to install and update free libraries and tools.
            <a href="http://go.microsoft.com/fwlink/?LinkId=245147">Learn more…</a>
        </li>
        <li class="three">
            <h5>Find Web Hosting</h5>
            You can easily find a web hosting company that offers the right mix of features and price for your applications.
            <a href="http://go.microsoft.com/fwlink/?LinkId=245143">Learn more…</a>
        </li>
    </ol>
    <asp:TreeView ID="TreeView2" runat="server" ImageSet="BulletedList3" ShowExpandCollapse="False" ShowLines="True">
                <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                <NodeStyle Font-Names="Verdana" Font-Size="8pt" ForeColor="Black" HorizontalPadding="0px" NodeSpacing="0px" VerticalPadding="0px" />
                <ParentNodeStyle Font-Bold="False" />
                <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px" VerticalPadding="0px" />
            </asp:TreeView>
</asp:Content>
