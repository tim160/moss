﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using EC.Constants;
using EC.Models.Database;

namespace EC.Controllers.API
{
    public class TrainerController : BaseApiController
    {
        [HttpGet]
        [Route("api/Trainer/get")]
        public object Get(DateTime dateFrom, DateTime dateTo)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
            {
                return null;
            }

            dateFrom = dateFrom.Date;
            dateTo = dateTo.Date.AddDays(1);
            var trainers = DB.TrainerTimes
                .Select(x => x.CreatedByUserId)
                .Distinct()
                .ToList();

            var colors = DB.color.OrderBy(x => x.id).Select(x => x.color_code).ToList();
            while (colors.Count < trainers.Count)
            {
                colors.AddRange(colors);
            }

            var times = DB.user
                .Where(x => trainers.Contains(x.id))
                .ToList()
                .Select((x, idx) => new {
                    x.id,
                    x.first_nm,
                    x.last_nm,
                    //color = $"#{colors[idx]}",
                    color = "green",
                    rendering = "background",
                    events = DB.TrainerTimes
                        .Where(z => z.CreatedByUserId == x.id && z.Hour >= dateFrom && z.Hour <= dateTo)
                        .Select(z => z.Hour)
                        .Distinct()
                        .ToList()
                        .Select(z => new {
                            title = $"{x.first_nm} {x.last_nm}",
                            start = new DateTime(z.Year, z.Month, z.Day, z.Hour, 0, 0),
                            end = new DateTime(z.Year, z.Month, z.Day, z.Hour + 1, 0, 0),
                        })
                    .ToList(),
                })
                .ToList();

            var now = DateTime.Now.AddDays(1).Date;
            var availableTimes = new[]
            {
                new {
                    color = "green",
                    rendering = "background",
                    events = DB.TrainerTimes
                        .Where(z => z.Hour >= dateFrom && z.Hour > now && z.Hour <= dateTo && z.CompanyId == null)
                        .Select(z => z.Hour)
                        .Distinct()
                        .ToList()
                        .Select(z => new
                        {
                            title = "",
                            start = new DateTime(z.Year, z.Month, z.Day, z.Hour, 0, 0),
                            end = new DateTime(z.Year, z.Month, z.Day, z.Hour + 1, 0, 0),
                        })
                        .ToList()
                },
            };

            return new
            {
                Events = new {
                    color = "darkmagenta",
                    events = DB.TrainerTimes
                        .Where(x => x.CompanyId == user.company_id && x.Hour >= dateFrom && x.Hour <= dateTo)
                        .ToList()
                        .Select(z => new {
                            title = "",
                            start = new DateTime(z.Hour.Year, z.Hour.Month, z.Hour.Day, z.Hour.Hour, 0, 0),
                            end = new DateTime(z.Hour.Year, z.Hour.Month, z.Hour.Day, z.Hour.Hour + 1, 0, 0),
                        }).ToList()
                },
                AvailableTimes = availableTimes,
            };
        }

        public class AddEventModel
        {
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
        }

        [HttpPost]
        [Route("api/Trainer/AddEvent")]
        public object AddEvent(AddEventModel model)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
            {
                return null;
            }

            var now = DateTime.Now.Date;
            if (model.DateFrom < now.AddDays(1))
            {
                return new
                {
                    Result = false,
                    Code = 2,
                    Message = $"You can book training from tomorrow only",
                };
            }
            if (DB.TrainerTimes.Count(x => x.CompanyId == user.company_id && x.Hour >= DateTime.Now) > 0)
            {
                return new
                {
                    Result = false,
                    Code = 2,
                    Message = $"You cannot book training as you have it already booked",
                };                
            }

            model.DateFrom = new DateTime(model.DateFrom.Year, model.DateFrom.Month, model.DateFrom.Day, model.DateFrom.Hour, 0, 0);
            var exists = DB.TrainerTimes.Where(x => x.Hour == model.DateFrom && x.Hour > now).ToList();
            if (exists.Count(x => x.CompanyId == null) == 0)
            {
                return new 
                {
                    Result = false,
                    Code = 1,
                    Message = $"{model.DateFrom.ToString("yyyy/MM/dd hh:mm:ss")} not available",
                };
            }

            var item = exists.FirstOrDefault(x => x.CompanyId == null);
            item.CompanyId = user.company_id;
            DB.SaveChanges();

            EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(is_cc);
            EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.RequestUri.AbsoluteUri.ToLower());
            eb.CalendarEvent(true, true, Request.RequestUri.AbsoluteUri.ToLower(), item.Hour.ToString("yyyy-MM-dd HH:mm:ss"));
            string body = eb.Body;
            var mediator = DB.user.FirstOrDefault(x => x.company_id == item.CompanyId && x.role_id == ECLevelConstants.level_administrator);
            if (mediator != null)
            {
                //em.Send("alexandr@ase.com.ua", "New book training added", body, "", true);
                em.Send(mediator.email, "New book training added", body, "", true);
            }

            eb.CalendarEvent(false, true, Request.RequestUri.AbsoluteUri.ToLower(), item.Hour.ToString("yyyy-MM-dd HH:mm:ss"));
            body = eb.Body;
            var trainer = DB.user.FirstOrDefault(x => x.id == item.CreatedByUserId);
            if (trainer != null)
            {
                //em.Send("alexandr@ase.com.ua", "New book training added", body, "", true);
                em.Send(trainer.email, "New book training added", body, "", true);
            }

            return new
            {
                Result = true,
            };
        }

        [HttpPost]
        [Route("api/Trainer/AddTime")]
        public object AddTime(AddEventModel model)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
            {
                return null;
            }

            if (model.DateFrom.Date < DateTime.Now.AddDays(1).Date)
            {
                return new
                {
                    Result = false,
                    Code = 2,
                    Message = $"You can add time only after {DateTime.Now.AddDays(1).Date.ToString("yyyy/MM/dd HH:mm:ss")}",
                };
            }

            for (var i = 0; i < (int)(model.DateTo - model.DateFrom).TotalHours; i++)
            {
                var h = model.DateFrom.AddHours(i);
                var item = DB.TrainerTimes.FirstOrDefault(x => x.Hour == h && x.CreatedByUserId == user.id);
                if (item == null)
                {
                    DB.TrainerTimes.Add(new Models.Database.TrainerTimes
                    {
                        CreatedByUserId = user.id,
                        Hour = h,
                        Description = "",
                    });
                }
            }
            DB.SaveChanges();

            return new
            {
                Result = true,
            };
        }

        [HttpGet]
        [Route("api/Trainer/getTrainer")]
        public object GetTrainer(DateTime dateFrom, DateTime dateTo)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
            {
                return null;
            }

            dateFrom = dateFrom.Date;
            dateTo = dateTo.Date.AddDays(1);

            var events = DB.TrainerTimes
                    .Where(x => x.Hour >= dateFrom && x.Hour < dateTo && x.CreatedByUserId == user.id)
                    .ToList()
                    .Select(z => new {
                        title = $"",
                        start = new DateTime(z.Hour.Year, z.Hour.Month, z.Hour.Day, z.Hour.Hour, 0, 0),
                        end = new DateTime(z.Hour.Year, z.Hour.Month, z.Hour.Day, z.Hour.Hour + 1, 0, 0),
                        companyId = z.CompanyId,
                        //id = z.Id,
                    })
                    .ToList();

            return new
            {
                Events = new[]
                {
                    new
                    {
                        color = "#3a87ad",
                        events = events.Where(x => x.companyId == null),
                    },
                    new
                    {
                        color = "darkmagenta",
                        events = events.Where(x => x.companyId != null),
                    },
                },
            };
        }

        [HttpPost]
        [Route("api/Trainer/deleteTime")]
        public object DeleteTime(TrainerTimes model)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
            {
                return null;
            }

            DB.TrainerTimes.Remove(DB.TrainerTimes.FirstOrDefault(x => x.Hour == model.Hour && x.CreatedByUserId == user.id));
            DB.SaveChanges();

            return new
            {
                Result = true,
            };
        }

        [HttpPost]
        [Route("api/Trainer/deleteCompanyTime")]
        public object DeleteCompanyTime(TrainerTimes model)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
            {
                return null;
            }

            var item = DB.TrainerTimes.FirstOrDefault(x => x.Hour == model.Hour && x.CompanyId == user.company_id);
            if (item == null)
            {
                return new
                {
                    Result = false,
                    Code = 2,
                    Message = $"Event not found",
                };
            }
            if (item.Hour < DateTime.Now.AddDays(3).Date)
            {
                return new
                {
                    Result = false,
                    Code = 2,
                    Message = $"You cannot cancel this training as it is less than 3 days",
                };
            }
            var companyId = item.CompanyId;
            item.CompanyId = null;
            DB.SaveChanges();

            EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(is_cc);
            EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.RequestUri.AbsoluteUri.ToLower());
            eb.CalendarEvent(true, true, Request.RequestUri.AbsoluteUri.ToLower(), item.Hour.ToString("yyyy-MM-dd HH:mm:ss"));
            string body = eb.Body;
            var mediator = DB.user.FirstOrDefault(x => x.company_id == companyId && x.role_id == ECLevelConstants.level_administrator);
            if (mediator != null)
            {
                //em.Send("alexandr@ase.com.ua", "The book training have removed", body, "", true);
                em.Send(mediator.email, "The book training have removed", body, "", true);
            }

            eb.CalendarEvent(false, true, Request.RequestUri.AbsoluteUri.ToLower(), item.Hour.ToString("yyyy-MM-dd HH:mm:ss"));
            body = eb.Body;
            var trainer = DB.user.FirstOrDefault(x => x.id == item.CreatedByUserId);
            if (trainer != null)
            {
                //em.Send("alexandr@ase.com.ua", "The book training have removed", body, "", true);
                em.Send(trainer.email, "The book training have removed", body, "", true);
            }

            return new
            {
                Result = true,
            };
        }        
    }
}
