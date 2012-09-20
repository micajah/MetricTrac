using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.WebControls;

namespace MetricTrac.MTControls
{
    public class MTDropDownListField : BaseValidatedField
    {
        #region Members

        private string m_SelectedValue = string.Empty;

        #endregion

        #region Public Properties


        [DefaultValue(false)]
        public bool HtmlDecode { get; set; }
        public bool AddEmptyItem { get; set; }
        public string OnClientSelectedIndexChanged { get; set; }
        public Unit Width { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether list items are cleared before data binding.
        /// </summary>
        [DefaultValue(false)]
        public bool AppendDataBoundItems
        {
            get
            {
                object obj = ViewState["AppendDataBoundItems"];
                return ((obj == null) ? false : (bool)obj);
            }
            set { ViewState["AppendDataBoundItems"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a postback to the server automatically occurs when the user changes the control selection.
        /// </summary>
        [DefaultValue(false)]
        public bool AutoPostBack
        {
            get
            {
                object obj = ViewState["AutoPostBack"];
                return ((obj == null) ? false : (bool)obj);
            }
            set { ViewState["AutoPostBack"] = value; }
        }

        /// <summary>
        /// Gets or sets the data source of the list that is being bound. The default value is a null reference.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object DataSource
        {
            get { return (object)ViewState["DataSource"]; }
            set { ViewState["DataSource"] = value; }
        }

        /// <summary>
        /// Gets or sets the ID of the control from which the data-bound control retrieves its list of data items.
        /// The default is System.String.Empty.
        [Category("Data")]
        [DefaultValue("")]
        [IDReferenceProperty(typeof(DataSourceControl))]
        public string DataSourceId
        {
            get
            {
                object obj = ViewState["DataSourceId"];
                return ((obj == null) ? string.Empty : (string)obj);
            }
            set { ViewState["DataSourceId"] = value; }
        }

        /// <summary>
        /// Specifies which property of a data-bound item to use when binding an item's Text property.
        /// The default is a System.String.Empty.
        /// </summary>
        [Category("Data")]
        [DefaultValue("")]
        public virtual string DataTextField
        {
            get
            {
                object obj = ViewState["DataTextField"];
                return ((obj == null) ? string.Empty : (string)obj);
            }
            set { ViewState["DataTextField"] = value; }
        }

        /// <summary>
        /// Specifies which property of a data-bound item to use when binding an item's Value property.
        /// The default is a System.String.Empty.
        /// </summary>
        [Category("Data")]
        [DefaultValue("")]
        public virtual string DataValueField
        {
            get
            {
                object obj = ViewState["DataValueField"];
                return ((obj == null) ? string.Empty : (string)obj);
            }
            set { ViewState["DataValueField"] = value; }
        }

        /// <summary>
        /// Gets or sets a brief phrase that summarizes what a control does, for use in ToolTips and catalogs of WebPart controls.
        /// </summary>
        [DefaultValue("")]
        public string Description
        {
            get
            {
                object obj = ViewState["Description"];
                return ((obj == null) ? string.Empty : (string)obj);
            }
            set { ViewState["Description"] = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the current instance of the DropDownList has child items.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsEmpty
        {
            get
            {
                object obj = ViewState["IsEmpty"];
                return ((obj == null) ? false : (bool)obj);
            }
        }


        [DefaultValue("")]
        public string SelectedValue
        {
            get
            {
                object obj = ViewState["SelectedValue"];
                return ((obj == null) ? string.Empty : (string)obj);
            }
            set { ViewState["SelectedValue"] = value; }
        }

        #endregion

        #region Events

        public event EventHandler SelectedIndexChanged;
        public event EventHandler DataBound;

        #endregion

        #region Private Methods

        private void CopyProperties(DropDownList control)
        {
            control.Width = base.ControlStyle.Width;
            control.AppendDataBoundItems = this.AppendDataBoundItems;
            control.AutoPostBack = this.AutoPostBack;
            control.Enabled = (!this.ReadOnly);
            control.DataSource = this.DataSource;
            control.DataSourceID = this.DataSourceId;
            control.DataTextField = this.DataTextField;
            control.DataValueField = this.DataValueField;
            control.Width = this.Width;

            // It's important to call the method next the code above.
            //CopyProperties(this, control);
            control.Enabled = this.Enabled;
            control.TabIndex = this.TabIndex;
            control.ValidationGroup = this.ValidationGroup;

            if (!string.IsNullOrEmpty(OnClientSelectedIndexChanged)) control.Attributes.Add("onchange", OnClientSelectedIndexChanged + "(this);");
        }

        private void OnBindingField(object sender, EventArgs e)
        {
            DropDownList dropDownList = sender as DropDownList;
            if (dropDownList != null)
            {
                m_SelectedValue = this.LookupStringValue(dropDownList);
                dropDownList.DataBound += new EventHandler(OnDataBound);
            }
        }

        private void OnDataBound(object sender, EventArgs e)
        {
            DropDownList dropDownList = sender as DropDownList;
            if (dropDownList != null)
            {
                if(AddEmptyItem) dropDownList.Items.Insert(0,new ListItem(string.Empty,null));
                if (!string.IsNullOrEmpty(m_SelectedValue))
                    dropDownList.SelectedValue = m_SelectedValue;
                else if (dropDownList.Items.Count > 0)
                    dropDownList.Items[0].Selected = true;
                if (HtmlDecode)
                    foreach (ListItem li in dropDownList.Items)
                        li.Text = System.Web.HttpUtility.HtmlDecode(li.Text);
            }

            if (this.DataBound != null) this.DataBound(sender, e);
        }

        #endregion

        #region Overriden Methods

        protected override DataControlField CreateField()
        {
            return new ComboBoxField();
        }

        protected override void CopyProperties(DataControlField newField)
        {
            base.CopyProperties(newField);

            ComboBoxField field = newField as ComboBoxField;
            if (field != null)
            {
                field.AppendDataBoundItems = this.AppendDataBoundItems;
                field.AutoPostBack = this.AutoPostBack;
                field.DataSource = this.DataSource;
                field.DataSourceId = this.DataSourceId;
                field.DataTextField = this.DataTextField;
                field.DataValueField = this.DataValueField;
                field.Description = this.Description;
                field.SelectedValue = this.SelectedValue;
                field.HtmlEncode = this.HtmlEncode;
            }
        }

        protected override object ExtractControlValue(System.Web.UI.Control control)
        {
            if (control is LiteralControl)
            {
                control = control.Parent.Controls[1];
            }
            DropDownList dropDownList = control as DropDownList;
            if(dropDownList == null) return null;
            if(string.IsNullOrEmpty(dropDownList.SelectedValue)) return null;
            return dropDownList.SelectedValue;
        }

        protected override void InitializeDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            DropDownList control = new DropDownList();
            this.CopyProperties(control);
            control.Init += OnControlInit;

            if (!(this.EditMode || this.InsertMode))
                control.Enabled = false;

            if (base.Visible)
            {
                control.DataBinding += new EventHandler(this.OnBindingField);
                if (this.SelectedIndexChanged != null) control.SelectedIndexChanged += this.SelectedIndexChanged;
            }

            cell.Controls.Add(new LiteralControl(" &nbsp; "));
            cell.Controls.Add(control);
            cell.Style.Add(HtmlTextWriterStyle.PaddingLeft, "11px");
        }

        #endregion
       
    }
}
