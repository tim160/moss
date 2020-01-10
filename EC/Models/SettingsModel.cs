using EC.Controllers.ViewModel;
using EC.Localization;
using EC.Models.Database;
using EC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

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
                        case "Outcome":
                            flag = this.addOutcome(newSetting);
                            break;
                        case "deleteOutcome":
                            flag = this.deleteOutcome(newSetting);
                            break;
                        case "RootCauses1":
                            flag = this.addRootCauses1(newSetting);
                            break;
                        case "RootCauses2":
                            flag = this.addRootCauses2(newSetting);
                            break;
                        case "RootCauses3":
                            flag = this.addRootCauses3(newSetting);
                            break;
                        case "deleteRootCauses1":
                            flag = this.deleteRootCauses1(newSetting);
                            break;
                        case "deleteRootCauses2":
                            flag = this.deleteRootCauses2(newSetting);
                            break;
                        case "deleteRootCauses3":
                            flag = this.deleteRootCauses3(newSetting);
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

        public string deleteRootCauses1(SettingsViewModel newSetting)
        {
            if (newSetting.companyId > 0 && newSetting.userId > 0 && newSetting.data != null)
            {
                var item = db.company_root_cases_behavioral.FirstOrDefault(x => x.id.ToString() == newSetting.data.Trim() & x.company_id == newSetting.companyId);
                if (item != null)
                {
                    item.status_id = 1;
                    db.SaveChanges();
                }
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                arr1 = db.company_root_cases_behavioral.Where(x => x.status_id == 2 & x.company_id == newSetting.companyId).OrderBy(x => x.name_en).ToList(),
                arr2 = db.company_root_cases_external.Where(x => x.status_id == 2 & x.company_id == newSetting.companyId).OrderBy(x => x.name_en).ToList(),
                arr3 = db.company_root_cases_organizational.Where(x => x.status_id == 2 & x.company_id == newSetting.companyId).OrderBy(x => x.name_en).ToList(),
            });
        }

        public string deleteRootCauses2(SettingsViewModel newSetting)
        {
            if (newSetting.companyId > 0 && newSetting.userId > 0 && newSetting.data != null)
            {
                var item = db.company_root_cases_external.FirstOrDefault(x => x.id.ToString() == newSetting.data.Trim() & x.company_id == newSetting.companyId);
                if (item != null)
                {
                    item.status_id = 1;
                    db.SaveChanges();
                }
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                arr1 = db.company_root_cases_behavioral.Where(x => x.status_id == 2 & x.company_id == newSetting.companyId).OrderBy(x => x.name_en).ToList(),
                arr2 = db.company_root_cases_external.Where(x => x.status_id == 2 & x.company_id == newSetting.companyId).OrderBy(x => x.name_en).ToList(),
                arr3 = db.company_root_cases_organizational.Where(x => x.status_id == 2 & x.company_id == newSetting.companyId).OrderBy(x => x.name_en).ToList(),
            });
        }

        public string deleteRootCauses3(SettingsViewModel newSetting)
        {
            if (newSetting.companyId > 0 && newSetting.userId > 0 && newSetting.data != null)
            {
                var item = db.company_root_cases_organizational.FirstOrDefault(x => x.id.ToString() == newSetting.data.Trim() & x.company_id == newSetting.companyId);
                if (item != null)
                {
                    item.status_id = 1;
                    db.SaveChanges();
                }
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                arr1 = db.company_root_cases_behavioral.Where(x => x.status_id == 2 & x.company_id == newSetting.companyId).OrderBy(x => x.name_en).ToList(),
                arr2 = db.company_root_cases_external.Where(x => x.status_id == 2 & x.company_id == newSetting.companyId).OrderBy(x => x.name_en).ToList(),
                arr3 = db.company_root_cases_organizational.Where(x => x.status_id == 2 & x.company_id == newSetting.companyId).OrderBy(x => x.name_en).ToList(),
            });
        }

        public string addRootCauses1(SettingsViewModel newSetting)
        {
            try
            {
                if (newSetting.companyId > 0 && newSetting.userId > 0 && newSetting.data != null)
                {
                    var item = db.company_root_cases_behavioral.FirstOrDefault(x => x.name_en.Trim().ToLower() == newSetting.data.Trim().ToLower() & x.company_id == newSetting.companyId);
                    if (item != null)
                    {
                        item.status_id = 2;
                        db.SaveChanges();
                    }
                    else
                    {
                        item = new company_root_cases_behavioral
                        {
                            //name_en = newSetting.data.Trim().ToLower(),
                            name_en = newSetting.data.Trim(),
                            name_es = "",
                            name_fr = "",
                            company_id = newSetting.companyId,
                            status_id = 2
                        };
                        db.company_root_cases_behavioral.Add(item);
                        db.SaveChanges();
                    }
                }
            }
            catch
            {

            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                arr1 = db.company_root_cases_behavioral.Where(x => x.status_id == 2 & x.company_id == newSetting.companyId).OrderBy(x => x.name_en).ToList(),
                arr2 = db.company_root_cases_external.Where(x => x.status_id == 2 & x.company_id == newSetting.companyId).OrderBy(x => x.name_en).ToList(),
                arr3 = db.company_root_cases_organizational.Where(x => x.status_id == 2 & x.company_id == newSetting.companyId).OrderBy(x => x.name_en).ToList(),
            });
        }

        public string addRootCauses2(SettingsViewModel newSetting)
        {
            try
            {
                if (newSetting.companyId > 0 && newSetting.userId > 0 && newSetting.data != null)
                {
                    var item = db.company_root_cases_external.FirstOrDefault(x => x.name_en.Trim().ToLower() == newSetting.data.Trim().ToLower() & x.company_id == newSetting.companyId);
                    if (item != null)
                    {
                        item.status_id = 2;
                        db.SaveChanges();
                    }
                    else
                    {
                        item = new company_root_cases_external
                        {
                            name_en = newSetting.data.Trim(),
                            name_es = "",
                            name_fr = "",
                            company_id = newSetting.companyId,
                            status_id = 2
                        };
                        db.company_root_cases_external.Add(item);
                        db.SaveChanges();
                    }
                }
            }
            catch
            {

            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                arr1 = db.company_root_cases_behavioral.Where(x => x.status_id == 2 & x.company_id == newSetting.companyId).OrderBy(x => x.name_en).ToList(),
                arr2 = db.company_root_cases_external.Where(x => x.status_id == 2 & x.company_id == newSetting.companyId).OrderBy(x => x.name_en).ToList(),
                arr3 = db.company_root_cases_organizational.Where(x => x.status_id == 2 & x.company_id == newSetting.companyId).OrderBy(x => x.name_en).ToList(),
            });
        }

        public string addRootCauses3(SettingsViewModel newSetting)
        {
            try
            {
                if (newSetting.companyId > 0 && newSetting.userId > 0 && newSetting.data != null)
                {
                    var item = db.company_root_cases_organizational.FirstOrDefault(x => x.name_en.Trim().ToLower() == newSetting.data.Trim().ToLower() & x.company_id == newSetting.companyId);
                    if (item != null)
                    {
                        item.status_id = 2;
                        db.SaveChanges();
                    }
                    else
                    {
                        item = new company_root_cases_organizational
                        {
                            name_en = newSetting.data.Trim(),
                            name_es = "",
                            name_fr = "",
                            company_id = newSetting.companyId,
                            status_id = 2
                        };
                        db.company_root_cases_organizational.Add(item);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception exc)
            {

            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                arr1 = db.company_root_cases_behavioral.Where(x => x.status_id == 2 & x.company_id == newSetting.companyId).OrderBy(x => x.name_en).ToList(),
                arr2 = db.company_root_cases_external.Where(x => x.status_id == 2 & x.company_id == newSetting.companyId).OrderBy(x => x.name_en).ToList(),
                arr3 = db.company_root_cases_organizational.Where(x => x.status_id == 2 & x.company_id == newSetting.companyId).OrderBy(x => x.name_en).ToList(),
            });
        }

        public string addOutcome(SettingsViewModel newSetting)
        {
            string id = null;

            try
            {
                if (newSetting.companyId > 0 && newSetting.userId > 0 && newSetting.data != null)
                {
                    company_outcome oldOutcome = db.company_outcome.Where(item => item.outcome_en.Trim().ToLower() == newSetting.data.Trim().ToLower()).FirstOrDefault();
                    if(oldOutcome == null)
                    {
                        oldOutcome = new company_outcome();
                    }
                    oldOutcome.status_id = 2;
                    oldOutcome.company_id = newSetting.companyId;
                    oldOutcome.outcome_en = newSetting.data.Trim();
                    oldOutcome.outcome_fr = newSetting.data.Trim();
                    oldOutcome.outcome_es = newSetting.data.Trim();
                    oldOutcome.outcome_ru = newSetting.data.Trim();
                    oldOutcome.outcome_ar = newSetting.data.Trim();
                    oldOutcome.outcome_jp = newSetting.data.Trim();
                    oldOutcome.last_update_dt = DateTime.Now;
                    db.company_outcome.Add(oldOutcome);
                    db.SaveChanges();
                    id = oldOutcome.id.ToString();
                }
            } catch (Exception e)
            {
                return "false";
            }
            return id;

        }
        public string deleteOutcome(SettingsViewModel newSetting)
        {
            try
            {
                if (newSetting.companyId > 0 && newSetting.userId > 0 && newSetting.data != null)
                {
                    int count = db.company_outcome.Where(item => item.company_id == newSetting.companyId && item.status_id == 2).Count();
                    if (count <= 1)
                    {
                        return "false";
                    }
                    int idSetting = Int32.Parse(newSetting.data);
                    company_outcome deleteOutcome = db.company_outcome.Where(item => item.id == idSetting).FirstOrDefault();
                    if (deleteOutcome != null)
                    {
                        deleteOutcome.status_id = 1;
                        deleteOutcome.last_update_dt = DateTime.Now;
                        db.SaveChanges();
                    }
                }
                return "true";
            }
            catch (Exception e)
            {
                return "false";
            }
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
                logger.Error(ex.ToString());
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
                logger.Error(ex.ToString());
                return "false";
            }
            return id;
        }
        public string addLocationCompany(SettingsViewModel newSetting)
        {
            string id = null;
            try
            {
                var data = newSetting.data == null ? new string[0] : newSetting.data.Split('\n');
                if (newSetting.companyId > 0 && newSetting.userId > 0 && data.Length == 2)
                {
                    var name = data[0].ToLower();
                    int extendId;
                    if (!int.TryParse(data[1], out extendId))
                    {
                        extendId = 0;
                    }
                    company_location oldLocation = db.company_location.Where(item => item.location_en.Trim().ToLower() == name && item.company_id == newSetting.companyId).FirstOrDefault();
                    if (oldLocation != null)
                    {
                        oldLocation.status_id = 2;
                        oldLocation.location_cc_extended_id = extendId == 0 ? null : (int?)extendId;
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
                            location_en = data[0],
                            last_update_dt = DateTime.Now,
                            user_id = 1,
                            location_cc_extended_id = extendId == 0 ? null : (int?)extendId,
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
                logger.Error(ex.ToString());
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
                    logger.Error(ex.ToString());
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
                logger.Error(ex.ToString());
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
            result = PasswordUtils.IsValidPass(newPass);
            if (result == "Success")
            {
                if (newPass != confPass)
                {
                    return "Password and Confirm Password do not match";
                }
                user user = db.user.Where(item => item.id == user_id).FirstOrDefault();
                if (user.password == PasswordUtils.GetHash(oldPass))
                {
                    //try save
                    user.password = PasswordUtils.GetHash(newPass);
                    db.SaveChanges();
                }
                else
                {
                    return "Wrong old password";
                }
            }
            return result;
        }

        public bool checkIsExistGlobalSettings(int userId)
        {
            var globalSetting = db.global_settings.Where(gl_settings => gl_settings.client_id == userId).FirstOrDefault();
            if (globalSetting != null)
            {
                return true;
            } else
            {
                return createNewGlobalSetting(userId);
            }
        }
        public bool createNewGlobalSetting(int userId)
        {
            var globalSetting = new global_settings();
            globalSetting.client_id = userId;
            globalSetting.application_name = LocalizationGetter.GetString("EmployeeConfidential");
            globalSetting.header_color_code = WebConfigurationManager.AppSettings["HeaderColor"];
            globalSetting.header_links_color_code = WebConfigurationManager.AppSettings["HeaderLinksColor"];
            db.global_settings.Add(globalSetting);
            db.SaveChanges();
            return true;
        }
        public bool updateIconPath(int userId, string iconPath)
        {
            var globalSetting = db.global_settings.Where(gl_settings => gl_settings.client_id == userId).FirstOrDefault();
            if (globalSetting != null)
            {
                globalSetting.custom_logo_path = iconPath;
                db.SaveChanges();
                return true;
            } else
            {
                return false;
            }
        }
        public bool updateColorGlobalSettings(int userId, string header_color_code, string header_links_color_code)
        {
            var globalSetting = db.global_settings.Where(gl_settings => gl_settings.client_id == userId).FirstOrDefault();
            if (globalSetting != null)
            {
                globalSetting.header_color_code = header_color_code;
                globalSetting.header_links_color_code = header_links_color_code;
                db.SaveChanges();
                return true;
            } else
            {
                return false;
            }
        }
    }
}