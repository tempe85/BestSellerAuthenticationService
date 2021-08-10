using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BestSeller.Authentication.Service.Entities;
using BestSeller.Authentication.Service.Interfaces;
using MongoDB.Driver;

namespace BestSeller.Authentication.Service.Repositories
{
    public class WorkAreaRepository : MongoBaseRepository<WorkArea>
    {
        public WorkAreaRepository(
            IMongoCollection<WorkArea> collection) : base(collection)
        {

        }
    }
}