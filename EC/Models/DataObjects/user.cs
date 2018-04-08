using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using EC.Constants;


namespace EC.Models.Database
{
  public partial class user : BaseDB
  {
    public string _location_string
    {
      get
      {
        string location_nm = "";

        if (company_location_id.HasValue)
        {
          company_location _loc = db.company_location.FirstOrDefault(item => item.id == company_location_id.Value);
          if (_loc != null)
            location_nm = _loc.location_en;
        }

        return location_nm.Trim();
      }
    }

    public string _department_string
    {
      get
      {
        string department_nm = "";

        if (company_department_id.HasValue)
        {
          company_department _dep = db.company_department.FirstOrDefault(item => item.id == company_department_id.Value);
          if (_dep != null)
            department_nm = _dep.department_en;
        }

        return department_nm.Trim();
      }
    }
        
 /*   public int _reporter_report_id
    {
      get
      {
        int report_id = 0;

          if (role_id == ECLevelConstants.level_informant)
          {
            report _report = db.report.FirstOrDefault(item => item.reporter_user_id == id);
            if(_report != null)
              report_id = _report.id;
            else
              logger.Error("No report found");
          }

        return report_id;
      }
    }*/

    public string _photo_path_string(int param)
    {
      string _photo_path = "";
      if (photo_path != "" && System.IO.File.Exists(photo_path))
      {
        _photo_path = photo_path;
      }
      else
      {
        if (param == 1)
        {
          _photo_path = "~/Content/Icons/noPhoto.png";
        }
        else if (param == 2)
        {
          _photo_path = "~/Content/Icons/settingsPersonalNOPhoto.png";
        }
        else if (param == 3)
        {
          _photo_path = "~/Content/Icons/settingsPersonalNOPhoto.png";
        }
        if (role_id == ECLevelConstants.level_informant)
        {
          _photo_path = "~/Content/Icons/anonimousReporterIcon.png";
        }
      }
      return _photo_path;
    }

    public string _detail { get; set; }
    public string _full_name_with_detail
    {
      get
      {
        if (String.IsNullOrEmpty(_detail))
        {
          return $"{first_nm} {last_nm}";
        }
        return $"{first_nm} {last_nm} [{_detail}]";
      }
    }

    public string _full_name
    {
      get
      {
        return String.Format("{0} {1}", first_nm, last_nm);
      }
    }



    public user GetById(int ID)
    {
      if (ID != 0)
      {
        return db.user.FirstOrDefault(item => item.id == id);
      }
      else
      {
        return null;
      }

    }

    public user GetByLogin(string login)
    {
      user _user = db.user.FirstOrDefault(item => item.login_nm == login);
      return _user;
    }
  }
}


