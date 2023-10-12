using AutoMapper;
using QA_checks.DtoModels;
using QA_checks.Models;

namespace QA_checks.Profiles
{
    public class QaOdersProfile : Profile
    {
        public QaOdersProfile() 
        {
            CreateMap<DtoOrderModel, Order> ();
            CreateMap<Order, DtoOrderModel>();
            CreateMap<DtoQaChecks, QAchecks>();
        }
    }
}
