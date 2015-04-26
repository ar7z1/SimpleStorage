﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Domain;
using SimpleStorage.Infrastructure;

namespace SimpleStorage.Controllers
{
    public class AdminController : ApiController
    {
        private readonly IOperationLog operationLog;
        private readonly IStateRepository stateRepository;
        private readonly IStorage storage;

        public AdminController(IStateRepository stateRepository, IStorage storage, IOperationLog operationLog)
        {
            this.stateRepository = stateRepository;
            this.storage = storage;
            this.operationLog = operationLog;
        }

        [HttpPost]
        public void Start()
        {
            stateRepository.SetState(State.Started);
        }

        [HttpPost]
        public void Stop()
        {
            stateRepository.SetState(State.Stopped);
        }

        [HttpPost]
        public void RemoveAllData()
        {
            operationLog.RemoveAll();
            storage.RemoveAll();
        }

        // GET api/values/5 
        [HttpGet]
        public Value InternalGet(string id)
        {
            stateRepository.ThrowIfNotStarted();
            var result = storage.Get(id);
            if (result == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return result;
        }

        // PUT api/values/5
        [HttpPut]
        public void InternalPut(string id, [FromBody] Value value)
        {
            stateRepository.ThrowIfNotStarted();
            storage.Set(id, value);
        }

        [HttpGet]
        public IEnumerable<ValueWithId> GetAllLocalData()
        {
            return storage.GetAll().ToArray();
        }
    }
}