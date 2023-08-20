using Gates.Server.Data;
using Gates.Shared.Data;
using Gates.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Gates.Server.Service
{
    public class GateService : IGateService
    {
        private readonly AppDbContext _dbContext;

        public GateService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool AddGate(GateModel model)
        {
            _dbContext.Gates.Add(model);
            int rowsAffected = _dbContext.SaveChanges();
            return rowsAffected > 0;
        }

        public bool RemoveGate(GateModel model)
        {
            _dbContext.Gates.Remove(model);
            int rowsAffected = _dbContext.SaveChanges();
            return rowsAffected > 0;
        }

        public bool RemoveGate(int appId)
        {
            foreach (var entity in _dbContext.Gates.Where(o => o.AppId == appId))
                _dbContext.Gates.Remove(entity);
            int rowsAffected = _dbContext.SaveChanges();
            return rowsAffected > 0;
        }

        public void UpdateGate(GateModel model)
        {
            _dbContext.Gates.Update(model);
            _dbContext.SaveChanges();
        }

        public void OpenGate(GateModel model)
        {
            model.Status = GateStatusEnum.Open.ToString();
            UpdateGate(model);
            _dbContext.SaveChanges();
        }

        public void CloseGate(GateModel model)
        {
            model.Status = GateStatusEnum.Close.ToString();
            UpdateGate(model);
            _dbContext.SaveChanges();
        }

        public async Task<List<GateModel>> GetGate(string webhookState)
        {
            return await _dbContext.Gates.Where(g => g.WebhookState == webhookState).ToListAsync();
        }

        public async Task<List<GateModel>> GetGateByAppId(int appId)
        {
            return await _dbContext.Gates.Where(g => g.AppId == appId).ToListAsync();
        }


        public async Task<bool> CheckGate(GateModel model)
        {
            var check = await _dbContext.Gates.Where(g => g.WebhookState == model.WebhookState && g.Name == model.Name && g.Namespace == model.Namespace && g.Status == GateStatusEnum.Open.ToString()).FirstOrDefaultAsync();
            return check == null ? false : true;
        }

        public void UpdateIsWaiting(GateModel model)
        {
            model.Waiting = true;
            UpdateGate(model);
            _dbContext.SaveChanges();
        }

        public void ResetWaiting(GateModel model)
        {
            model.Waiting = false;
            UpdateGate(model);
            _dbContext.SaveChanges();
        }

        public async Task<GateModel> GetGate(string name, string @namespace, string webhookState)
        {
            return await _dbContext.Gates.Where(g => g.WebhookState == webhookState && g.Name == name && g.Namespace == @namespace).FirstOrDefaultAsync();
        }

        public void ModifyAllGatesStatus(int appId, GateStatusEnum status)
        {
            var gates = _dbContext.Gates.Where(g => g.AppId == appId);
            foreach (var entity in gates)
                CloseGate(entity);
        }
    }
}
