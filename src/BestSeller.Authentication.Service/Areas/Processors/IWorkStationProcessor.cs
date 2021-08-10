using System.Threading.Tasks;
using BestSeller.Authentication.Service.Dtos;
using BestSeller.Authentication.Service.Entities;

namespace BestSeller.Authentication.Service.Processors
{
    public interface IWorkStationProcessor
    {
        Task<WorkStation> GetNewWorkStationFromWorkStationDtoAsync(CreateWorkStationDto createWorkStationDto);

        Task UpdateWorkstationFromUpdateWorkStationDtoAsync(UpdateWorkStationDto updateWorkStationDto, WorkStation workStation);
    }
}