using EC.Controllers.ViewModel;
using EC.Models.Database;
using EC.Models.Utils;
using EC.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace EC.Models.ViewModels
{
    public class ReportAddModel
    {
        public ReportModelResult AddReport(ReportViewModel model, out string password)
        {
            var resultModel = new ReportModelResult();
            Guid _guid = Guid.NewGuid();
            var currentReport = new report();

            using (ECEntities adv = new ECEntities())
            {
                using (var transaction = adv.Database.BeginTransaction())
                {
                    try
                    {
                        #region creating report
                        /**
                        * addUserID to the getAttachment.user_id, add report_id to the getAttachment.report_id
                        * */
                        var getAttachmentList = FileUtils.SaveFile(model.files);
                        MailAddress mail = null;
                        string nameOfEmail = String.Empty;

                        if (model.userEmail != null)
                        {
                            mail = new MailAddress(model.userEmail);
                            nameOfEmail = mail.User;
                        }

                        int notification = Convert.ToInt16(model.sendUpdates);
                        //if not checked = 3, if  check = 1
                        if (notification != 1)
                        {
                            notification = 3;
                        }

                        var generateModel = new GenerateRecordsModel();

                        password = generateModel.GeneretedPassword();
                        user newUser = new user()
                        {
                            company_id = model.currentCompanyId,
                            role_id = 8,
                            status_id = 2,
                            address_id = 0,
                            first_nm = model.userName == null ? "" : model.userName,
                            last_nm = model.userLastName == null ? "" : model.userLastName,
                            login_nm = "",
                            password = PasswordUtils.GetHash(password),
                            email = model.userEmail == null ? "" : model.userEmail,
                            preferred_contact_method_id = 2,
                            question_ds = "",
                            answer_ds = "",
                            user_id = 0,
                            last_update_dt = DateTime.Now,
                            preferred_email_language_id = 1,
                            photo_path = "",
                            notification_messages_actions_flag = notification,
                            notification_new_reports_flag = 1,
                            notification_marketing_flag = 1,
                            guid = _guid,
                            notification_summary_period = 1,
                            phone = model.userTelephone
                        };

                        newUser = adv.user.Add(newUser);
                        adv.SaveChanges();
                        string reporter_login = generateModel.GenerateReporterLogin();

                        while (adv.user.Any(t => t.login_nm.Trim().ToLower() == reporter_login.Trim().ToLower()))
                        {
                            reporter_login = generateModel.GenerateReporterLogin();
                        }

                        newUser.login_nm = reporter_login;
                        currentReport = model.Merge(currentReport);
                        currentReport.agent_id = model.agentId;
                        currentReport.user_id = newUser.id;
                        currentReport.reporter_user_id = newUser.id;
                        currentReport.report_by_myself = model.report_by_myself;
                        currentReport.guid = Guid.NewGuid();
                        adv.user.AddOrUpdate(newUser);
                        currentReport.status_id = 1;
                        currentReport.last_update_dt = DateTime.Now;
                        currentReport.user_id = newUser.id;

                        currentReport = adv.report.Add(currentReport);
                        adv.SaveChanges();
                        //  t = adv.SaveChanges();
                        currentReport.display_name = generateModel.GenerateCaseNumber(currentReport.id, currentReport.company_id, currentReport.company_nm);
                        currentReport.report_color_id = GetNextColor(currentReport.company_id, currentReport.id);
                        //t = adv.SaveChanges();

                        if (getAttachmentList != null)
                        {
                            for (int i = 0; i < getAttachmentList.Count; i++)
                            {
                                if (getAttachmentList[i] != null)
                                {
                                    getAttachmentList[i].report_id = currentReport.id;
                                    getAttachmentList[i].user_id = newUser.id;
                                    adv.attachment.Add(getAttachmentList[i]);
                                }
                            }
                        }

                        if (model.mediatorsInvolved != null)
                        {
                            foreach (string mediatorId in model.mediatorsInvolved)
                            {
                                report_mediator_involved result = new report_mediator_involved()
                                {
                                    user_id = 1,
                                    mediator_id = Convert.ToInt32(mediatorId),
                                    status_id = 2,
                                    last_update_dt = DateTime.Now,
                                    report_id = currentReport.id
                                };

                                adv.report_mediator_involved.Add(result);
                                //adv.SaveChanges();
                            }
                        }

                        List<report_department> departments = model.GetReportDepartment(currentReport.id);
                        foreach (report_department department in departments)
                        {
                            department.added_by_reporter = true;
                            AddReportDepartment(department);
                            //t = adv.SaveChanges();
                        }


                        #region Status Saving = pending
                        report_investigation_status _review_status = new report_investigation_status();
                        _review_status.created_date = DateTime.Now;
                        _review_status.investigation_status_id = 1;
                        _review_status.report_id = currentReport.id;
                        _review_status.user_id = newUser.id;
                        _review_status.description = "";
                        ///adv.report_investigation_status.Add(_review_status);
                        ////   t = adv.SaveChanges();

                        adv.report_investigation_status.Add(_review_status);

                        //var report = adv.report.FirstOrDefault(x => x.id == _review_status.report_id);


                        //adv.SaveChanges();
                        #endregion

                        //savind secondary type 

                        /*CustomSecondaryType == false*/
                        List<report_secondary_type> secondaryTypeList = new List<report_secondary_type>();
                        foreach (var item in model.whatHappened)
                        {
                            report_secondary_type temp = new report_secondary_type();
                            temp.report_id = currentReport.id;
                            temp.secondary_type_nm = "";
                            temp.secondary_type_id = 0;
                            //check is it custom
                            if (model.CustomSecondaryType)
                            {
                                temp.mandatory_secondary_type_id = item;
                            }
                            else
                            {
                                temp.secondary_type_id = item;
                            }
                            //check is it other
                            if (item == 0 && model.caseInformationReportDetail != null)
                            {
                                temp.secondary_type_nm = model.caseInformationReportDetail;
                            }

                            temp.last_update_dt = DateTime.Now;
                            temp.user_id = 1;
                            temp.added_by_reporter = true;
                            //adv.

                            adv.report_secondary_type.Add(temp);
                        }

                        /*report_relationship*/
                        report_relationship rep = new report_relationship();
                        rep.report_id = currentReport.id;
                        rep.user_id = currentReport.user_id;
                        rep.last_update_dt = DateTime.Now;

                        if (currentReport.not_current_employee != null && currentReport.not_current_employee.Trim().Length > 0)
                        {
                            //other
                            rep.relationship_nm = currentReport.not_current_employee.Trim();
                            rep.relationship_id = null;
                            if (currentReport.company_relationship_id != 0)
                            {
                                //'Former Employee'
                                rep.company_relationship_id = currentReport.company_relationship_id;
                            }
                        }
                        else
                        {
                            int count = adv.company_relationship.Where(rel => rel.status_id == 2).Count();
                            if (count > 0)
                            {
                                //custom
                                rep.company_relationship_id = currentReport.company_relationship_id;
                            }
                            else
                            {
                                rep.relationship_id = currentReport.company_relationship_id;
                            }
                        }
                        adv.report_relationship.Add(rep);

                        /*for(var i = 0; i < model.personName.Count;  i++)
                        {
                            var person = new report_non_mediator_involved
                            {
                                created_dt = DateTime.Now,
                                last_name = model.personLastName.ToList()[i],
                                Name = model.personName.ToList()[i],
                                report_id = currentReport.id,
                                Title
                            };
                        }*/
                        //foreach(var person in model.pe)

                        ////   if (model.mediatorsInvolved != null)
                        foreach (var item in adv.company_case_routing_location.Where(x => x.company_id == currentReport.company_id & x.company_location_id == currentReport.location_id).ToList())
                        {
                            if ((model.mediatorsInvolved == null) || !model.mediatorsInvolved.Contains(item.user_id.ToString()))
                            {
                                var cl = adv.company_location.FirstOrDefault(x => x.id == item.company_location_id);
                                if ((cl != null) && (cl.status_id == 2))
                                {
                                    adv.report_mediator_assigned.Add(new report_mediator_assigned
                                    {
                                        report_id = currentReport.id,
                                        by_location_id = item.company_location_id,
                                        mediator_id = item.user_id,
                                        last_update_dt = DateTime.Now,
                                        assigned_dt = DateTime.Now,
                                        status_id = 2,
                                        user_id = currentReport.user_id,
                                    });
                                }
                            }
                        }

                        ////////    if (model.mediatorsInvolved != null)
                        {
                            foreach (var item in adv.company_case_routing.Where(x => x.company_id == currentReport.company_id && model.whatHappened.Contains(x.company_secondary_type_id)).ToList())
                            {
                                if ((model.mediatorsInvolved == null) || !model.mediatorsInvolved.Contains(item.user_id.ToString()))
                                {
                                    var inc = adv.company_secondary_type.FirstOrDefault(x => x.id == item.company_secondary_type_id);
                                    if ((inc != null) && (inc.status_id == 2))
                                    {
                                        adv.report_mediator_assigned.Add(new report_mediator_assigned
                                        {
                                            report_id = currentReport.id,
                                            mediator_id = item.user_id,
                                            by_secondary_type_id = item.company_secondary_type_id,
                                            last_update_dt = DateTime.Now,
                                            assigned_dt = DateTime.Now,
                                            status_id = 2,
                                            user_id = currentReport.user_id,
                                        });
                                    }
                                }
                            }
                        }

                        //test = adv.SaveChanges();


                        List<report_non_mediator_involved> mediators = model.GetModeMediatorInvolveds();
                        foreach (var item in mediators)
                        {
                            item.added_by_reporter = true;
                            item.report_id = currentReport.id;
                            adv.report_non_mediator_involved.Add(item);

                        }

                        adv.SaveChanges();
                        #region Create Folder for Report
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Upload/reports/" + _guid));
                        #endregion


                        transaction.Commit();
                        
                        resultModel.report = currentReport;
                        resultModel.StatusCode = 200;
                        #endregion
                        return resultModel;
                    }
                    catch (DbEntityValidationException e)
                    {
                        resultModel.StatusCode = 500;

                        foreach (var eve in e.EntityValidationErrors)
                        {
                            string str = String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            Console.WriteLine(str);
                            resultModel.ErrorMessage = str;

                            foreach (var ve in eve.ValidationErrors)
                            {
                                string tempError = String.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                                Console.WriteLine(tempError);
                                resultModel.ErrorMessage += tempError;
                            }
                        }
                        password = null;
                        transaction.Rollback();
                        return resultModel;
                    }
                    catch (Exception ex)
                    {
                        resultModel.StatusCode = 500;
                        resultModel.ErrorMessage = ex.Message;
                        password = null;
                        transaction.Rollback();
                        return resultModel;
                    }
                }
            }
        }
        private int GetNextColor(int company_id, int report_id)
        {
            using (ECEntities adv = new ECEntities())
            {
                int color_id = 1;

                List<report> reports = new List<report>();


                if (adv.report.Any(item => ((item.company_id == company_id) && (item.id != report_id))))
                    reports = (adv.report.Where(item => ((item.company_id == company_id) && (item.id != report_id))).OrderByDescending(item => item.id)).ToList();

                if (reports.Count > 0)
                {
                    report _report = reports[0];
                    color_id = _report.report_color_id;
                    int _all_colors_count = adv.color.Count();

                    if (color_id < _all_colors_count)
                    {
                        color_id++;
                    }
                    else
                        color_id = 1;
                }

                return color_id;
            }
        }

        public report_department AddReportDepartment(report_department item)
        {
            using (ECEntities adv = new ECEntities())
            {
                adv.report_department.Add(item);
                adv.SaveChanges();
            }
            return item;
        }
    }
}