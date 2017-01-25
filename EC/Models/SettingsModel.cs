using EC.Controllers.ViewModel;
using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models
{
    public class SettingsModel : BaseModel
    {
        public string setNewItem(SettingsViewModel newSetting)
        {
            string flag = "false";

            try
            {
                if (newSetting.userId > 0 && newSetting.newSetting != "" && newSetting.data != "" && newSetting.data.ToLower() != "other")
                {
                    newSetting.data = newSetting.data.Trim();
                    // newSetting.data = newSetting.data.ToLower();
                    switch (newSetting.newSetting)
                    {
                        case "Location":
                            flag = this.addLocationCompany(newSetting);
                            break;
                        case "deleteLocation":
                            flag = this.deleteLocation(newSetting);
                            break;
                        case "Department":
                            flag = this.addDepartment(newSetting);
                            break;
                        case "deleteDepartment":
                            flag = this.delDepartment(newSetting);
                            break;
                        case "Language":

                            break;
                        case "IncidentType":
                            flag = this.addIncidentType(newSetting);
                            break;
                        case "deleteIncidentType":
                            flag = this.delIncidentType(newSetting);
                            break;
                        case "addReporterType":
                            flag = this.addReporterType(newSetting);
                            break;
                        case "deleteReporterType":
                            flag = this.deleteReporterType(newSetting);
                            break;
                    }
                }
            }
            catch (FormatException e)
            {
                Console.WriteLine("Input string is not a sequence of digits.");
            }

            return flag;
        }
        public string deleteLocation(SettingsViewModel newSetting)
        {
            try
            {
                if (newSetting.companyId > 0 && newSetting.userId > 0 && newSetting.data != null)
                {
                    int count = db.company_location.Where(item => item.company_id == newSetting.companyId && item.status_id == 2).Count();
                    if (count <= 1)
                    {
                        return "false";
                    }
                    int idSetting = Int32.Parse(newSetting.data);
                    company_location oldLocation = db.company_location.Where(item => item.id == idSetting).FirstOrDefault();
                    if (oldLocation != null)
                    {
                        oldLocation.status_id = 1;
                        oldLocation.last_update_dt = DateTime.Now;
                        db.SaveChanges();
                    }
                }
                else
                {
                    return "false";
                }
            }
            catch (Exception e)
            {
                return "false";
            }
            return "true";
        }
        public string delDepartment(SettingsViewModel newSetting)
        {
            try
            {
                if (newSetting.companyId > 0 && newSetting.userId > 0 && newSetting.data != null)
                {
                    int count = db.company_department.Where(item => item.company_id == newSetting.companyId && item.status_id == 2).Count();
                    if (count <= 1)
                    {
                        return "false";
                    }
                    int idDepartment = Int32.Parse(newSetting.data);
                    company_department oldDepartmnet = db.company_department.Where(item => item.id == idDepartment).FirstOrDefault();
                    if (oldDepartmnet != null)
                    {
                        oldDepartmnet.status_id = 1;
                        oldDepartmnet.user_id = newSetting.userId;
                        oldDepartmnet.last_update_dt = DateTime.Now;
                        db.SaveChanges();
                    }
                }
                else
                {
                    return "false";
                }
            }
            catch (Exception e)
            {
                return "false";
            }
            return "true";
        }
        public string addDepartment(SettingsViewModel newSetting)
        {
            string id = null;
            try
            {
                if (newSetting.companyId > 0 && newSetting.userId > 0 && newSetting.data != null)
                {

                    company_department oldDepartment = db.company_department.Where(item => item.department_en.Trim().ToLower() == newSetting.data.Trim().ToLower() && item.company_id == newSetting.companyId).FirstOrDefault();
                    if (oldDepartment != null)
                    {
                        oldDepartment.status_id = 2;
                        db.SaveChanges();
                        id = oldDepartment.id.ToString();
                    }
                    else
                    {
                        company_department department = new company_department();
                        department.user_id = newSetting.userId;
                        department.client_id = 1;
                        department.company_id = newSetting.companyId;
                        department.department_en = newSetting.data;
                        department.status_id = 2;
                        department.last_update_dt = DateTime.Now;
                        department = db.company_department.Add(department);
                        db.SaveChanges();
                        id = department.id.ToString();
                    }
                }
                else
                {
                    return "false";
                }
            }
            catch (Exception e)
            {
                return "false";
            }
            return id;
        }
        public string deleteReporterType(SettingsViewModel newSetting)
        {
            try
            {
                int count = db.company_relationship.Where(item => item.company_id == newSetting.companyId && item.status_id == 2).Count();
                if (count <= 1)
                {
                    return "false";
                }

                int id = Int32.Parse(newSetting.data);
                var order = (from o in db.company_relationship
                             where o.id == id
                             select o).First();
                if (order != null)
                {
                    order.status_id = 1;
                    db.SaveChanges();
                }
            }
            catch (System.Data.DataException ex)
            {
                return "false";
            }
            return "true";
        }
        public string addReporterType(SettingsViewModel newSetting)
        {
            string id = null;
            try
            {
                company_relationship idContain = db.company_relationship.Where(item => item.relationship_en.Trim().ToLower() == newSetting.data.Trim().ToLower() && item.company_id == newSetting.companyId).FirstOrDefault();
                if (idContain != null)
                {
                    idContain.status_id = 2;
                    db.SaveChanges();
                    return idContain.id.ToString();
                }
                else
                {
                    company_relationship custom = new company_relationship
                    {
                        company_id = newSetting.companyId,
                        client_id = newSetting.userId,
                        status_id = 2,
                        relationship_en = newSetting.data,
                        relationship_es = "",
                        relationship_ar = "",
                        relationship_fr = "",
                        relationship_jp = "",
                        relationship_ru = ""
                    };
                    custom = db.company_relationship.Add(custom);
                    db.SaveChanges();
                    id = custom.id.ToString();
                }
            }
            catch (System.Data.DataException ex)
            {
                return "false";
            }
            return id;
        }
        public string addLocationCompany(SettingsViewModel newSetting)
        {
            string id = null;
            try
            {

                if (newSetting.companyId > 0 && newSetting.userId > 0 && newSetting.data != null)
                {
                    company_location oldLocation = db.company_location.Where(item => item.location_en.Trim().ToLower() == newSetting.data.Trim().ToLower() && item.company_id == newSetting.companyId).FirstOrDefault();
                    if (oldLocation != null)
                    {
                        oldLocation.status_id = 2;
                        db.SaveChanges();
                        id = oldLocation.id.ToString();
                    }
                    else
                    {
                        company_location location = new company_location
                        {
                            client_id = newSetting.userId,
                            company_id = newSetting.companyId,
                            status_id = 2,
                            location_en = newSetting.data,
                            last_update_dt = DateTime.Now,
                            user_id = 1,

                        };
                        location = db.company_location.Add(location);
                        db.SaveChanges();
                        id = location.id.ToString();
                    }
                }
                else
                {
                    return "false";
                }
            }
            catch (System.Data.DataException ex)
            {
                return "false";
            }
            return id;
        }
        public company_secondary_type isIncydentTypeCreated(string secondary_type_en, int company_id)
        {
            return db.company_secondary_type.Where(item => item.secondary_type_en.Trim().ToLower() == secondary_type_en.Trim().ToLower() && item.company_id == company_id).FirstOrDefault();
        }
        public string addIncidentType(SettingsViewModel newSetting)
        {
            string id = null;
            company_secondary_type temp = isIncydentTypeCreated(newSetting.data, newSetting.companyId);
            if (temp != null)
            {
                temp.status_id = 2;
                db.SaveChanges();
                return temp.id.ToString();
            }
            else
            {
                try
                {
                    company_secondary_type type = new company_secondary_type
                    {
                        client_id = newSetting.userId,
                        company_id = newSetting.companyId,
                        parent_secondary_type = 0,
                        status_id = 2,
                        secondary_type_en = newSetting.data,
                        last_update_dt = DateTime.Now,
                        user_id = 1,
                        type_id = 6
                    };
                    type = db.company_secondary_type.Add(type);
                    db.SaveChanges();
                    id = type.id.ToString();
                }
                catch (System.Data.DataException ex)
                {
                    return "false";
                }
                return id;
            }

        }
        public string delIncidentType(SettingsViewModel newSetting)
        {
            try
            {
                int count = db.company_secondary_type.Where(item => item.company_id == newSetting.companyId && item.status_id == 2).Count();
                if (count <= 1)
                {
                    return "false";
                }
                int id = Int32.Parse(newSetting.data);
                var order = (from o in db.company_secondary_type
                             where o.id == id
                             select o).First();
                if (order != null)
                {
                    order.status_id = 1;
                    db.SaveChanges();
                }
            }
            catch (System.Data.DataException ex)
            {
                return "false";
            }
            return "true";
        }
        public string IsValidPass(string oldPass, string newPass, string confPass, int user_id)
        {
            string result = "Saved successfully";
            if (oldPass == "" || newPass == "" || confPass == "")
            {
                return "Some fields are empty!";
            }
            result = GlobalFunctions.IsValidPass(newPass);
            if (result == "Success")
            {
                if (newPass != confPass)
                {
                    return "Password and Confirm Password do not match";
                }
                user user = db.user.Where(item => item.id == user_id).FirstOrDefault();
                if (user.password == oldPass)
                {
                    //try save
                    user.password = newPass;
                    db.SaveChanges();
                }
                else
                {
                    return "Wrong old password";
                }
            }
            return result;
        }
    }
}