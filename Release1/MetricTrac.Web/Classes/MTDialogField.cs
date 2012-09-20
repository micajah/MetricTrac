using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Micajah.Common.WebControls;

namespace MetricTrac.MTControls
{
    public class MTDialogField : DataControlField, INamingContainer
    {
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Browsable(false)]
        [TemplateContainer(typeof(IDataItemContainer), BindingDirection.TwoWay)]
        [DefaultValue("")]
        public virtual ITemplate ItemTemplate { get; set; }


        [UrlProperty]
        public string EditUrl { get; set; }

        protected override void CopyProperties(DataControlField newField)
        {
            base.CopyProperties(newField);
            MTDialogField f = newField as MTDialogField;
            f.ItemTemplate = this.ItemTemplate;
            f.HeaderText = this.HeaderText;
        }

        protected override DataControlField CreateField()
        {
            return new MTDialogField();
        }

        static Telerik.Web.UI.RadWindow EditWindow;
        HyperLink EditLink;
        public override void InitializeCell(DataControlFieldCell cell, DataControlCellType cellType, DataControlRowState rowState, int rowIndex)
        {
            string s = ItemTemplate.ToString();
            switch (cellType)
            {
                case DataControlCellType.DataCell:
                    System.Web.UI.Control c = new System.Web.UI.Control();
                    ItemTemplate.InstantiateIn(c);
                    cell.Controls.Add(c);

                    EditWindow = new RadWindow();
                    EditWindow.Modal = true;
                    EditWindow.VisibleStatusbar = false;
                    EditWindow.Height = 350;
                    EditWindow.Width = 490;
                    EditWindow.DestroyOnClose = false;
                    EditWindow.Title = "Edit Performance Indicator Metrics";
                    cell.Controls.Add(EditWindow);
                    Control.PreRender += new EventHandler(Control_PreRender);



                    break;
                case DataControlCellType.Header:
                    if(!string.IsNullOrEmpty(HeaderText))
                        cell.Controls.Add(new LiteralControl(HeaderText + "<br>"));

                    EditLink = new HyperLink();
                    EditLink.Text = "Edit";                    
                    cell.Controls.Add(EditLink);
                    break;
            }
        }

        void Control_PreRender(object sender, EventArgs e)
        {
            if (EditWindow == null || EditLink==null) return;
            System.Web.UI.Control c = sender as System.Web.UI.Control;
            string FunctionName = "OpenDialogWindow_"+c.ClientID;
            string FBody = "<script type='text/javascript'> function " + FunctionName + "(url){var oWnd = $find('" + EditWindow.ClientID + "');oWnd.setUrl(url); oWnd.Center();oWnd.show();}</script>";
            c.Page.ClientScript.RegisterClientScriptBlock(typeof(MTDialogField), FunctionName,FBody,false);
            EditLink.NavigateUrl = "javascript:" + FunctionName + "('" + EditUrl + "');";
        }
    }
}
