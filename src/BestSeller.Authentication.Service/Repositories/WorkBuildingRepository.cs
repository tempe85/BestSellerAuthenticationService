using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BestSeller.Authentication.Service.Entities;
using MongoDB.Driver;

namespace BestSeller.Authentication.Service.Repositories
{
    public class WorkBuildingRepository : MongoBaseRepository<WorkBuilding>
    {
        public WorkBuildingRepository(
            IMongoCollection<WorkBuilding> collection) : base(collection)
        {

        }
    }
}