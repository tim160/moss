using System;
using System.Collections.Generic;
using System.Linq;
using EC.Models.Database;
using EC.Constants;
using EC.Localization;

namespace EC.Models
{
  public class ReportStringModel : BaseModel
  {


    #region Base constructors
    public ReportStringModel(report _report_from_parent, user _report_user)
    {
      ID = _report_from_parent.id;
      _report = _report_from_parent;
      _reporter_user = _report_user;
    }

    public ReportStringModel(int report_id)
    {
      ID = report_id;
      _report = ReportById(ID);
      _reporter_user = GetReporterUser(ID);
    }
    private report ReportById(int id)
    {
      ID = id;

      if (id != 0)
      {
        return db.report.Where(item => item.id == id).FirstOrDefault();
      }
      else
        return null;

    }


    private user GetReporterUser(int id)
    {
      if ((_report != null) && (ID != 0))
      {
        user _user = db.user.Where(item => item.id == _report.reporter_user_id).FirstOrDefault();
        return _user;
      }
      return null;
    }
    #endregion

    #region Properties
    public int ID
    { get; set; }

    public report _report;
    public user _reporter_user;
    #endregion



    /// <summary>
    /// Returns the location of report in string
    /// </summary>
    /// <param name="report_id"></param>
    /// <returns></returns>
    public string LocationString()
    {
      string location = LocalizationGetter.GetString("unknown_location");
      if ((_report != null) && (ID != 0))
      {
        if (_report.location_id.HasValue)
        {
          company_location _c_location = db.company_location.Where(item => item.id == _report.location_id.Value).FirstOrDefault();
          if (_c_location != null)
            location = _c_location.location_en;
        }
        else if ((_report != null) && (_report.other_location_name != null) && (_report.other_location_name.Trim().Length > 0))
          location = _report.other_location_name.Trim();
      }

      return location;
    }


    /// <summary>
    /// Returns string country of report
    /// </summary>
    /// <param name="report_id"></param>
    /// <returns></returns>
    public string CountryString()
    {
      string country_nm = LocalizationGetter.GetString("unknown");

      if (_report != null)
      {
        if (_report.reporter_country_id.HasValue)
        {
          country _country = db.country.Where(item => item.id == _report.reporter_country_id.Value).FirstOrDefault();
          if (_country != null)
            country_nm = _country.country_nm;
        }
      }
      return country_nm;
    }


    public string IsReportedOutside()
    {

      string report_outside = LocalizationGetter.GetString("unknown");

      if (_report != null)
      {
        if (_report.reported_outside_id.HasValue)
        {
          reported_outside reported_outside =
              db.reported_outside.Where(item => item.id == _report.reported_outside_id.Value).FirstOrDefault();
          if (reported_outside != null)
          {
            report_outside = reported_outside.description_en;
            if (_report.reported_outside_id.Value != 1)
            {
              report_outside = report_outside;// +". Description : " + _report.reported_outside_text;
            }
          }
        }
      }
      return report_outside;
    }
    public string IsReportedUrgent()
    {
      string report_urgent = LocalizationGetter.GetString("unknown");

      if (_report.priority_id == 0)
        report_urgent = LocalizationGetter.GetString("No");
      else if (_report.priority_id == 1)
        report_urgent = LocalizationGetter.GetString("Yes");

      return report_urgent;
    }

    public string IsOngoing()
    {
      string is_ongoing = LocalizationGetter.GetString("unknown");

      if (_report != null)
      {
        if (_report.is_ongoing == 1)
        {
          is_ongoing = LocalizationGetter.GetString("No");
        }
        else if (_report.is_ongoing == 2)
        {
          is_ongoing = LocalizationGetter.GetString("Yes");// +" Description : " + _report.report_frequency_text;
        }
        else if (_report.is_ongoing == 3)
        {
          is_ongoing = LocalizationGetter.GetString("NotSureUp");
        }
      }
      return is_ongoing;
    }

    public string HasInjuryDamage()
    {
      string injury_damage = LocalizationGetter.GetString("unknown");

      if (_report != null)
      {
        injury_damage injuryOrDamage =
            db.injury_damage.Where(item => item.id == _report.injury_damage_id).FirstOrDefault();
        if (injuryOrDamage != null)
        {
          injury_damage = injuryOrDamage.text_en;
          if (_report.reported_outside_id.Value == 2)
          {
            injury_damage = injury_damage;// +". Description : " + _report.injury_damage;
          }
        }
      }
      return injury_damage;
    }

    /// <summary>
    /// Returns "Jan 31, 2015"
    /// </summary>
    /// <param name="report_id"></param>
    /// <returns></returns>
    public string IncidentDateString()
    {
      string date_string = "";
      DateTime dt;
      if (_report != null)
      {
        dt = _report.incident_dt;
        date_string = m_DateTimeHelper.GetShortMonth(dt.Month) + " " + dt.Day.ToString() + ", " + dt.Year.ToString();
      }

      if (date_string.Trim().Length == 0)
        date_string = LocalizationGetter.GetString("unknown_date");

      return date_string.Trim();
    }

