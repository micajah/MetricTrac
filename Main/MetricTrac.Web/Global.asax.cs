using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Micajah.FileService.WebControls;
using System.Threading;


namespace MetricTrac
{
    public class Global : Micajah.Common.Application.WebApplication
    {        
        protected override void Application_Start(object sender, EventArgs e)
        {
            base.Application_Start(sender, e);
            Micajah.FileService.Client.ResourceVirtualPathProvider.Register();
            Micajah.Common.Security.UserContext.SelectedInstanceChanged += new EventHandler(UserContext_SelectedInstanceChanged);
            Micajah.Common.Security.UserContext.SelectedOrganizationChanged += new EventHandler(UserContext_SelectedOrganizationChanged);
            MetricTrac.MetricValuesCalc.Start(Thread.CurrentThread.CurrentCulture, Thread.CurrentThread.CurrentUICulture);
        }

        protected override void Application_End(object sender, EventArgs e)
        {
            base.Application_End(sender, e);
            MetricTrac.MetricValuesCalc.Stop();
        }

        protected void UserContext_SelectedInstanceChanged(object sender, EventArgs e)
        {
            Micajah.Common.Security.UserContext user = sender as Micajah.Common.Security.UserContext;
            UploadControlSettings.DepartmentName = user.SelectedInstance.Name;
            UploadControlSettings.DepartmentId = user.SelectedInstance.InstanceId;
        }

        protected void UserContext_SelectedOrganizationChanged(object sender, EventArgs e)
        {
            Micajah.Common.Security.UserContext user = sender as Micajah.Common.Security.UserContext;
            UploadControlSettings.OrganizationName = user.SelectedOrganization.Name;
            UploadControlSettings.OrganizationId = user.SelectedOrganization.OrganizationId;
            // def instance
            string DefInstanceName = user.SelectedOrganization.Name;
            Guid DefInstanceGuid = user.SelectedOrganization.OrganizationId;
            if (user.SelectedOrganization.Instances.Count > 0)
            {
                DefInstanceName = user.SelectedOrganization.Instances[0].Name;
                DefInstanceGuid = user.SelectedOrganization.Instances[0].InstanceId;
            }
            UploadControlSettings.DepartmentName = DefInstanceName;
            UploadControlSettings.DepartmentId = DefInstanceGuid;
        }
    }
}