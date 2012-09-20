using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Micajah.Common.WebControls;

namespace MetricTrac.MTControls
{
    public class MTGridField : BaseValidatedField, IValidated
    {
        public MTGridField()
        {
        }

        #region Public Properties
        ///
        // Summary:
        ///     Gets or sets an array that contains the names of the primary key fields for
        ///     the items displayed in a System.Web.UI.WebControls.GridView control.
        ///
        /// Returns:
        ///     An array that contains the names of the primary key fields for the items
        ///     displayed in a System.Web.UI.WebControls.GridView control.
        [TypeConverter(typeof(StringArrayConverter))]
        [DefaultValue("")]
        public string[] DataKeyNames { get; set; }

        ///
        /// Summary:
        ///     Gets or sets a value indicating whether bound fields are automatically created
        ///     for each field in the data source.
        ///
        ///Returns:
        ///     true to automatically create bound fields for each field in the data source;
        ///     otherwise, false. The default is true.
        [DefaultValue(false)]
        public virtual bool AutoGenerateColumns { get; set; }
        #endregion



        // Summary:
        //     Gets or sets the object from which the data-bound control retrieves its list
        //     of data items.
        //
        // Returns:
        //     An object that represents the data source from which the data-bound control
        //     retrieves its data. The default is null.
        [Bindable(true)]
        [Themeable(false)]
        [DefaultValue("")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual object DataSource { get; set; }
        //
        // Summary:
        //     Gets or sets the ID of the control from which the data-bound control retrieves
        //     its list of data items.
        //
        // Returns:
        //     The ID of a control that represents the data source from which the data-bound
        //     control retrieves its data. The default is System.String.Empty.
        [Themeable(false)]
        [DefaultValue("")]
        public virtual string DataSourceID { get; set; }


        //
        // Summary:
        //     Gets a collection of System.Web.UI.WebControls.DataControlField objects that
        //     represent the column fields in a System.Web.UI.WebControls.GridView control.
        //
        // Returns:
        //     A System.Web.UI.WebControls.DataControlFieldCollection that contains all
        //     the column fields in the System.Web.UI.WebControls.GridView control.
        [MergableProperty(false)]
        [DefaultValue("")]
        [Editor("System.Web.UI.Design.WebControls.DataControlFieldTypeEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual DataControlFieldCollection Columns
        {
            get
            {
                if (mColumns == null) mColumns = new DataControlFieldCollection();
                return mColumns;
            }
        }
        private DataControlFieldCollection mColumns;

        public event EventHandler DataBinding;

        protected override DataControlField CreateField()
        {
            return new MTGridField();
        }

        protected override void CopyProperties(DataControlField newField)
        {
            base.CopyProperties(newField);

            MTGridField field = newField as MTGridField;
            if (field != null)
            {
                field.DataSource = this.DataSource;
                field.DataSourceID = this.DataSourceID;
                field.AutoGenerateColumns = this.AutoGenerateColumns;
                foreach (DataControlField f in this.Columns)
                {
                    field.Columns.Add(f);
                }
            }
        }

        private void CopyProperties(CommonGridView control)
        {
            control.DataSource = this.DataSource;
            control.DataSourceID = this.DataSourceID;
            control.AutoGenerateColumns = this.AutoGenerateColumns;
            if (this.DataBinding != null)
            {
                foreach(Delegate d in this.DataBinding.GetInvocationList())
                    control.DataBinding += (EventHandler)d;
            }
            
            foreach (DataControlField f in this.Columns)
            {
                control.Columns.Add(f);
            }
        }

        protected override object ExtractControlValue(System.Web.UI.Control control)
        {
            return null;
        }

        public override void InitializeCell(DataControlFieldCell cell, DataControlCellType cellType, DataControlRowState rowState, int rowIndex)
        {
            base.InitializeCell(cell, cellType, rowState, rowIndex);
            if (cellType == DataControlCellType.Header)
            {
                LinkButton lbEdit = new LinkButton();
                lbEdit.Text = "Edit";
                lbEdit.ID = "lbEdit";
                lbEdit.Click += new EventHandler(lbEdit_Click);
                cell.Controls.Add(lbEdit);
            }
        }

        public void lbEdit_Click(object sender, EventArgs e)
        {
        }

        protected override void InitializeDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            CommonGridView control = new CommonGridView();
            control.ColorScheme = ColorScheme.Gray;
            this.CopyProperties(control);
            control.Init += OnControlInit;

            if (!(this.EditMode || this.InsertMode))
                control.Enabled = false;

            if (base.Visible)
            {
                //control.DataBinding += new EventHandler(this.OnBindingField);
            }

            cell.Controls.Add(control);
        }

        public override bool Initialize(bool enableSorting, System.Web.UI.Control control)
        {
            return base.Initialize(enableSorting, control);
        }


        protected override void TrackViewState()
        {
            base.TrackViewState();
        }

        string IValidated.ValidatorInitialValue
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