    /// <summary>
    /// Returns "January 31, 2015"
    /// </summary>
    /// <param name="report_id"></param>
    /// <returns></returns>
    public string IncidentDateStringMonthLong()
    {
      string date_string = "";
      DateTime dt;
      if (_report != null)
      {
        dt = _report.incident_dt;
        date_string = m_DateTimeHelper.GetFullMonth(dt.Month) + " " + dt.Day.ToString() + ", " + dt.Year.ToString();
      }

      if (date_string.Trim().Length == 0)
        date_string = LocalizationGetter.GetString("unknown_date");

      return date_string.Trim();
    }
    /// <summary>
    /// Returns "Jan 31, 2015"
    /// </summary>
    /// <param name="report_id"></param>
    /// <returns></returns>
    public string ReportedDateString()
    {
      string date_string = "";
      DateTime dt;
      if (_report != null)
      {
        dt = _report.reported_dt;
        date_string = m_DateTimeHelper.GetShortMonth(dt.Month) + " " + dt.Day.ToString() + ", " + dt.Year.ToString();
      }

      if (date_string.Trim().Length == 0)
        date_string = LocalizationGetter.GetString("unknown_date");

      return date_string.Trim();
    }
 

    public string ColorCode()
    {
      string color_code = "";
      color _color;

      if (_report != null)
      {
        int color_id = _report.report_color_id;

        _color = db.color.Where(item => item.id == _report.report_color_id).FirstOrDefault();
        if (_color != null)
          color_code = _color.color_code;
        else
        {
          _color = db.color.Where(item => item.id == 1).FirstOrDefault();
          color_code = _color.color_code;
        }
      }
      else
      {
        _color = db.color.Where(item => item.id == 1).FirstOrDefault();
        color_code = _color.color_code;
      }

      return color_code.Trim();
    }
    public string ColorDescr()
    {
      string color_descr = "";
      color _color;

      if (_report != null)
      {
        int color_id = _report.report_color_id;

        _color = db.color.Where(item => item.id == _report.report_color_id).FirstOrDefault();
        if (_color != null)
          color_descr = _color.color_description;
        else
        {
          _color = db.color.Where(item => item.id == 1).FirstOrDefault();
          color_descr = _color.color_description;
        }
      }
      else
      {
        _color = db.color.Where(item => item.id == 1).FirstOrDefault();
        color_descr = _color.color_description;
      }

      return color_descr.Trim();
    }

    public string ManagementKnowString()
    {
      string management_know = LocalizationGetter.GetString("unknown");

      if (_report != null)
      {
        if (_report.management_know_id.HasValue)
        {
          management_know _management_know = db.management_know.Where(item => item.id == _report.management_know_id.Value).FirstOrDefault();
          if (_management_know != null)
          {
            management_know = _management_know.text_en;
          }
        }
      }
      return management_know;
    }

    /// <summary>
    /// Returns the departments involved in reported
    /// </summary>
    /// <param name="report_id"></param>
    /// <returns></returns>
    public string DepartmentsString(bool is_cc)
    {
      string departments = "";

      if (_report != null)
      {
        List<report_department> _c_departments = db.report_department.Where(item => item.report_id == _report.id && item.added_by_reporter != false).ToList();
        company_department temp_c_dep;
        foreach (report_department _temp_dep in _c_departments)
        {
          temp_c_dep = db.company_department.Where(item => item.id == _temp_dep.department_id).FirstOrDefault();
          if (temp_c_dep != null)
          {
            if (departments.Length > 0)
              departments = departments + ", " + temp_c_dep.department_en.Trim();
            else
              departments = temp_c_dep.department_en.Trim();
          }
        }

        if (_report.other_department_name.Trim().Length > 0)
          if (departments.Length > 0)
            departments = departments + ", " + _report.other_department_name.Trim();
          else
            departments = _report.other_department_name.Trim();
      }
      if (departments.Length == 0)
        departments = LocalizationGetter.GetString("unknown_departments", is_cc);

      return departments;
    }

