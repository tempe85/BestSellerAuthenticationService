using System;
using System.Linq;
using System.Threading.Tasks;
using BestSeller.Authentication.Service.Dtos;
using BestSeller.Authentication.Service.Entities;
using BestSeller.Authentication.Service.Interfaces;

namespace BestSeller.Authentication.Service.Processors
{

    public class WorkStationProcessor : IWorkStationProcessor
    {
        private readonly IMongoBaseRepository<WorkArea> _workAreaRepository;
        private readonly IMongoBaseRepository<WorkStation> _workStationRepository;

        public WorkStationProcessor(IMongoBaseRepository<WorkArea> workAreaRepository,
                                    IMongoBaseRepository<WorkStation> workStationRepository)
        {
            _workAreaRepository = workAreaRepository;
            _workStationRepository = workStationRepository;
        }

        public async Task<WorkStation> GetNewWorkStationFromWorkStationDtoAsync(CreateWorkStationDto createWorkStationDto)
        {
            var workArea = await _workAreaRepository.GetOneAsync(createWorkStationDto.WorkAreaId);

            var maxOperationPosition = (await workArea.GetWorkAreaWorkStationsAsync(_workStationRepository))?.Select(p => p.WorkAreaPosition).ToArray().Max() ?? -1;
            if (workArea == null)
            {
                throw new Exception($"Unable to find work area: {createWorkStationDto.WorkAreaId} for created work station");
            }
            return new WorkStation
            {
                CreatedDate = DateTimeOffset.UtcNow,
                Description = createWorkStationDto.Description,
                Id = Guid.NewGuid(),
                Name = createWorkStationDto.Name,
                WorkAreaPosition = maxOperationPosition + 1,
                WorkerCapacity = createWorkStationDto.WorkerCapacity,
                WorkStationType = createWorkStationDto.WorkStationType,
            };
        }

        public async Task UpdateWorkstationFromUpdateWorkStationDtoAsync(UpdateWorkStationDto updateWorkStationDto, WorkStation workStation)
        {
            workStation.Description = updateWorkStationDto.Description;
            workStation.Name = updateWorkStationDto.Name;
            workStation.isDeleted = updateWorkStationDto.isDeleted;

            await _workStationRepository.UpdateAsync(workStation);
        }
    }
}