﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;

using EC.Models;
using EC.Models.Database;
using EC.Constants;
using EC.Core.Common;

using EC.Models.ViewModel;
using EC.Common.Interfaces;
using EC.Localization;

namespace EC.Controllers.API
{
    public class NewCaseInvestigationNotesController : BaseApiController
    {
        public class Filter
        {
            public int id { get; set; }

            public int? Company_secondary_type_add { get; set; }
            public int? Company_secondary_type_delete { get; set; }

            public int? Mediator_add { get; set; }
            public int? Mediator_delete { get; set; }
            public int? Mode { get; set; }


            public int? Department_add { get; set; }
            public int? Department_delete { get; set; }

            public string Note1 { get; set; }

            public string Note2 { get; set; }

            public int? inv_meth_st_id { get; set; }
            public int? inv_meth_bf_id { get; set; }
            public int? inv_meth_ei_id { get; set; }
            public int? inv_meth_ci_id { get; set; }
            public string inv_meth_bf_note { get; set; }
            public string inv_meth_ei_note { get; set; }
            public string inv_meth_ci_note { get; set; }


            /* ADD NEW PERSON */
            public string addPersonFirstName { get; set; }
            public string addPersonLastName { get; set; }
            public string addPersonTitle { get; set; }
            public int? addPersonRole { get; set; }

        }

        [HttpGet]
        public object Get([FromUri] Filter filter)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            // DEBUG
            //user = DB.user.FirstOrDefault(x => x.id == 2);
            // DEBUG
            if (user == null || user.id == 0)
            {
                return null;
            }

            UserModel um = new UserModel(user.id);

            var company_secondary_types = DB.company_secondary_type
                .Where(x => x.company_id == um._user.company_id)
                .OrderBy(x => x.secondary_type_en)
                .ToList();

            var report_secondary_type_selected = DB.report_secondary_type
                .Where(x => x.report_id == filter.id)
                .ToList();

            var all_mediators = DB.user
                .Where(item => (item.company_id == user.company_id) && (item.role_id == 4 || item.role_id == 5))
                .ToList();

            var mediator_involved = DB.report_mediator_involved
                .Where(x => x.report_id == filter.id & x.status_id == 2)
                .ToList();

            var mediator_not_involved = DB.report_non_mediator_involved
                .Where(x => x.report_id == filter.id)
                .ToList();

            var mediator_assigned = DB.report_mediator_assigned
                .Where(x => x.report_id == filter.id & x.status_id == 2)
                .ToList();

            var departments = DB.company_department
                .Where(x => x.company_id == user.company_id && x.status_id == 2)
                .OrderBy(x => x.department_en)
                .ToList();

            var m = new
            {
                report_secondary_type_selected = company_secondary_types
                    .Where(x => report_secondary_type_selected.Select(z => z.secondary_type_id).Contains(x.id))
                    .Select(x => new
                    {
                        id = x.id,
                        secondary_type_en = x.secondary_type_en,
                        added_by_reporter = report_secondary_type_selected.FirstOrDefault(z => z.secondary_type_id == x.id).added_by_reporter,
                    })
                    .OrderByDescending(x => x.added_by_reporter)
                    .ThenBy(x => x.secondary_type_en)
                    .ToList(),

                report_secondary_type_selected_avilable = company_secondary_types
                    .Where(x => !report_secondary_type_selected.Select(z => z.secondary_type_id).Contains(x.id) && x.status_id == 2)
                    .ToList(),

                mediator_involved = all_mediators
                    .Where(x => mediator_involved.Select(z => z.mediator_id).Contains(x.id))
                    .Select(x => new {
                        id = x.id,
                        first_nm = x.first_nm,
                        last_nm = x.last_nm,
                        added_by_reporter = mediator_involved.FirstOrDefault(z => z.mediator_id == x.id).added_by_reporter,
                    }).ToList(),

                mediator_not_involved = mediator_not_involved,

                mediator_all = all_mediators
                    .Where(x => !mediator_involved.Select(z => z.mediator_id).Contains(x.id) & !mediator_assigned.Select(z => z.mediator_id).Contains(x.id))
                    .Select(x => new {
                        id = x.id,
                        first_nm = x.first_nm,
                        last_nm = x.last_nm
                    }).ToList(),

                departments_all = departments,

                departments_report = DB.report_department
                    .Where(x => x.report_id == filter.id)
                    .ToList()
                    .Select(x => new {
                        id = x.department_id,
                        added_by_reporter = x.added_by_reporter,
                        name = departments.FirstOrDefault(z => z.id == x.department_id)?.department_en,
                    })
                    .OrderByDescending(x => x.added_by_reporter)
                    .ThenBy(x => x.name)
                    .ToList(),

                note1 = DB.report_inv_notes.FirstOrDefault(x => x.report_id == filter.id & x.type == 1)?.note,

                note2 = DB.report_inv_notes.FirstOrDefault(x => x.report_id == filter.id & x.type == 2)?.note,

                company_root_cases_behavioral = DB.company_root_cases_behavioral
                    .Where(x => x.company_id == user.company_id & x.status_id == 2)
                    .OrderBy(x => x.name_en)
                    .ToList(),

                company_root_cases_external = DB.company_root_cases_external
                    .Where(x => x.company_id == user.company_id & x.status_id == 2)
                    .OrderBy(x => x.name_en)
                    .ToList(),

                company_root_cases_organizational = DB.company_root_cases_organizational
                    .Where(x => x.company_id == user.company_id & x.status_id == 2)
                    .OrderBy(x => x.name_en)
                    .ToList(),

                report_investigation_methodology = DB.report_investigation_methodology
                    .Where(x => x.report_id == filter.id)
                    .ToList()
                    .Where(x => report_secondary_type_selected.Select(z => z.secondary_type_id).ToList().Contains(x.report_secondary_type_id))
                    .ToList(),

                addNewPersonRoles = DB.role_in_report.OrderBy(x => x.role_en).ToList()
            };