    public string ReporterCompanyRelationShort()
    {
      string relationship_text = LocalizationGetter.GetString("Other");

      if (_report != null)
      {
        if (db.report_relationship.Any(t => t.report_id == _report.id))
        {
          report_relationship _report_relationship = db.report_relationship.Where(t => t.report_id == _report.id).First();
          if ((_report_relationship.relationship_id.HasValue) && (_report_relationship.relationship_id.Value != 0) && (_report_relationship.relationship_id.Value != -1))
          {
            relationship temp_relationship = db.relationship.Where(item => item.id == _report_relationship.relationship_id.Value).FirstOrDefault();
            relationship_text = temp_relationship.relationship_en;
          }
          // 
          if ((_report_relationship.company_relationship_id.HasValue) && (_report_relationship.company_relationship_id.Value != 0) && (_report_relationship.company_relationship_id.Value != -1))
          {
            company_relationship temp_comp_relationship = db.company_relationship.Where(item => item.id == _report_relationship.company_relationship_id.Value).FirstOrDefault();
            if (temp_comp_relationship != null)
            {
              relationship_text = temp_comp_relationship.relationship_en.Trim();
            }
          }
        }
      }

      return relationship_text;
    }
    /// <summary>
    /// gets the secondary type of report
    /// </summary>
    /// <param name="report_id"></param>
    /// <returns></returns>
    public string SecondaryTypeString()
    {
      string secondary_type = "";

      if (_report != null)
      {
        if (db.report_secondary_type.Any(t => t.report_id == _report.id && t.added_by_reporter != false))
        {
          List<report_secondary_type> _report_secondary_type_list = db.report_secondary_type.Where(t => t.report_id == _report.id && t.added_by_reporter != false).ToList();
          foreach (report_secondary_type _report_secondary_type in _report_secondary_type_list)
          {
            if (_report_secondary_type.mandatory_secondary_type_id != null)
            {
              secondary_type_mandatory temp_sec_type = db.secondary_type_mandatory.Where(item => item.id == _report_secondary_type.mandatory_secondary_type_id).FirstOrDefault();
              if (temp_sec_type != null)
              {
                if ((secondary_type.Length > 0) && (!secondary_type.Contains(temp_sec_type.secondary_type_en.Trim())))
                  secondary_type = secondary_type + ", " + temp_sec_type.secondary_type_en.Trim();
                else
                  secondary_type = temp_sec_type.secondary_type_en.Trim();
              }
            }

            if ((_report_secondary_type.secondary_type_id != 0) && (_report_secondary_type.secondary_type_id != -1))
            {
              company_secondary_type temp_comp_sec_type = db.company_secondary_type.Where(item => item.id == _report_secondary_type.secondary_type_id).FirstOrDefault();
              if (temp_comp_sec_type != null)
              {
                if ((secondary_type.Length > 0) && (!secondary_type.Contains(temp_comp_sec_type.secondary_type_en.Trim())))
                  secondary_type = secondary_type + ", " + temp_comp_sec_type.secondary_type_en.Trim();
                else
                  secondary_type = temp_comp_sec_type.secondary_type_en.Trim();
              }
            }

            if (_report_secondary_type.secondary_type_nm.Trim() != "")
            {
              if ((secondary_type.Length > 0) && (!secondary_type.Contains(_report_secondary_type.secondary_type_nm.Trim())))
                secondary_type = secondary_type + ", " + _report_secondary_type.secondary_type_nm.Trim();
              else
                secondary_type = _report_secondary_type.secondary_type_nm.Trim();
            }
            else if (_report_secondary_type.secondary_type_id == 0)
            {
              if (secondary_type.Length > 0)
              {
                secondary_type = secondary_type + ", " + LocalizationGetter.GetString("Other");
              }
              else
              {
                secondary_type = LocalizationGetter.GetString("Other");
              }
            }
          }

        }
      }
      if (secondary_type.Length == 0)
        secondary_type = LocalizationGetter.GetString("unknown_secondary_type");

      return secondary_type.Trim();
    }

    public string DaysLeftClosedSpamMessage(int delay_allowed)
    {
      if (_report.status_id == (int)CaseStatusConstants.CaseStatusValues.Spam)
        return $"Sent on {m_DateTimeHelper.ConvertDateToShortString(_report.last_update_dt) }";
      else if (_report.status_id == (int)CaseStatusConstants.CaseStatusValues.Closed)
        return $"Closed on {m_DateTimeHelper.ConvertDateToShortString(_report.last_update_dt) }";
      else
      {
        ReportModel rm = new ReportModel(ID);
        int days_left = rm.GetThisStepDaysLeft(delay_allowed);
        return $"{days_left}" + (days_left == 1 ? " day left" : " days left");
      }
    }
    public string GetAgentName()
    {
      if (this._report.agent_id != null && this._report.agent_id > 0)
      {
        return db.user.Find(this._report.agent_id).first_nm;
      }
      else
      {
        return "";
      }

    }

    public string Last_investigation_case_closure_reason()
    {
      ReportModel rm = new ReportModel(ID);
      report_investigation_status last_status = rm._last_investigation_status();

      if ((last_status == null) || (!last_status.case_closure_reason_id.HasValue))
      {
        return null;
      }

      var reason = db.case_closure_reason.FirstOrDefault(x => x.id == last_status.case_closure_reason_id);

      return reason == null ? null : reason.case_closure_reason_en;
    }

    public string InvestigationMethodology()
    {
      return $"{db.report_inv_notes.FirstOrDefault(x => x.report_id == ID & x.type == 2)?.note}";
    }

    public string FactsEstablished()
    {
      return $"{db.report_inv_notes.FirstOrDefault(x => x.report_id == ID & x.type == 1)?.note}";
    }

    public string ExecutiveSummary()
    {
      var report_cc_crime = db.report_cc_crime
          .Where(x => x.report_id == ID)
          .FirstOrDefault();

      return $"{report_cc_crime?.executive_summary}";
    }


    public string InvestigationStatusString()
    {
      string status = "";

      investigation_status _status = db.investigation_status.Where(item => item.id == _report.status_id).FirstOrDefault();
      if (_status != null)
      {
        status = _status.investigation_status_en;
      }

      return status.Trim();
    }
  }
}