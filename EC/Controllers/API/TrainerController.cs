using System;
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

            return new
            {
                Events = DB.TrainerEvents
                    .Where(x => x.DateFrom >= dateFrom && x.DateTo < dateTo)
                    .Select(x => new {
                        title = "",
                        start = x.DateFrom,
                        end = x.DateTo,
                    })
                    .ToList(),
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

            var exists = DB.TrainerEvents.Where(x => x.DateFrom < model.DateTo && model.DateFrom < x.DateTo).ToList();
            //if (exists.Count > DB.user.Count(x => x.role_id == ECLevelConstants.level_trainer))
            if (exists.Count > 0)
            {
                return new
                {
                    Result = false,
                    Code = 1,
                    Message = $"Already there are event from {exists[0].DateFrom.ToString("yyyy/MM/dd hh:mm:ss")} to {exists[0].DateTo.ToString("yyyy/MM/dd hh:mm:ss")}",
                };
            }

            DB.TrainerEvents.Add(new Models.Database.TrainerEvents
            {
                CreatedByUserId = user.id,
                DateFrom = model.DateFrom,
                DateTo = model.DateTo,
                CompanyId = user.company_id,
                Description = "",
                TrainerId = null,
            });
            DB.SaveChanges();

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

            var exists = DB.TrainerTimes.Where(x => x.DateFrom < model.DateTo && model.DateFrom < x.DateTo && x.CreatedByUserId == user.id).ToList();
            //if (exists.Count > DB.user.Count(x => x.role_id == ECLevelConstants.level_trainer))
            if (exists.Count > 0)
            {
                return new
                {
                    Result = false,
                    Code = 1,
                    Message = $"Already there are event from {exists[0].DateFrom.ToString("yyyy/MM/dd hh:mm:ss")} to {exists[0].DateTo.ToString("yyyy/MM/dd hh:mm:ss")}",
                };
            }

            DB.TrainerTimes.Add(new Models.Database.TrainerTimes
            {
                CreatedByUserId = user.id,
                DateFrom = model.DateFrom,
                DateTo = model.DateTo,
                Description = "",
            });
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

            return new
            {
                Events = DB.TrainerTimes
                    .Where(x => x.DateFrom >= dateFrom && x.DateTo < dateTo && x.CreatedByUserId == user.id)
                    .Select(x => new {
                        title = "",
                        start = x.DateFrom,
                        end = x.DateTo,
                        id = x.Id,
                    })
                    .ToList(),
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

            DB.TrainerTimes.Remove(DB.TrainerTimes.FirstOrDefault(x => x.Id == model.Id && x.CreatedByUserId == user.id));
            DB.SaveChanges();

            return new
            {
                Result = true,
            };
        }
    }
}