            m.addNewPersonRoles.Insert(0, new role_in_report { id = 0, role_en = LocalizationGetter.GetString("Other") });


            return ResponseObject2Json(m);
        }

        [HttpPost]
        public object Post([FromBody] Filter filter)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            // DEBUG
            //user = DB.user.FirstOrDefault(x => x.id == 2);
            // DEBUG
            if (user == null || user.id == 0)
            {
                return null;
            }
              LogModel logModel = new LogModel();

            UserModel um = new UserModel(user.id);

            if (filter.Company_secondary_type_add.HasValue)
            {
                var company_secondary_type = DB.company_secondary_type.FirstOrDefault(x => x.company_id == um._user.company_id & x.id == filter.Company_secondary_type_add);
                if (company_secondary_type != null)
                {
                    var report_secondary_type = DB.report_secondary_type.FirstOrDefault(x => x.report_id == filter.id & x.secondary_type_id == company_secondary_type.id);
                    if (report_secondary_type == null)
                    {
                        DB.report_secondary_type.Add(new report_secondary_type
                        {
                            last_update_dt = DateTime.Now,
                            report_id = filter.id,
                            secondary_type_id = company_secondary_type.id,
                            secondary_type_nm = company_secondary_type.secondary_type_en,
                            user_id = user.id,
                            added_by_reporter = false,
                        });
                        DB.SaveChanges();

                        logModel.UpdateReportLog(user.id, 30, filter.id, company_secondary_type.secondary_type_en, null, "");
                    }
                }
            }

            if (filter.Company_secondary_type_delete.HasValue)
            {
                var report_secondary_type = DB.report_secondary_type.FirstOrDefault(x => x.report_id == filter.id & x.secondary_type_id == filter.Company_secondary_type_delete.Value & x.added_by_reporter != true);
                DB.report_secondary_type.Remove(report_secondary_type);
                DB.SaveChanges();
                logModel.UpdateReportLog(user.id, 31, filter.id, report_secondary_type.secondary_type_nm, null, "");
            }

            if (filter.Mediator_add.HasValue)
            {
                var model = DB.report_mediator_involved.FirstOrDefault(x => x.report_id == filter.id & x.mediator_id == filter.Mediator_add.Value);
                if (model != null)
                {
                    model.status_id = 2;
                }
                else
                {
                    DB.report_mediator_involved.Add(new report_mediator_involved
                    {
                        mediator_id = filter.Mediator_add.Value,
                        report_id = filter.id,
                        last_update_dt = DateTime.Now,
                        status_id = 2,
                        user_id = user.id,
                        added_by_reporter = false,
                    });
                    logModel.UpdateReportLog(user.id, 31, filter.id, model.mediator_id.ToString(), null, "");

                }
                DB.SaveChanges();
            }

            if (filter.Mediator_delete.HasValue)
            {
                if (filter.Mode == 2)
                {
                    var model = DB.report_non_mediator_involved.FirstOrDefault(x => x.report_id == filter.id & x.id == filter.Mediator_delete.Value);
                    DB.report_non_mediator_involved.Remove(model);
                    logModel.UpdateReportLog(user.id, 32, filter.id, model.Role, null, "");
                }
                else
                {
                    var model = DB.report_mediator_involved.FirstOrDefault(x => x.report_id == filter.id & x.mediator_id == filter.Mediator_delete.Value & x.added_by_reporter == false);
                    model.status_id = 1;
                    logModel.UpdateReportLog(user.id, 32, filter.id, model.mediator_id.ToString(), null, "");
                }
                DB.SaveChanges();
                

            }

            if (filter.Department_add.HasValue)
            {
                var model = DB.report_department.FirstOrDefault(x => x.report_id == filter.id & x.department_id == filter.Department_add.Value & x.added_by_reporter == false);
                if (model == null)
                {
                    model = new report_department
                    {
                        department_id = filter.Department_add.Value,
                        report_id = filter.id,
                        added_by_reporter = false,
                    };
                    DB.report_department.Add(model);
                    DB.SaveChanges();
                    logModel.UpdateReportLog(user.id, 35, filter.id, model.department_id.ToString(), null, "");
                }
            }

