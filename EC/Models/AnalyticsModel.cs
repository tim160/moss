using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Models.Database;
using EC.Controllers.API;
using EC.Constants;

namespace EC.Models
{
  public class AnalyticsModel : BaseModel
  {

    public Object AnalyticsCasesTurnAroundTime(int[] idAdditionalComapanies)
    {
      List<company> company_item = db.company.Where(c => idAdditionalComapanies.Contains(c.id)).ToList();

      var resultobj = new
      {
        resultAroundTime = calcAroundTime(company_item),
        CaseManagamentTime = calculateResponseTimeCompanies(company_item),
      };
      return resultobj;
    }
    private List<TodaySnapshot> calcAroundTime(List<company> companies)
    {
      int[] _array = new int[] { 0, 0, 0, 0 };
      foreach (var dbCompany in companies)
      {
        _array = calculateReportsInCompany(dbCompany, ref _array);
      }
      List<TodaySnapshot> resultAroundTime = new List<TodaySnapshot>();
      resultAroundTime.Add(new TodaySnapshot
      {
        numberOfCases = _array[0],
        miniSquareColor = "#d47472",
        titleHeaderLegend = "New Report",
      });
      resultAroundTime.Add(new TodaySnapshot
      {
        numberOfCases = _array[1],
        miniSquareColor = "#ff9b42",
        titleHeaderLegend = "Report Review",
      });
      resultAroundTime.Add(new TodaySnapshot
      {
        numberOfCases = _array[2],
        miniSquareColor = "#3099be",
        titleHeaderLegend = "Under Investigation",
      });
      resultAroundTime.Add(new TodaySnapshot
      {
        numberOfCases = _array[3],
        miniSquareColor = "#64cd9b",
        titleHeaderLegend = "Awaiting Sign-Off",
      });
      return resultAroundTime;
    }
    private object calculateResponseTimeCompanies(List<company> companies)
    {
      int[] _array = new int[] { 0, 0, 0, 0 };
      foreach (var company in companies)
      {
        _array[0] += company.step1_delay;
        _array[1] += company.step2_delay;
        _array[2] += company.step3_delay;
        _array[3] += company.step4_delay;
      }
      _array[0] = _array[0] / companies.Count();
      _array[1] = _array[1] / companies.Count();
      _array[2] = _array[2] / companies.Count();
      _array[3] = _array[3] / companies.Count();

      return new[]
      {
                new {Name = "New Report", value = _array[0] },
                new {Name = "Report Review", value = _array[1] },
                new {Name = "Under Inves", value = _array[2] },  //WARNING Under Inves
                new {Name = "Awaiting Sign-Off", value = _array[3] }
            };
    }

    private int[] calculateReportsInCompany(company dbCompany, ref int[] _array)
    {
      int[] delay_allowed = new int[4];
      delay_allowed[0] = dbCompany.step1_delay;
      delay_allowed[1] = dbCompany.step2_delay;
      delay_allowed[2] = dbCompany.step3_delay;
      delay_allowed[3] = dbCompany.step4_delay;


      List<int> allowed_statuses = new List<int> { (int)CaseStatusConstants.CaseStatusValues.Pending, (int)CaseStatusConstants.CaseStatusValues.Review, (int)CaseStatusConstants.CaseStatusValues.Investigation, (int)CaseStatusConstants.CaseStatusValues.Completed };

      List<report> _all_reports = db.report.Where(
              c => c.company_id == dbCompany.id &&
              c.status_id != (int)CaseStatusConstants.CaseStatusValues.Closed &&
              c.status_id != (int)CaseStatusConstants.CaseStatusValues.Spam
              ).ToList();
      for (int i = 0; i < allowed_statuses.Count; i++)
      {
        foreach (var report in _all_reports)
        {
          _array[i] += CheckReportsForAllowed(report, allowed_statuses[i], delay_allowed[i]);
        }
        //_array[i] += _all_reports.Where(t => t.last_update_dt != null && t.status_id == allowed_statuses[i]
        //&& (int)(delay_allowed[i] - (DateTime.Today - t.last_update_dt).TotalDays) < 0).Count();

        //if (allowed_statuses[i] == ECGlobalConstants.investigation_status_pending)
        //    _array[i] += _all_reports.Where(t => t.last_update_dt == null && t.status_id == allowed_statuses[i]
        //    && (int)(delay_allowed[i] - (DateTime.Today - t.reported_dt).TotalDays) < 0).Count();
      }
      return _array;
    }
    private int CheckReportsForAllowed(report report, int allowedStatus, int delay_allowed)
    {
      if ((int)CaseStatusConstants.CaseStatusValues.Pending == allowedStatus)
      {
        if (report.status_id == allowedStatus && delay_allowed - (DateTime.Today - report.reported_dt).TotalDays < 0)
        {
          return 1;
        }
      }

      if (report.status_id == allowedStatus &&
          delay_allowed - (DateTime.Today - report.last_update_dt).TotalDays < 0)
      {
        return 1;
      }
      return 0;
      //int result = report.Where(t => t.last_update_dt != null && t.status_id == allowed_statuses[i]
      //&& (int)(delay_allowed[i] - (DateTime.Today - t.last_update_dt).TotalDays) < 0).Count();

      //if (allowed_statuses[i] == )
      //    _array[i] += _all_reports.Where(t => t.last_update_dt == null && t.status_id == allowed_statuses[i]
      //    && (int)(delay_allowed[i] - (DateTime.Today - t.reported_dt).TotalDays) < 0).Count();
      //return 0;
    }

    public int[] AnalyticsCasesArrayByDate(DateTime? _start, int[] ArrCompanyId)
    {
      DateTime _real_start;

      if (_start.HasValue)
        _real_start = _start.Value;
      else
        _real_start = new DateTime(2019, 1, 1);

      List<report> _all_reports = db.report.Where(c => ArrCompanyId.Contains(c.company_id)).ToList();

      var _all_reports_ids = _all_reports.Select(t => t.id);
      int[] _array = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

      if (!_start.HasValue)
      {
        for (int i = 0; i < _array.Length; i++)
        {
          _array[i] = _all_reports.Where(t => t.status_id == i + 1).Count();
        }
      }
      else
      {
        _all_reports = _all_reports.Where(t => t.reported_dt <= _real_start).ToList();

        var refGroupInvestigationStatuses = (from m in db.report_investigation_status
                                             where _all_reports_ids.Contains(m.report_id) && m.created_date <= _real_start
                                             group m by m.report_id into refGroup
                                             select refGroup.OrderByDescending(x => x.id).FirstOrDefault());
        for (int i = 0; i < _array.Length; i++)
        {
          _array[i] = (from m in refGroupInvestigationStatuses
                       where m.investigation_status_id == i + 1
                       select m.report_id).Count();
        }

        // var statuses_list = db.report_investigation_status.Where(item => ((_all_reports_ids.Contains(item.report_id) && (item.investigation_status_id == i))).FirstOrDefault();

      }
      return _array;
    }


  }
}