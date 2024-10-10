using HRManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRManager.Common
{
    public class setRole
    {
        HRManagerEntities db = new HRManagerEntities();

        //Home page
        public static bool Role_homePage_view;
        public static bool Role_homePage_delete;
        public static bool Role_homePage_update;


        //Human resource
        public static bool Role_HR_view;
        public static bool Role_HR_delete;
        public static bool Role_HR_update;


        //Human resource category
        public static bool Role_HRCategory_view;
        public static bool Role_HRCategory_delete;
        public static bool Role_HRCategory_update;


        //Salary
        public static bool Role_Salary_view;
        public static bool Role_Salary_delete;
        public static bool Role_Salary_update;


        //Salary category
        public static bool Role_SalaryCategory_view;
        public static bool Role_SalaryCategory_delete;
        public static bool Role_SalaryCategory_update;


        //Contract
        public static bool Role_Contract_view;
        public static bool Role_Contract_delete;
        public static bool Role_Contract_update;


        //ContractCategory
        public static bool Role_ContractCategory_view;
        public static bool Role_ContractCategory_delete;
        public static bool Role_ContractCategory_update;


        //System
        public static bool Role_System_view;
        public static bool Role_System_delete;
        public static bool Role_System_update;

        public static void setRoleMenu(List<ViewRoleDetails> roleList)
        {
            foreach (ViewRoleDetails item in roleList)
            {

                //Homepage
                if (item.idCategory == 1)
                {
                    Role_homePage_view = (item.roleView == 1) ? true : false;
                    Role_homePage_update = (item.roleUpdate == 1) ? true : false;
                    Role_homePage_delete = (item.roleDelete == 1) ? true : false;
                }


                //Human resource
                if (item.idCategory == 2)
                {
                    Role_HR_view = (item.roleView == 1) ? true : false;
                    Role_HR_update = (item.roleUpdate == 1) ? true : false;
                    Role_HR_delete = (item.roleDelete == 1) ? true : false;
                }



                //Human resource Category
                if (item.idCategory == 3)
                {
                    Role_HRCategory_view = (item.roleView == 1) ? true : false;
                    Role_HRCategory_update = (item.roleUpdate == 1) ? true : false;
                    Role_HRCategory_delete = (item.roleDelete == 1) ? true : false;
                }


                //Salary
                if (item.idCategory == 4)
                {
                    Role_Salary_view = (item.roleView == 1) ? true : false;
                    Role_Salary_update = (item.roleUpdate == 1) ? true : false;
                    Role_Salary_delete = (item.roleDelete == 1) ? true : false;
                }


                //Salary Category
                if (item.idCategory == 5)
                {
                    Role_SalaryCategory_view = (item.roleView == 1) ? true : false;
                    Role_SalaryCategory_update = (item.roleUpdate == 1) ? true : false;
                    Role_SalaryCategory_delete = (item.roleDelete == 1) ? true : false;
                }


                //Contract
                if (item.idCategory == 6)
                {
                    Role_Contract_view = (item.roleView == 1) ? true : false;
                    Role_Contract_update = (item.roleUpdate == 1) ? true : false;
                    Role_Contract_delete = (item.roleDelete == 1) ? true : false;
                }

                //ContractCategory
                if (item.idCategory == 7)
                {
                    Role_ContractCategory_view = (item.roleView == 1) ? true : false;
                    Role_ContractCategory_update = (item.roleUpdate == 1) ? true : false;
                    Role_ContractCategory_delete = (item.roleDelete == 1) ? true : false;
                }

                //System
                if (item.idCategory == 8)
                {
                    Role_System_view = (item.roleView == 1) ? true : false;
                    Role_System_update = (item.roleUpdate == 1) ? true : false;
                    Role_System_delete = (item.roleDelete == 1) ? true : false;
                }
            }
        }

        public List<ViewRoleDetails> loadRoleDetail(int? EmployeeID)
        {
            string sql = "SELECT CAST(rc.id as INT) as id , " +
                        "CAST(rc.ID as INT) as idCategory , " +
                      "rc.categorynamevn as nameCategoryNameVN , " +
                      EmployeeID + "  as employeeID , " +
                      "COALESCE(CAST(r.RoleView AS INT), 0) AS roleView, " +
                      "COALESCE(CAST(r.RoleUpdate AS INT), 0) AS roleUpdate, " +
                      "COALESCE(CAST(r.RoleDelete AS INT), 0) AS roleDelete " +
                      "FROM tblRoleCategory rc " +
                      "LEFT JOIN tblrole r ON r.CategoryRole = rc.id AND r.EmployeeID = " + EmployeeID +
                      "LEFT JOIN tblEmployee e ON e.id = r.EmployeeID; ";
            var roleDetails = db.Database.SqlQuery<ViewRoleDetails>(sql).ToList();
            return roleDetails;
        }
    }
}