            if (filter.Department_delete.HasValue)
            {
                var model = DB.report_department.FirstOrDefault(x => x.report_id == filter.id & x.department_id == filter.Department_delete.Value & x.added_by_reporter == false);
                DB.report_department.Remove(model);
                DB.SaveChanges();

                logModel.UpdateReportLog(user.id, 37, filter.id, model.department_id.ToString(), null, "");

            }

            if ((filter.Note1 != null) || (filter.Note2 != null))
            {
                var type = filter.Note1 != null ? 1 : 2;
                var model = DB.report_inv_notes.FirstOrDefault(x => x.report_id == filter.id & x.type == type);
                if (model == null)
                {
                    DB.report_inv_notes.Add(new report_inv_notes
                    {
                        added_by_reporter = false,
                        last_update_dt = DateTime.Now,
                        user_id = user.id,
                        report_id = filter.id,
                        note = filter.Note1 != null ? filter.Note1 : filter.Note2,
                        type = type,
                    });
                    if (type == 1)
                        logModel.UpdateReportLog(user.id, 38, filter.id, filter.Note1, null, "");
                    else
                        logModel.UpdateReportLog(user.id, 47, filter.id, filter.Note2, null, "");
                }
                else
                {
                    model.last_update_dt = DateTime.Now;
                    model.user_id = user.id;
                    model.note = filter.Note1 != null ? filter.Note1 : filter.Note2;
                    if (type == 1)
                        logModel.UpdateReportLog(user.id, 38, filter.id, filter.Note1, null, "");
                    else
                        logModel.UpdateReportLog(user.id, 48, filter.id, filter.Note2, null, "");
                }
                DB.SaveChanges();
            }

            if (filter.inv_meth_st_id.HasValue)
            {
                var model = DB.report_investigation_methodology.FirstOrDefault(x => x.report_id == filter.id & x.report_secondary_type_id == filter.inv_meth_st_id);
                if (model == null)
                {
                    model = new report_investigation_methodology
                    {
                        report_id = filter.id,
                        report_secondary_type_id = filter.inv_meth_st_id.Value
                    };
                    DB.report_investigation_methodology.Add(model);
                }
                if (filter.inv_meth_bf_id.HasValue)
                {
                    model.company_root_cases_behavioral_id = filter.inv_meth_bf_id.Value;
                    model.company_root_cases_behavioral_update_dt = DateTime.Now;
                    model.company_root_cases_behavioral_user_id = user.id;
                }
                if (filter.inv_meth_ei_id.HasValue)
                {
                    model.company_root_cases_external_id = filter.inv_meth_ei_id.Value;
                    model.company_root_cases_external_update_dt = DateTime.Now;
                    model.company_root_cases_external_user_id = user.id;
                }
                if (filter.inv_meth_ci_id.HasValue)
                {
                    model.company_root_cases_organizational_id = filter.inv_meth_ci_id.Value;
                    model.company_root_cases_organizational_note_update_dt = DateTime.Now;
                    model.company_root_cases_organizational_note_user_id = user.id;
                }
                if (filter.inv_meth_bf_note != null)
                {
                    model.company_root_cases_behavioral_note = filter.inv_meth_bf_note;
                    model.company_root_cases_organizational_update_dt = DateTime.Now;
                    model.company_root_cases_organizational_user_id = user.id;
                }
                if (filter.inv_meth_ei_note != null)
                {
                    model.company_root_cases_external_note = filter.inv_meth_ei_note;
                    model.company_root_cases_organizational_note_update_dt = DateTime.Now;
                    model.company_root_cases_organizational_note_user_id = user.id;
                }
                if (filter.inv_meth_ci_note != null)
                {
                    model.company_root_cases_organizational_note = filter.inv_meth_ci_note;
                    model.company_root_cases_organizational_note_update_dt = DateTime.Now;
                    model.company_root_cases_organizational_note_user_id = user.id;
                }

                DB.SaveChanges();
            }

            if (filter.addPersonFirstName != null)
            {
                var item = DB.report_non_mediator_involved.FirstOrDefault(x => x.report_id == filter.id & x.Name == filter.addPersonFirstName & x.last_name == filter.addPersonLastName & x.Title == filter.addPersonTitle);
                if (item == null)
                {
                    DB.report_non_mediator_involved.Add(new report_non_mediator_involved
                    {
                        created_dt = DateTime.Now,
                        last_name = filter.addPersonLastName,
                        Name = filter.addPersonFirstName,
                        report_id = filter.id,
                        Title = filter.addPersonTitle,
                        role_in_report_id = filter.addPersonRole,
                        added_by_reporter = false,
                    });
                    DB.SaveChanges();
                    logModel.UpdateReportLog(user.id, 51, filter.id, $"{filter.addPersonLastName} {filter.addPersonFirstName}", null, "");
                }
            }

            return Get(filter);
        }
    }
}