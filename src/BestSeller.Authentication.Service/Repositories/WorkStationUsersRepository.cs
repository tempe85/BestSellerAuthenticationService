using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BestSeller.Authentication.Service.Entities;
using MongoDB.Driver;

namespace BestSeller.Authentication.Service.Repositories
{
    public class WorkStationUsersRepository : MongoBaseRepository<WorkStation_Users>
    {
        public WorkStationUsersRepository(
            IMongoCollection<WorkStation_Users> collection) : base(collection)
        {

        }
    }
}