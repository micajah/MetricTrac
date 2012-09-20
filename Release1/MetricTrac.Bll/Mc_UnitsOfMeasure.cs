using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Micajah;

namespace MetricTrac.Bll
{
    public partial class Mc_UnitsOfMeasure
    {
        public static List<Micajah.Common.Bll.MeasureUnit> GetOrganizationUoMs()
        {            
            Micajah.Common.Bll.MeasureUnitCollection UoMs = Micajah.Common.Bll.MeasureUnitCollection.GetUnits(LinqMicajahDataContext.OrganizationId);
            return UoMs.ToList();
        }

        public static List<Micajah.Common.Bll.MeasureUnit> GetConvertedUoMs(Guid? ParentUoMID)
        {
            return GetConvertedUoMs(ParentUoMID, LinqMicajahDataContext.OrganizationId);
        }

        public static List<Micajah.Common.Bll.MeasureUnit> GetConvertedUoMs(Guid? ParentUoMID, Guid OrganizationId)
        {            
            Micajah.Common.Bll.MeasureUnitCollection UoMs = null;
            if (ParentUoMID == null)
                UoMs = Micajah.Common.Bll.MeasureUnitCollection.GetUnits(OrganizationId);
            else
            {
                Micajah.Common.Bll.MeasureUnit mu = Micajah.Common.Bll.MeasureUnit.Create((Guid)ParentUoMID, OrganizationId);
                UoMs = mu.GetConvertUnits();
                UoMs.Add(mu);
                UoMs.SortByName();
            }                
            return UoMs.ToList();
        }

        public static string ConvertValue(string value, Guid FromUoMID, Guid ToUoMID)
        {            
            return ConvertValue(value, FromUoMID, ToUoMID, LinqMicajahDataContext.OrganizationId);         
        }

        public static string ConvertValue(string value, Guid FromUoMID, Guid ToUoMID, Guid OrganizationId)
        {
            double v = 0;
            double? nv = null;
            if (double.TryParse(value, out v))
            {
                nv = ConvertValue(v, FromUoMID, ToUoMID, OrganizationId);
            }
            return nv == null ? value : nv.ToString();
        }

        public static double? ConvertValue(double value, Guid FromUoMID, Guid ToUoMID, Guid  OrganizationId)
        {
            Micajah.Common.Bll.MeasureUnit mu = Micajah.Common.Bll.MeasureUnit.Create(FromUoMID, OrganizationId);
            Micajah.Common.Bll.MeasureUnitCollection UoMs = mu.GetConvertUnits();
            var cmu = UoMs.Where(u => u.MeasureUnitId == ToUoMID).ToList();
            if (cmu.Count <= 0) return null;
            return cmu[0].ConversionFactor * value;
        }

        public static string GetSingularAbbreviation(Guid UomID)
        {
            Micajah.Common.Bll.MeasureUnit mu = Micajah.Common.Bll.MeasureUnit.Create(UomID, LinqMicajahDataContext.OrganizationId);
            return mu.SingularAbbreviation;
        }
    }
}