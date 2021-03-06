﻿using EC.Constants;
using EC.Localization;
using EC.Models;
using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace EC.Controllers.API
{
    public class AnalyticsDashboardAPIController : BaseApiController
    {
        [HttpGet]
        public object Get([FromUri] Filter model)
        {
            user user = (user)System.Web.HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            // -- DEBUG
            //user = DB.user.FirstOrDefault(x => x.id == 2);
            // -- DEBUG
            if (user == null || user.id == 0)
            {
                return null;
            }

            var company_behavioral = DB.company_root_cases_behavioral.Where(x => x.company_id == user.company_id).ToList();
            var company_external = DB.company_root_cases_external.Where(x => x.company_id == user.company_id).ToList();
            var company_organizational = DB.company_root_cases_organizational.Where(x => x.company_id == user.company_id).ToList();
            var idsB = company_behavioral.Select(x => x.id).ToList();
            var idsE = company_external.Select(x => x.id).ToList();
            var idsO = company_organizational.Select(x => x.id).ToList();

            var reportInfo = DB.report_investigation_methodology
                .Where(x =>
                    (x.report_secondary_type_id == model.SecondaryType || model.SecondaryType == 0)
                    && (idsB.Contains(x.company_root_cases_behavioral_id.Value) || idsE.Contains(x.company_root_cases_external_id.Value) || idsO.Contains(x.company_root_cases_organizational_id.Value)))
                .ToList();

            var behavioral = reportInfo
                .Where(x => x.company_root_cases_behavioral_id.HasValue)
                .GroupBy(x => x.company_root_cases_behavioral_id)
                .Select(x => new
                {
                    id = x.Key,
                    name = company_behavioral.FirstOrDefault(z => z.id == x.Key)?.name_en,
                    val = x.Count()
                }).OrderByDescending(x => x.val)
                .ToList();


            var external = reportInfo
                .Where(x => x.company_root_cases_external_id.HasValue)
                .GroupBy(x => x.company_root_cases_external_id)
                .Select(x => new
                {
                    id = x.Key,
                    name = company_external.FirstOrDefault(z => z.id == x.Key)?.name_en,
                    val = x.Count()
                }).OrderByDescending(x => x.val)
                .ToList();

            var organizational = reportInfo
                .Where(x => x.company_root_cases_organizational_id.HasValue)
                .GroupBy(x => x.company_root_cases_organizational_id)
                .Select(x => new
                {
                    id = x.Key,
                    name = company_organizational.FirstOrDefault(z => z.id == x.Key)?.name_en,
                    val = x.Count()
                }).OrderByDescending(x => x.val)
                .ToList();

            //var secondaryTypes = DB.company_secondary_type.Where(x => x.company_id == user.company_id).ToList();
            //secondaryTypes.Insert(0, new company_secondary_type { id = 0, secondary_type_en = "All Incident Types" });
            var colors = DB.color.OrderBy(x => x.id).Select(x => "#" + x.color_code);
            return new
            {
                //SecondaryTypes = secondaryTypes,
                Behavioral = behavioral,
                BehavioralTotal = behavioral.Sum(x => x.val),
                External = external,
                ExternalTotal = external.Sum(x => x.val),
                Organizational = organizational,
                OrganizationalTotal = organizational.Sum(x => x.val),
                //Colors = DB.color.OrderBy(x => x.id).Select(x => "#" + x.color_code),
            };
        }

        [HttpPost]
        public Object GetMenuDashboard([FromUri] int[] id)
        {
            user user = (user)System.Web.HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return null;

            var DepartmentsList = DB.company_department.Where(s => id.Contains(s.company_id)).Select(x => new { x.id, x.department_en }).Distinct().ToList();
            DepartmentsList.Add(new { id = 0, department_en = LocalizationGetter.GetString("Not Listed") });
            
            var LocationsList = DB.company_location.Where(s => id.Contains(s.company_id)).Select(x => new { x.id, x.location_en }).Distinct().ToList();
            LocationsList.Add(new { id = 0, location_en = LocalizationGetter.GetString("Other") });

            var SecondaryTypesList = DB.company_secondary_type.Where(s => id.Contains(s.company_id)).Select(m => new { m.id, m.secondary_type_en }).OrderBy(t => t.secondary_type_en).Distinct().ToList();
            SecondaryTypesList.Add(new { id = 0, secondary_type_en = LocalizationGetter.GetString("Other") });

            var RelationTypesList = DB.company_relationship.Where(s => id.Contains(s.company_id)).Select(m => new { m.id, m.relationship_en }).OrderBy(t => t.relationship_en).Distinct().ToList();
            RelationTypesList.Add(new { id = 0, relationship_en = LocalizationGetter.GetString("Other") });

            var resultObj = new
            {
                DepartmentsList,
                LocationsList,
                SecondaryTypesList,
                RelationTypesList
            };

            return ResponseObject2Json(resultObj);
        }

    }

    public class Filter
    {
        public int SecondaryType { get; set; }
    }
}
