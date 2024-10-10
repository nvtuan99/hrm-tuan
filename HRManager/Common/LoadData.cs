using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRManager.Common
{
    public class LoadData
    {
        HRManagerEntities db = new HRManagerEntities();
        public int loadNationality(string Nationality)
        {
            var nationalityID = db.tblNationality.Where(a => a.NameNationalityVN.Equals(Nationality)).FirstOrDefault();
            if (nationalityID != null) return nationalityID.ID; else return 0;
        }

        public int loadNation(string Nation)
        {
            var nationID = db.tblNation.Where(a => a.NameNationVN.Equals(Nation)).FirstOrDefault();
            if (nationID != null) return nationID.ID; else return 0;

        }
        public int loadReligion(string Religion)
        {
            var ReligionID = db.tblReligion.Where(a => a.NameReligionVN.Equals(Religion)).FirstOrDefault();
            if (ReligionID != null) return ReligionID.ID; else return 0;

        }
        public int loadDegree(string degree)
        {
            var degreeID = db.tblDegree.Where(a => a.DegreeVN.Equals(degree)).FirstOrDefault();
            if (degreeID != null) return degreeID.ID; else return 0;
        }

        public int loadMarital(string marital)
        {
            var maritalID = db.tblMaritalStatus.Where(a => a.MaritalStatusVN.Equals(marital)).FirstOrDefault();
            if (maritalID != null) return maritalID.ID; else return 0;
        }

        public int loadDepartment(string department)
        {
            var departmentID = db.tblDepartment.Where(a => a.NameDepartmentVN.Equals(department)).FirstOrDefault();
            return departmentID.ID;
        }

        public int loadPosition(string position)
        {
            var positionID = db.tblPosition.Where(a => a.PositionNameVN.Equals(position)).FirstOrDefault();
            if (positionID != null) return positionID.ID; else return 0;
        }

        public int loadCompany(string company)
        {
            var companyID = db.tblCompany.Where(a => a.NameCompanyVN.Equals(company)).FirstOrDefault();
            if (companyID != null) return companyID.ID; else return 0;
        }
    }
}