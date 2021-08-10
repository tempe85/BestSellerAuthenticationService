using System;
using BestSeller.Authentication.Service.Dtos;

namespace BestSeller.Authentication.Service.Models
{
    public class WorkStationsByWorkAreaModel
    {
        public Guid WorkAreaId { get; init; }

        public WorkStationDto[] WorkStationDtos { get; init; }
    }

}